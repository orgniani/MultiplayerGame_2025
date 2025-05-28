using UnityEngine;
using Inputs;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerJump))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerAudio))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Input Reader")]
        [SerializeField] private InputReader inputReader;

        private PlayerMovement _movement;
        private PlayerJump _jump;
        private PlayerAnimation _animationModule;
        private PlayerAudio _audioModule;
        private CharacterController _controller;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _jump = GetComponent<PlayerJump>();
            _animationModule = GetComponent<PlayerAnimation>();
            _audioModule = GetComponent<PlayerAudio>();
            _controller = GetComponent<CharacterController>();

            InitializeAll();
        }

        private void OnEnable()
        {
            inputReader.OnMoveInput += _movement.SetMoveInput;
            inputReader.OnJumpInput += _jump.SetJumpInput;
            inputReader.OnSprintInput += _movement.SetSprintInput;
        }

        private void OnDisable()
        {
            inputReader.OnMoveInput -= _movement.SetMoveInput;
            inputReader.OnJumpInput -= _jump.SetJumpInput;
            inputReader.OnSprintInput -= _movement.SetSprintInput;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            _jump.HandleJumpAndGravity(_animationModule, _movement);
            _movement.HandleMove(_animationModule);
            _jump.GroundedCheck(_animationModule);
        }

        private void InitializeAll()
        {
            _movement.Initialize(_controller);
            _audioModule.Initialize(_controller);
        }
    }
}