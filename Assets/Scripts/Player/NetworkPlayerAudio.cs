using UnityEngine;
using Fusion;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class NetworkPlayerAudio : NetworkBehaviour
    {
        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;

        [SerializeField][Range(0, 1)] private float footstepAudioVolume = 0.5f;

        private CharacterController _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && footstepAudioClips.Length > 0)
            {
                int index = Random.Range(0, footstepAudioClips.Length);
                RpcPlayFootstep(index);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && landingAudioClip != null)
            {
                RpcPlayLanding();
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RpcPlayFootstep(int clipIndex)
        {
            if (clipIndex < 0 || clipIndex >= footstepAudioClips.Length) return;
            AudioSource.PlayClipAtPoint(footstepAudioClips[clipIndex], transform.TransformPoint(_controller.center), footstepAudioVolume);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RpcPlayLanding()
        {
            if (landingAudioClip == null) return;
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_controller.center), footstepAudioVolume);
        }
    }

}