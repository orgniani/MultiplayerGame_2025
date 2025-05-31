using Fusion;
using UnityEngine;

namespace Managers
{
    public class TimerManager : NetworkBehaviour
    {
        //TODO: Do not hardcode time
        [Networked] public float RemainingTime { get; set; } = 120f;
        private bool _timerRunning = false;

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

            RemainingTime -= Runner.DeltaTime;
            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;
                _timerRunning = false;
                Debug.Log("¡Tiempo terminado!");
            }
        }
    }
}