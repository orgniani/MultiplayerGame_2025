using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Helpers;

namespace Inputs
{
    public class InputReader : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Action Keys")]
        [SerializeField] private string moveAction = "Move";
        [SerializeField] private string jumpAction = "Jump";
        [SerializeField] private string lookAction = "Look";
        [SerializeField] private string sprintAction = "Sprint";

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;

        public event Action<Vector2> OnMoveInput;
        public event Action<Vector2> OnLookInput;
        public event Action<bool> OnJumpInput;
        public event Action<bool> OnSprintInput;

        private void Awake()
        {
            ValidateReferences();
        }

        private void OnEnable()
        {
            _moveAction = inputActions.FindAction(moveAction);
            if (_moveAction != null)
            {
                _moveAction.performed += HandleMovementInput;
                _moveAction.canceled += HandleMovementInput;
            }

            _jumpAction = inputActions.FindAction(jumpAction);
            if (_jumpAction != null)
            {
                _jumpAction.started += HandleJumpInput;
                _jumpAction.canceled += HandleJumpInput;
            }

            _lookAction = inputActions.FindAction(lookAction);
            if (_lookAction != null)
            {
                _lookAction.performed += HandleCameraInput;
                _lookAction.canceled += HandleCameraInput;
            }

            _sprintAction = inputActions.FindAction(sprintAction);
            if (_sprintAction != null)
            {
                _sprintAction.started += HandleSprintInput;
                _sprintAction.canceled += HandleSprintInput;
            }
        }

        private void OnDisable()
        {
            if (_moveAction != null)
            {
                _moveAction.performed -= HandleMovementInput;
                _moveAction.canceled -= HandleMovementInput;
            }

            if (_jumpAction != null)
            {
                _jumpAction.started -= HandleJumpInput;
                _jumpAction.canceled -= HandleJumpInput;
            }

            if (_lookAction != null)
            {
                _lookAction.performed -= HandleCameraInput;
                _lookAction.canceled -= HandleCameraInput;
            }

            if (_sprintAction != null)
            {
                _sprintAction.started -= HandleSprintInput;
                _sprintAction.canceled -= HandleSprintInput;
            }
        }

        private void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            OnMoveInput?.Invoke(input);
        }

        private void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            bool isPressed = ctx.phase == InputActionPhase.Started;
            OnJumpInput?.Invoke(isPressed);
        }

        private void HandleSprintInput(InputAction.CallbackContext ctx)
        {
            bool isPressed = ctx.phase == InputActionPhase.Started;
            OnSprintInput?.Invoke(isPressed);
        }

        private void HandleCameraInput(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            OnLookInput?.Invoke(input);
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(inputActions, nameof(inputActions), this);
        }
    }
}
