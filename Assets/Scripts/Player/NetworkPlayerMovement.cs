using Fusion;
using UnityEngine;

namespace Player
{
    public class NetworkPlayerMovement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float sprintSpeed = 5.335f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 10.0f;

        private NetworkCharacterController _networkController;

        private float _rotationVelocity;
        private float _animationSpeedBlend;
        private float _animationInputXBlend;
        private float _animationInputZBlend;

        public override void Spawned()
        {
            _networkController = GetComponent<NetworkCharacterController>();
        }

        public void HandleMoveFusion(Vector3 moveDirection, bool isSprinting, NetworkPlayerAnimation animation)
        {
            float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
            float inputMagnitude = moveDirection.magnitude;

            Rotate(moveDirection);
            Move(moveDirection, targetSpeed, inputMagnitude);
            BlendAnimations(moveDirection, targetSpeed, inputMagnitude, animation);
        }

        private void Rotate(Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero)
                return;

            float targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        private void Move(Vector3 moveDirection, float targetSpeed, float inputMagnitude)
        {
            float actualSpeed = targetSpeed * inputMagnitude;

            _networkController.Move(moveDirection * (actualSpeed * Time.fixedDeltaTime));

            Vector3 horizontalVelocity = new Vector3(_networkController.Velocity.x, 0, _networkController.Velocity.z);
            if (horizontalVelocity.magnitude > actualSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * actualSpeed;
                _networkController.Velocity = new Vector3(horizontalVelocity.x, _networkController.Velocity.y, horizontalVelocity.z);
            }
        }


        private void BlendAnimations(Vector3 moveDirection, float targetSpeed, float inputMagnitude, NetworkPlayerAnimation animation)
        {
            float targetAnimSpeedBlend = (moveDirection == Vector3.zero) ? 0f : targetSpeed;
            _animationSpeedBlend = Mathf.Lerp(_animationSpeedBlend, targetAnimSpeedBlend, Time.fixedDeltaTime * speedChangeRate);

            Vector3 localMoveDir = transform.InverseTransformDirection(moveDirection);

            if (moveDirection == Vector3.zero)
            {
                _animationInputXBlend = Mathf.Lerp(_animationInputXBlend, 0, Time.fixedDeltaTime * speedChangeRate);
                _animationInputZBlend = Mathf.Lerp(_animationInputZBlend, 0, Time.fixedDeltaTime * speedChangeRate);
            }
            else
            {
                _animationInputXBlend = Mathf.Lerp(_animationInputXBlend, localMoveDir.x, Time.fixedDeltaTime * speedChangeRate);
                _animationInputZBlend = Mathf.Lerp(_animationInputZBlend, localMoveDir.z, Time.fixedDeltaTime * speedChangeRate);
            }

            animation.UpdateMovementAnimations(_animationSpeedBlend, inputMagnitude, _animationInputXBlend, _animationInputZBlend);
        }
    }
}