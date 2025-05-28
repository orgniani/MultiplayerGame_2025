using UnityEngine;

namespace Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;

        [SerializeField][Range(0, 1)] private float footstepAudioVolume = 0.5f;

        private CharacterController _controller;
        public void Initialize(CharacterController controller)
        {
            _controller = controller;
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_controller.center), footstepAudioVolume);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && landingAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_controller.center), footstepAudioVolume);
            }
        }
    }
}