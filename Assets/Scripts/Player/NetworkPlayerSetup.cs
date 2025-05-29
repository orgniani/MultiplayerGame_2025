using UnityEngine;
using Managers;
using Fusion;
using Cameras;
using Inputs;

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

        private NetworkCharacterController _networkCharacterController;
        private NetworkPlayerAnimation _animation;
        private NetworkPlayerMovement _movement;
        private NetworkPlayerJump _jump;
        private CameraTracker _cameraTracker;

        public override void Spawned()
        {
            if (!Object.HasInputAuthority)
                return;

            _cameraTracker = FindAnyObjectByType<CameraTracker>();
            _cameraTracker.SetFollowTarget(cameraTarget);
            NetworkManager.Instance.LocalPlayer = this;
        }

        private void Awake()
        {
            _networkCharacterController = GetComponent<NetworkCharacterController>();
            _movement = GetComponent<NetworkPlayerMovement>();
            _jump = GetComponent<NetworkPlayerJump>();
            _animation = GetComponent<NetworkPlayerAnimation>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out NetworkInputData networkInput))
                return;

            Vector3 moveDirection = GetMoveDirection(networkInput);
            bool isSprinting = networkInput.IsInputDown(NetworkInputType.Sprint);

            _movement.HandleMoveFusion(moveDirection, isSprinting, _animation);
            _jump.HandleJumpFusion(networkInput);
            _animation.UpdateGrounded(_networkCharacterController.Grounded);
        }

        private Vector3 GetMoveDirection(NetworkInputData input)
        {
            Vector3 moveDirection = Vector3.zero;

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
    }
}