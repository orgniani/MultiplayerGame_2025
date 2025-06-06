using UnityEngine;
using Managers;
using Fusion;
using Cameras;
using Inputs;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    [RequireComponent(typeof(NetworkPlayerMovement))]
    [RequireComponent(typeof(NetworkPlayerJump))]
    [RequireComponent(typeof(NetworkPlayerAnimation))]
    [RequireComponent(typeof(CharacterController))]
    public class NetworkPlayerSetup : NetworkBehaviour
    {   
        [Header("Camera Follow Target")]
        [SerializeField] private Transform cameraTarget;

        [Header("Children")]
        [SerializeField] private GameObject _collider;
        [SerializeField] private GameObject _visuals;

        private NetworkCharacterController _networkCharacterController;
        private NetworkPlayerAnimation _animation;
        private NetworkPlayerMovement _movement;
        private NetworkPlayerJump _jump;
        private CameraTracker _cameraTracker;

        private GameOverManager _gameOverManager;
        [Networked] public NetworkString<_16> PlayerName { get; set; }

        public static readonly Dictionary<PlayerRef, NetworkPlayerSetup> PlayersByRef = new();
        public static readonly Dictionary<PlayerRef, string> PlayerNames = new();

        public Transform GetCameraTarget() => cameraTarget;
        public CameraTracker GetCameraTracker() => _cameraTracker;

        public override void Spawned()
        {
            PlayersByRef[Object.InputAuthority] = this;

            if (Object.HasInputAuthority)
            {
                RpcSubmitNameToHost(PlayerInfo.PlayerName);

                _cameraTracker = FindAnyObjectByType<CameraTracker>();
                _cameraTracker.SetFollowTarget(cameraTarget);

                NetworkManager.Instance.RegisterLocalPlayerInput(this);

                RpcRequestAllNamesFromHost();
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayersByRef.Remove(Object.InputAuthority);
        }

        private void Awake()
        {
            _networkCharacterController = GetComponent<NetworkCharacterController>();
            _movement = GetComponent<NetworkPlayerMovement>();
            _jump = GetComponent<NetworkPlayerJump>();
            _animation = GetComponent<NetworkPlayerAnimation>();
        }

        private IEnumerator Start()
        {
            while (_gameOverManager == null)
            {
                _gameOverManager = NetworkManager.Instance.GameOverManager;
                yield return null;
            }
        }

        public override void FixedUpdateNetwork()
        {
            _animation.UpdateGrounded(_networkCharacterController.Grounded);

            bool isInAir = !_networkCharacterController.Grounded && _networkCharacterController.Velocity.y < 0f;
            _animation.SetFreeFall(isInAir);

            if (!GetInput(out NetworkInputData networkInput))
                return;

            Vector3 moveDirection = GetMoveDirection(networkInput);
            bool isSprinting = networkInput.IsInputDown(NetworkInputType.Sprint);

            _movement.HandleMoveFusion(moveDirection, isSprinting, _animation);
            _jump.HandleJumpFusion(networkInput);
        }

        private Vector3 GetMoveDirection(NetworkInputData input)
        {
            Vector3 moveDirection = Vector3.zero;

            if (_gameOverManager != null && _gameOverManager.IsGameOver)
                return moveDirection;

            Vector3 forward = input.LookDirection;
            Vector3 right = Quaternion.AngleAxis(90f, Vector3.up) * input.LookDirection;

            if (input.IsInputDown(NetworkInputType.MoveForward)) moveDirection += forward;
            if (input.IsInputDown(NetworkInputType.MoveBackwards)) moveDirection -= forward;
            if (input.IsInputDown(NetworkInputType.MoveLeft)) moveDirection -= right;
            if (input.IsInputDown(NetworkInputType.MoveRight)) moveDirection += right;

            moveDirection.y = 0f;
            return moveDirection.normalized;
        }

        public Vector3 GetNormalizedLookDirection()
        {
            if (!_cameraTracker)
                return transform.forward;

            Vector3 lookDirection = _cameraTracker.transform.forward;
            lookDirection.y = 0f;
            return lookDirection.normalized;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RpcRequestAllNamesFromHost()
        {
            Debug.Log($"[CLIENT] Requesting all names from host (I'm player {Object.InputAuthority.PlayerId})");

            var copy = new List<KeyValuePair<PlayerRef, string>>(PlayerNames);

            foreach (var kvp in copy)
                RpcDistributeNameToOne(Object.InputAuthority, kvp.Key, kvp.Value);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RpcSubmitNameToHost(string name)
        {
            Debug.Log($"[HOST] Received name '{name}' from player {Object.InputAuthority.PlayerId}");

            PlayerNames[Object.InputAuthority] = name;
            RpcDistributeName(Object.InputAuthority, name);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RpcDistributeName(PlayerRef playerRef, string name)
        {
            Debug.Log($"[ALL] Setting name for PlayerRef {playerRef.PlayerId}: {name}");
            PlayerNames[playerRef] = name;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RpcDistributeNameToOne(PlayerRef targetPlayer, PlayerRef nameOwner, string name)
        {
            if (Runner.LocalPlayer != targetPlayer)
                return;

            Debug.Log($"[LateJoiner] Received name for Player {nameOwner.PlayerId}: {name}");
            PlayerNames[nameOwner] = name;
        }

        public void HideVisuals()
        {
            _collider.SetActive(false);
            _visuals.SetActive(false);

            if (TryGetComponent<CharacterController>(out var controller))
                controller.enabled = false;
        }
    }
}