using Fusion;
using UnityEngine;

namespace Managers
{
    public class ScoreManager : NetworkBehaviour
    {
        [Networked] private int _scoreA { get; set; }
        [Networked] private int _scoreB { get; set; }

        public int GetScoreA() => _scoreA;
        public int GetScoreB() => _scoreB;

        public void AddScoreToA(int amount)
        {
            if (!HasStateAuthority) return;
            _scoreA += amount;
            Debug.Log("Score A: " + _scoreA);
        }

        public void AddScoreToB(int amount)
        {
            if (!HasStateAuthority) return;
            _scoreB += amount;
            Debug.Log("Score B: " + _scoreB);
        }
    }
}
