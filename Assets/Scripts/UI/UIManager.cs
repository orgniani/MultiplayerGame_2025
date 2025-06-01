using Fusion;
using Managers;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text racePositionsText;
        [SerializeField] private TMP_Text winnersText;

        private TimerManager _timerManager;
        private RacePositionManager _racePositionManager;

        private IEnumerator Start()
        {
            while (_timerManager == null || _racePositionManager == null)
            {
                _timerManager ??= FindFirstObjectByType<TimerManager>();
                _racePositionManager ??= FindFirstObjectByType<RacePositionManager>();
                yield return null;
            }
        }

        private void FixedUpdate()
        {
            UpdateTimer();
            UpdateRacePositions();
            UpdateWinners();
        }

        private void UpdateTimer()
        {
            if (_timerManager == null) return;

            int minutes = Mathf.FloorToInt(_timerManager.RemainingTime / 60f);
            int seconds = Mathf.FloorToInt(_timerManager.RemainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private void UpdateRacePositions()
        {
            if (_racePositionManager == null) return;

            var playerOrder = _racePositionManager.GetCurrentPlayerOrder();
            if (playerOrder.Count == 0) return;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < playerOrder.Count; i++)
            {
                string playerName = GetPlayerName(playerOrder[i]);
                sb.AppendLine($"{i + 1} {playerName}");
            }

            racePositionsText.text = sb.ToString();
        }

        private void UpdateWinners()
        {
            if (_racePositionManager == null) return;

            var winners = _racePositionManager.GetWinnersOrder();
            if (winners.Count == 0)
            {
                winnersText.text = "No winners yet!";
                return;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < winners.Count; i++)
            {
                string playerName = GetPlayerName(winners[i]);
                sb.AppendLine($"{i + 1} {playerName}");
            }

            winnersText.text = sb.ToString();
        }

        private string GetPlayerName(PlayerRef player)
        {
            return "Player_" + player.PlayerId;
        }
    }
}