using UnityEngine;
using Fusion;
using Network;


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
                Debug.Log("Applying raw jump boost!");

                var vel = _networkCharacterController.Velocity;
                vel.y = 15f;

                _networkCharacterController.Velocity = vel;

                _animation.SetJumping(true);
            }
            else if (_networkCharacterController.Grounded)
            {
                _animation.SetJumping(false);
            }
        }
    }
}