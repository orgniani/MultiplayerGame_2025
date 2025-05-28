using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;

        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDGrounded = Animator.StringToHash("Grounded");
        private int _animIDJump = Animator.StringToHash("Jump");
        private int _animIDFreeFall = Animator.StringToHash("FreeFall");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
        }

        public void UpdateMovementAnimations(float speedBlend, float motionSpeed, bool strafeOnAim, float inputX, float inputZ)
        {
            _animator.SetFloat(_animIDSpeed, speedBlend);
            _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
        }

        public void UpdateGrounded(bool grounded) => _animator.SetBool(_animIDGrounded, grounded);
        public void SetJumping(bool jumping) => _animator.SetBool(_animIDJump, jumping);
        public void SetFreeFall(bool freeFall) => _animator.SetBool(_animIDFreeFall, freeFall);
    }
}