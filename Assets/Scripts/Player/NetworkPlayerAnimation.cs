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

        private void Awake()
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


        // Example of a networked trigger for taunts
        private const string TauntTriggerName = "Taunt";

        void Update()
        {
            if (!Object.HasInputAuthority)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                Rpc_PlayTaunt(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                Rpc_PlayTaunt(2);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_PlayTaunt(int tauntNumber)
        {
            Rpc_RelayTaunt(tauntNumber);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        private void Rpc_RelayTaunt(int tauntNumber)
        {
            _networkAnimator.SetTrigger($"{TauntTriggerName} {tauntNumber}", passThroughOnInputAuthority: true);
        }

    }
}