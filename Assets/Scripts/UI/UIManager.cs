using Managers;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text racePositionsText;
        [SerializeField] private TMP_Text winnersText;

        [Header("Game objects")]
        [SerializeField] private GameObject waitingForPlayersText;
        [SerializeField] private GameObject gameOverCanvas;

        [Header("Buttons")]
        [SerializeField] private Button menuButton;

        private TimerManager _timerManager;
        private RacePositionManager _racePositionManager;
        private GameOverManager _gameOverManager;

        private UITimer _uiTimer;
        private UIRacePositions _uiRacePositions;

        private void OnEnable()
        {
            menuButton.onClick.AddListener(ReturnToMainMenu);
            gameOverCanvas.SetActive(false);
            waitingForPlayersText.SetActive(true);
        }

        private IEnumerator Start()
        {
            while (_timerManager == null || _racePositionManager == null || _gameOverManager == null)
            {
                _timerManager ??= FindFirstObjectByType<TimerManager>();
                _racePositionManager ??= FindFirstObjectByType<RacePositionManager>();
                _gameOverManager ??= FindFirstObjectByType<GameOverManager>();
                yield return null;
            }

            _uiTimer = new UITimer(_timerManager, timerText);
            _uiRacePositions = new UIRacePositions(_racePositionManager, racePositionsText, winnersText);

            UpdateWaitingStatus();
        }

        private void Update()
        {
            if (_uiTimer == null || _uiRacePositions == null)
                return;

            _uiTimer.UpdateTimer();
            _uiRacePositions.UpdateRacePositions();
            _uiRacePositions.UpdateWinners();

            CheckGameOver();
        }

        private void UpdateWaitingStatus()
        {
            if (_timerManager == null) return;
            waitingForPlayersText.SetActive(false);
        }

        private void CheckGameOver()
        {
            if (_gameOverManager != null && _gameOverManager.IsGameOver && !gameOverCanvas.activeSelf)
            {
                Debug.Log("Client: Showing game over screen!");
                gameOverCanvas.SetActive(true);
            }
        }

        private void ReturnToMainMenu()
        {

        }
    }
}