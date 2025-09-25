using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Core;
using UI;

namespace RobotCoder.UI
{
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject palettePanel;
        [SerializeField] private GameObject workspacePanel;
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject progressPanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject pausePanel;

        [SerializeField] private Button startButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button menuButton;

        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private TextMeshProUGUI commandCounterText;
        [SerializeField] private TextMeshProUGUI maxCommandsText;
        [SerializeField] private Image[] starsDisplay;

        [SerializeField] private TextMeshProUGUI winTitleText;
        [SerializeField] private Transform starsContainer;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Button winNextLevelButton;
        [SerializeField] private Button winRetryButton;
        [SerializeField] private Button winMenuButton;

        [SerializeField] private TextMeshProUGUI loseTitleText;
        [SerializeField] private TextMeshProUGUI loseMessageText;
        [SerializeField] private Button loseRetryButton;
        [SerializeField] private Button loseMenuButton;

        [SerializeField] private TextMeshProUGUI pauseTitleText;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button pauseMenuButton;

        [SerializeField] private Color activeStarColor = Color.yellow;
        [SerializeField] private Color inactiveStarColor = Color.gray;

        [SerializeField] private float panelAnimationSpeed = 2f;
        [SerializeField] private AnimationCurve panelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private int _currentCommandCount = 0;
        private int _maxCommands = 10;
        private int _currentLevel = 1;
        private bool _isGamePaused = false;
        private bool _isGameRunning = false;

        public System.Action OnStartProgram;
        public System.Action OnResetProgram;
        public System.Action OnPauseProgram;
        public System.Action<float> OnSpeedChanged;

        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
            LoadLevelData();
        }

        private void InitializeUI()
        {
            winPanel.SetActive(false);
            losePanel.SetActive(false);
            pausePanel.SetActive(false);

            UpdateCommandCounter(0);
            UpdateStarsDisplay(0);
        }

        private void SetupEventListeners()
        {
            ClearEventListeners();

            startButton.onClick.AddListener(OnStartButtonClicked);
            resetButton.onClick.AddListener(OnResetButtonClicked);
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
            menuButton.onClick.AddListener(OnMenuButtonClicked);

            winNextLevelButton.onClick.AddListener(OnNextLevelClicked);
            winRetryButton.onClick.AddListener(OnRetryButtonClicked);
            winMenuButton.onClick.AddListener(OnMenuButtonClicked);

            loseRetryButton.onClick.AddListener(OnRetryButtonClicked);
            loseMenuButton.onClick.AddListener(OnMenuButtonClicked);

            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            restartButton.onClick.AddListener(OnRetryButtonClicked);
            pauseMenuButton.onClick.AddListener(OnMenuButtonClicked);

            var inputManager = InputManager.Instance;

            inputManager.RegisterKeyAction(KeyCode.Space, OnStartButtonClicked);
            inputManager.RegisterKeyAction(KeyCode.R, OnResetButtonClicked);
            inputManager.RegisterKeyAction(KeyCode.Escape, OnPauseButtonClicked);
        }

        private void ClearEventListeners()
        {
            startButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();
            pauseButton.onClick.RemoveAllListeners();
            menuButton.onClick.RemoveAllListeners();

            if (winNextLevelButton != null) winNextLevelButton.onClick.RemoveAllListeners();
            if (winRetryButton != null) winRetryButton.onClick.RemoveAllListeners();
            if (winMenuButton != null) winMenuButton.onClick.RemoveAllListeners();

            if (loseRetryButton != null) loseRetryButton.onClick.RemoveAllListeners();
            if (loseMenuButton != null) loseMenuButton.onClick.RemoveAllListeners();

            if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
            if (restartButton != null) restartButton.onClick.RemoveAllListeners();
            if (pauseMenuButton != null) pauseMenuButton.onClick.RemoveAllListeners();

            var inputManager = InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.UnregisterKeyAction(KeyCode.Space);
                inputManager.UnregisterKeyAction(KeyCode.R);
                inputManager.UnregisterKeyAction(KeyCode.Escape);
            }
        }

        private void LoadLevelData()
        {
            string levelName = SceneManager.GetActiveScene().name;
            if (levelName.StartsWith("Level_"))
            {
                string levelNumberStr = levelName.Substring(6);
                if (int.TryParse(levelNumberStr, out _currentLevel))
                {
                    UpdateLevelInfo();
                }
            }
        }

        private void UpdateLevelInfo()
        {
            string localizedTitle = LocalizationManager.Instance?.GetText("LEVEL") ?? "Уровень";
            levelTitleText.text = $"{localizedTitle} {_currentLevel}";

            LevelData levelData = Resources.Load<LevelData>($"Levels/Level_{_currentLevel:D2}");
            if (levelData != null)
            {
                _maxCommands = levelData.maxCommands;
                UpdateMaxCommandsDisplay();
            }
        }

        private void OnStartButtonClicked()
        {
            if (_isGameRunning)
            {
                OnPauseProgram?.Invoke();
                SetGameRunning(false);
            }
            else
            {
                OnStartProgram?.Invoke();
                SetGameRunning(true);
            }

            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnResetButtonClicked()
        {
            OnResetProgram?.Invoke();
            SetGameRunning(false);
            UpdateCommandCounter(0);
            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnPauseButtonClicked()
        {
            _isGamePaused = true;
            ShowPausePanel(true);

            OnPauseProgram?.Invoke();
            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnResumeButtonClicked()
        {
            _isGamePaused = false;
            ShowPausePanel(false);
            if (_isGameRunning)
            {
                OnStartProgram?.Invoke();
            }

            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnMenuButtonClicked()
        {
            ResumeGame();

            AudioManager.Instance?.PlaySound("button_click");
            SceneManager.LoadScene($"MainMenu");
        }

        private void OnNextLevelClicked()
        {
            AudioManager.Instance?.PlaySound("button_click");

            int nextLevel = _currentLevel + 1;
            string nextSceneName = $"Level_{nextLevel:D2}";

            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                ResumeGame();
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                ResumeGame();
                SceneManager.LoadScene($"MainMenu");
            }
        }

        private void OnRetryButtonClicked()
        {
            AudioManager.Instance?.PlaySound("button_click");
            ResumeGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SetGameRunning(bool running)
        {
            _isGameRunning = running;

            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            string key = running ? "STOP" : "START";
            buttonText.text = LocalizationManager.Instance?.GetText(key) ?? (running ? "СТОП" : "СТАРТ");

            resetButton.interactable = !running;
        }

        public void UpdateCommandCounter(int count)
        {
            _currentCommandCount = count;

            commandCounterText.text = $"{_currentCommandCount}";

            if (_currentCommandCount > _maxCommands)
            {
                commandCounterText.color = Color.red;
            }
            else if (_currentCommandCount == _maxCommands)
            {
                commandCounterText.color = Color.yellow;
            }
            else
            {
                commandCounterText.color = Color.white;
            }
        }

        private void UpdateMaxCommandsDisplay()
        {
            maxCommandsText.text = $"/ {_maxCommands}";
        }

        public void UpdateStarsDisplay(int stars)
        {
            for (int i = 0; i < starsDisplay.Length; i++)
            {
                starsDisplay[i].color = i < stars ? activeStarColor : inactiveStarColor;
            }
        }

        public void ShowWinPanel(int starsEarned)
        {
            winPanel.SetActive(true);
            UpdateStarsDisplay(starsEarned);
            StartCoroutine(AnimatePanel(winPanel, true));

            LevelButton.SaveLevelProgress(_currentLevel, starsEarned);
            MainMenuManager.UnlockLevel(_currentLevel + 1);

            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowWinPanelDetailed(int starsEarned, int score, float timeSeconds)
        {
            winPanel.SetActive(true);

            winTitleText.text = LocalizationManager.Instance?.GetText("WIN_TITLE") ?? "ПОБЕДА!";

            if (starsContainer)
            {
                var images = starsContainer.GetComponentsInChildren<Image>(true);
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = i < starsEarned ? activeStarColor : inactiveStarColor;
                }
            }
            else
            {
                UpdateStarsDisplay(starsEarned);
            }

            string scoreLabel = LocalizationManager.Instance?.GetText("SCORE") ?? "Очки";
            scoreText.text = $"{scoreLabel}: {score}";

            string timeLabel = LocalizationManager.Instance?.GetText("TIME") ?? "Время";
            timeText.text = $"{timeLabel}: {FormatTime(timeSeconds)}";

            StartCoroutine(AnimatePanel(winPanel, true));

            LevelButton.SaveLevelProgress(_currentLevel, starsEarned);
            MainMenuManager.UnlockLevel(_currentLevel + 1);

            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowLosePanel()
        {
            losePanel.SetActive(true);
            loseTitleText.text = LocalizationManager.Instance?.GetText("LOSE_TITLE") ?? "НЕУДАЧА";
            loseMessageText.text = LocalizationManager.Instance?.GetText("TRY_AGAIN") ?? "Попробуй еще раз!";
            StartCoroutine(AnimatePanel(losePanel, true));

            AudioManager.Instance?.PlaySound("fail");
        }

        public void ShowPausePanel(bool show)
        {
            if (show)
            {
                pausePanel.SetActive(true);
                StartCoroutine(AnimatePanel(pausePanel, true));
                pauseTitleText.text = LocalizationManager.Instance?.GetText("PAUSE") ?? "ПАУЗА";
                pauseButton.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(AnimatePanel(pausePanel, false));
                pauseButton.gameObject.SetActive(true);
            }

            _isGamePaused = show;
            Time.timeScale = show ? 0f : 1f;
        }

        // Новый метод для снятия паузы
        private void ResumeGame()
        {
            _isGamePaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }

            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(true);
            }
        }

        private static string FormatTime(float seconds)
        {
            if (seconds < 0) return "--:--";
            int s = Mathf.RoundToInt(seconds);
            int m = s / 60;
            int r = s % 60;
            return $"{m:00}:{r:00}";
        }

        private IEnumerator AnimatePanel(GameObject panel, bool show)
        {
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>() ?? panel.AddComponent<CanvasGroup>();

            float duration = 1f / panelAnimationSpeed;
            float startTime = Time.unscaledTime;

            Vector3 startScale = show ? Vector3.zero : Vector3.one;
            Vector3 endScale = show ? Vector3.one : Vector3.zero;
            float startAlpha = show ? 0f : 1f;
            float endAlpha = show ? 1f : 0f;

            while (Time.unscaledTime - startTime < duration)
            {
                float progress = (Time.unscaledTime - startTime) / duration;
                float curveProgress = panelCurve.Evaluate(progress);

                rectTransform.localScale = Vector3.Lerp(startScale, endScale, curveProgress);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curveProgress);

                yield return null;
            }

            rectTransform.localScale = endScale;
            canvasGroup.alpha = endAlpha;

            if (!show)
            {
                panel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // Очищаем все слушатели событий при уничтожении объекта
            ClearEventListeners();
            Time.timeScale = 1f;
        }

        private void OnDisable()
        {
            // Очищаем слушатели событий при отключении объекта
            ClearEventListeners();
        }
    }
}