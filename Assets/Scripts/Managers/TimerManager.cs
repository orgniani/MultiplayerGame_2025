using Fusion;
using UnityEngine;

namespace Managers
{
    public class TimerManager : NetworkBehaviour
    {
        [Networked] private float _remainingTime { get; set; } = 120f;
        private bool _timerRunning = false;

        public float GetRemainingTime() => _remainingTime;

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                _timerRunning = true;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || !_timerRunning) return;

            _remainingTime -= Runner.DeltaTime;
            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                _timerRunning = false;
                Debug.Log("¡Tiempo terminado!");
            }
        }
    }
}