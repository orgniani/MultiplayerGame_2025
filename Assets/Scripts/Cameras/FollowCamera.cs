using UnityEngine;
using Inputs;
using Unity.Cinemachine;

namespace Cameras
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private Transform cameraTarget;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 150f;

        private Vector2 _lookInput;
        private float _yaw;
        private float _pitch;

        private void OnEnable()
        {
            inputReader.OnLookInput += HandleLookInput;
        }

        private void OnDisable()
        {
            inputReader.OnLookInput -= HandleLookInput;
        }

        private void HandleLookInput(Vector2 lookInput)
        {
            _lookInput = lookInput;
        }

        private void Update()
        {
            if (_lookInput.sqrMagnitude >= 0.01f)
            {
                _yaw += _lookInput.x * rotationSpeed * Time.deltaTime;
                _pitch -= _lookInput.y * rotationSpeed * Time.deltaTime;
                _pitch = Mathf.Clamp(_pitch, -30f, 70f);

                cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            }
        }
    }
}