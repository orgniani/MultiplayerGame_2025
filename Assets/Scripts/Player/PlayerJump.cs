using UnityEngine;

namespace Player
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundedOffset = -0.14f;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -15.0f;
        [SerializeField] private float jumpTimeout = 0.50f;
        [SerializeField] private float fallTimeout = 0.15f;

        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private bool _grounded;
        private bool _jumpInput;

        private void Start()
        {
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        public void SetJumpInput(bool isJumping)
        {
            _jumpInput = isJumping;
        }

        public void GroundedCheck(PlayerAnimation animation)
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
            animation.UpdateGrounded(_grounded);
        }

        public void HandleJumpAndGravity(PlayerAnimation animation, PlayerMovement movement)
        {
            if (_grounded)
            {
                _fallTimeoutDelta = fallTimeout;

                animation.SetJumping(false);
                animation.SetFreeFall(false);

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_jumpInput && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    animation.SetJumping(true);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else
                    animation.SetFreeFall(true);

                _jumpInput = false;
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += gravity * Time.deltaTime;

            movement.SetVerticalVelocity(_verticalVelocity);
        }
    }
}