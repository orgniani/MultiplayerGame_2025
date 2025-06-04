using UnityEngine;
using Fusion;

namespace Player
{
    [RequireComponent(typeof(NetworkMecanimAnimator))]
    public class NetworkPlayerAnimation : NetworkBehaviour
    {
        private NetworkMecanimAnimator _networkAnimator;

        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDGrounded = Animator.StringToHash("Grounded");
        private int _animIDJump = Animator.StringToHash("Jump");
        private int _animIDFreeFall = Animator.StringToHash("FreeFall");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        public override void Spawned()
        {
            _networkAnimator = GetComponent<NetworkMecanimAnimator>();
        }

        public void UpdateMovementAnimations(float speedBlend, float motionSpeed, float inputX, float inputZ)
        {
            _networkAnimator.Animator.SetFloat(_animIDSpeed, speedBlend);
            _networkAnimator.Animator.SetFloat(_animIDMotionSpeed, motionSpeed);
        }

        public void UpdateGrounded(bool grounded)
        {
            _networkAnimator.Animator.SetBool(_animIDGrounded, grounded);
        }

        public void SetJumping(bool jumping)
        {
            _networkAnimator.Animator.SetBool(_animIDJump, jumping);
        }

        public void SetFreeFall(bool freeFall)
        {
            _networkAnimator.Animator.SetBool(_animIDFreeFall, freeFall);
        }
    }
}