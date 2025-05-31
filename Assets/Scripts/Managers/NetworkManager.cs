using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using Common;
using Player;
using Inputs;

namespace Managers
{
    public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, INetworkRunnerCallbacks
    {
        [Header("References")]
        [SerializeField] private Transform finishLine;
        [SerializeField] private Transform[] spawnPositions;

        [Header("Prefabs")]
        [SerializeField] private NetworkPrefabRef playerPrefab;
        [SerializeField] private NetworkPrefabRef timerManagerPrefab;
        [SerializeField] private NetworkPrefabRef racePositionManagerPrefab;

        private RacePositionManager _racePositionManager;
        
        private readonly Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
        private NetworkRunner _networkRunner;

        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnNewPlayerJoined;
        public event Action<string> OnJoinedPlayerLeft;

        public NetworkPlayerSetup LocalPlayer { get; set; }

        private float _jumpBufferTimer;
        private float _jumpBufferDuration = 0.1f;

        async void Start ()
        {
            bool sessionStarted = await StartGameSession();

            if (!sessionStarted)
                Debug.LogError("Could not start game session!");
        }

        void OnApplicationQuit ()
        {
            Shutdown();
        }

        private async Task<bool> StartGameSession ()
        {
            GameObject networkRunnerObject = new GameObject(typeof(NetworkRunner).Name, typeof(NetworkRunner));

            _networkRunner = networkRunnerObject.GetComponent<NetworkRunner>();
            _networkRunner.AddCallbacks(this);

            StartGameArgs startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                SceneManager = _networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                PlayerCount = spawnPositions.Length
            };

            Task<StartGameResult> startTask = _networkRunner.StartGame(startGameArgs);
            await startTask;

            return startTask.Result.Ok;
        }

        private void Shutdown ()
        {
            if (_networkRunner)
                _networkRunner.Shutdown();
        }

        private void SpawnNewPlayer(NetworkRunner runner, PlayerRef player)
        {
            Vector3 spawnPosition = spawnPositions[_spawnedPlayers.Count].position;
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            _spawnedPlayers.Add(player, networkPlayerObject);

            _racePositionManager.RegisterPlayer(player, networkPlayerObject.transform);
            _racePositionManager.SetFinishLine(finishLine);
        }

        private void DespawnPlayer (NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedPlayers.ContainsKey(player))
            {
                runner.Despawn(_spawnedPlayers[player]);

                if (_racePositionManager != null)
                    _racePositionManager.UnregisterPlayer(player);

                _spawnedPlayers.Remove(player);
            }
        }

        public Vector3 GetRespawnPoint(PlayerRef player)
        {
            int index = player.PlayerId % spawnPositions.Length;
            return spawnPositions[index].position;
        }

        void INetworkRunnerCallbacks.OnConnectedToServer (NetworkRunner runner)
        {
            if (_networkRunner.IsClient)
                OnConnected?.Invoke();
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer (NetworkRunner runner, NetDisconnectReason reason)
        {
            if (_networkRunner.IsClient)
                Shutdown();
        }

        void INetworkRunnerCallbacks.OnShutdown (NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason == ShutdownReason.GameNotFound)
                return;

            if (_networkRunner.IsServer)
                _spawnedPlayers.Clear();

            _networkRunner = null;

            OnDisconnected?.Invoke();
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                if (FindFirstObjectByType<TimerManager>() == null)
                    runner.Spawn(timerManagerPrefab, Vector3.zero, Quaternion.identity);

                _racePositionManager = FindFirstObjectByType<RacePositionManager>();
                if (_racePositionManager == null)
                    _racePositionManager = runner.Spawn(racePositionManagerPrefab, Vector3.zero, Quaternion.identity).GetComponent<RacePositionManager>();

                SpawnNewPlayer(runner, player);
            }

            OnNewPlayerJoined?.Invoke("Player_" + player.PlayerId);
        }

        void INetworkRunnerCallbacks.OnPlayerLeft (NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                DespawnPlayer(runner, player);

                if (_spawnedPlayers.Count == 0)
                    Shutdown();
            }

            OnJoinedPlayerLeft?.Invoke("Player_" + player.PlayerId);
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!LocalPlayer)
                return;

            NetworkInputData networkInput = new NetworkInputData();

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            bool isJumpingKeyPressed = Input.GetKey(KeyCode.Space);

            if (Input.GetKeyDown(KeyCode.Space))
                _jumpBufferTimer = _jumpBufferDuration;

            if (_jumpBufferTimer > 0f)
                _jumpBufferTimer -= Time.deltaTime;

            networkInput.LookDirection = LocalPlayer.GetNormalizedLookDirection();

            if (verticalInput > 0f)
                networkInput.AddInput(NetworkInputType.MoveForward);
            else if (verticalInput < 0f)
                networkInput.AddInput(NetworkInputType.MoveBackwards);

            if (horizontalInput < 0f)
                networkInput.AddInput(NetworkInputType.MoveLeft);
            else if (horizontalInput > 0f)
                networkInput.AddInput(NetworkInputType.MoveRight);

            if (isSprinting)
                networkInput.AddInput(NetworkInputType.Sprint);

            if (_jumpBufferTimer > 0f || isJumpingKeyPressed)
                networkInput.AddInput(NetworkInputType.Jump);

            input.Set(networkInput);
        }

        void INetworkRunnerCallbacks.OnInputMissing (NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnConnectRequest (NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        void INetworkRunnerCallbacks.OnConnectFailed (NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        void INetworkRunnerCallbacks.OnUserSimulationMessage (NetworkRunner runner, SimulationMessagePtr message) { }
        void INetworkRunnerCallbacks.OnSessionListUpdated (NetworkRunner runner, List<SessionInfo> sessionList) { }
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse (NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnHostMigration (NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone (NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart (NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnObjectExitAOI (NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnObjectEnterAOI (NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived (NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnReliableDataProgress (NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    }
}