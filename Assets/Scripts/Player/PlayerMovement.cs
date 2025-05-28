using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float sprintSpeed = 5.335f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 10.0f;

        private CharacterController _controller;
        private float _speed;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _animationSpeedBlend;
        private float _animationInputXBlend;
        private float _animationInputZBlend;
        private bool _strafeOnAim = false;
        private bool _rotateOnMove = true;

        private Vector2 _moveInput;
        private bool _sprint;
        private float _verticalVelocity;
        private GameObject _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main.gameObject;
        }

        public void Initialize(CharacterController controller) => _controller = controller;
        public void SetMoveInput(Vector2 input) => _moveInput = input;
        public void SetSprintInput(bool isSprinting) => _sprint = isSprinting;
        public void SetVerticalVelocity(float velocity) => _verticalVelocity = velocity;

        public void HandleMove(PlayerAnimation animation)
        {
            float targetSpeed = _sprint ? sprintSpeed : moveSpeed;
            if (_moveInput == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _moveInput.magnitude;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationSpeedBlend = Mathf.Lerp(_animationSpeedBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationSpeedBlend < 0.01f) _animationSpeedBlend = 0f;

            Vector3 inputDirection = new Vector3(_moveInput.x, 0.0f, _moveInput.y).normalized;

            if (_moveInput != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
                if (_rotateOnMove) transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            Vector3 move = targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
            _controller.Move(move);

            Vector3 localMoveDirection = transform.InverseTransformDirection(targetDirection);
            if (_moveInput == Vector2.zero)
            {
                _animationInputXBlend = Mathf.Lerp(_animationInputXBlend, 0, Time.deltaTime * speedChangeRate);
                _animationInputZBlend = Mathf.Lerp(_animationInputZBlend, 0, Time.deltaTime * speedChangeRate);
            }
            else
            {
                _animationInputXBlend = Mathf.Lerp(_animationInputXBlend, localMoveDirection.x, Time.deltaTime * speedChangeRate);
                _animationInputZBlend = Mathf.Lerp(_animationInputZBlend, localMoveDirection.z, Time.deltaTime * speedChangeRate);
            }

            animation.UpdateMovementAnimations(_animationSpeedBlend, inputMagnitude, _strafeOnAim, _animationInputXBlend, _animationInputZBlend);
        }
    }
}