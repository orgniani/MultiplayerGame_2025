using Fusion;
using UnityEngine;
using Managers;

namespace Player
{
    [RequireComponent(typeof(NetworkCharacterController))]
    public class NetworkPlayerRespawn : NetworkBehaviour
    {
        [SerializeField] private float deathHeightThreshold = -50f;
        private NetworkCharacterController _networkCharacterController;

        private void Awake()
        {
            _networkCharacterController = GetComponent<NetworkCharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority)
                return;

            if (transform.position.y < deathHeightThreshold)
            {
                RespawnPlayer();
            }
        }

        private void RespawnPlayer()
        {
            Vector3 respawnPosition = NetworkManager.Instance.GetRespawnPoint(Object.InputAuthority);
            _networkCharacterController.Teleport(respawnPosition);
            Debug.Log($"Player {Object.InputAuthority} respawned at {respawnPosition}");
        }
    }
}
