using UnityEngine;
using Fusion;
using Inputs;


namespace Player
{
    [RequireComponent(typeof(NetworkCharacterController))]
    [RequireComponent(typeof(NetworkPlayerAnimation))]
    public class NetworkPlayerJump : NetworkBehaviour
    {
        private NetworkCharacterController _networkCharacterController;
        private NetworkPlayerAnimation _animation;

        public override void Spawned()
        {
            _networkCharacterController = GetComponent<NetworkCharacterController>();
            _animation = GetComponent<NetworkPlayerAnimation>();
        }

        public void HandleJumpFusion(NetworkInputData inputData)
        {
            if (inputData.IsInputDown(NetworkInputType.Jump) && _networkCharacterController.Grounded)
            {
                _networkCharacterController.Jump();
                _animation.SetJumping(true);
            }
            else if (_networkCharacterController.Grounded)
            {
                _animation.SetJumping(false);
            }
        }
    }
}