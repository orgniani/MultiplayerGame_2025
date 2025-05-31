using Managers;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text scoreAText;
        [SerializeField] private TMP_Text scoreBText;

        private TimerManager _timerManager;
        private ScoreManager _scoreManager;

        private IEnumerator Start()
        {
            while (_timerManager == null)
            {
                _timerManager = FindFirstObjectByType<TimerManager>();
                yield return null;
            }

            while (_scoreManager == null)
            {
                _scoreManager = FindFirstObjectByType<ScoreManager>();
                yield return null;
            }
        }

        private void FixedUpdate()
        {
            if (_timerManager)
            {
                int minutes = Mathf.FloorToInt(_timerManager.RemainingTime / 60f);
                int seconds = Mathf.FloorToInt(_timerManager.RemainingTime % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }

            if (_scoreManager)
            {
                scoreAText.text = $"Score A: {_scoreManager.GetScoreA()}";
                scoreBText.text = $"Score B: {_scoreManager.GetScoreB()}";
            }
        }
    }
}