using Fusion;
using System;
using UnityEngine;

namespace Managers
{
    public class TimerManager : NetworkBehaviour
    {
        //TODO: Do not hardcode time
        [Networked] public float RemainingTime { get; set; } = 120f;
        [Networked] public bool TimerRunning { get; private set; } = false;


        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                TimerRunning = true;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || !TimerRunning) return;

            RemainingTime -= Runner.DeltaTime;
            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;
                TimerRunning = false;
                Debug.Log("Time ended!");
            }
        }
    }
}