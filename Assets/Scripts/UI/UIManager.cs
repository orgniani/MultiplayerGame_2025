using Fusion;
using Managers;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Helpers;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text racePositionsText;
        [SerializeField] private TMP_Text winnersText;
        [SerializeField] private TMP_Text finalWinnersText;
        [SerializeField] private TMP_Text countdownText;


        [Header("Game objects")]
        [SerializeField] private GameObject waitingForPlayersPanel;
        [SerializeField] private GameObject countdownCanvas;
        [SerializeField] private GameObject gameOverCanvas;

        [Header("Buttons")]
        [SerializeField] private Button menuButton;

        [Header("Main menu build index")]
        [SerializeField] private int mainMenuBuildIndex = 0;

        private TimerManager _timerManager;
        private RacePositionManager _racePositionManager;
        private GameOverManager _gameOverManager;

        private UITimer _uiTimer;
        private UIRacePositions _uiRacePositions;

        private void OnEnable()
        {
            menuButton.onClick.AddListener(ReturnToMainMenu);
            gameOverCanvas.SetActive(false);
            waitingForPlayersPanel.SetActive(true);
            countdownCanvas.SetActive(false);

            CursorLocker.Lock();
        }

        private IEnumerator Start()
        {
            while (_timerManager == null || !_timerManager.HasStateAuthority)
            {
                _timerManager ??= FindFirstObjectByType<TimerManager>();
                yield return null;
            }

            while (_racePositionManager == null)
            {
                _racePositionManager = FindFirstObjectByType<RacePositionManager>();
                yield return null;
            }

            while (_gameOverManager == null)
            {
                _gameOverManager = FindFirstObjectByType<GameOverManager>();
                yield return null;
            }


            _uiTimer = new UITimer(_timerManager, timerText);
            _uiRacePositions = new UIRacePositions(_racePositionManager, racePositionsText, winnersText);

            StartCountdownVisual();
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
            waitingForPlayersPanel.SetActive(false);
        }

        private void CheckGameOver()
        {
            if(_gameOverManager == null || !_gameOverManager.HasStateAuthority)
                return;

            if (_gameOverManager.IsGameOver && !gameOverCanvas.activeSelf)
            {
                Debug.Log("Client: Showing game over screen!");

                CursorLocker.Unlock();
                gameOverCanvas.SetActive(true);
                finalWinnersText.text = "WINNERS\n" + winnersText.text;
            }
        }

        private void ReturnToMainMenu()
        {
            var runner = FindFirstObjectByType<NetworkRunner>();
            if (runner != null)
                runner.Shutdown();

            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuBuildIndex);
        }

        private void StartCountdownVisual()
        {
            countdownCanvas.SetActive(true);
            StartCoroutine(CountdownRoutine());
        }

        private IEnumerator CountdownRoutine()
        {
            countdownText.gameObject.SetActive(true);

            countdownText.text = "3";
            yield return new WaitForSeconds(1f);

            countdownText.text = "2";
            yield return new WaitForSeconds(1f);

            countdownText.text = "1";
            yield return new WaitForSeconds(1f);

            countdownCanvas.gameObject.SetActive(false);
        }

    }
}