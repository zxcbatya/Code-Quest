using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Core;
using UI;
using UnityEngine.SceneManagement;
using InputManager = Core.InputManager;

namespace RobotCoder.UI
{
    public class GameplayUIManager : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private TextMeshProUGUI levelTitleText;

        [SerializeField] private TextMeshProUGUI commandCounterText;
        [SerializeField] private TextMeshProUGUI maxCommandsText;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private TextMeshProUGUI winTitleText;
        [SerializeField] private Transform starsContainer;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI loseTitleText;
        [SerializeField] private TextMeshProUGUI loseMessageText;
        [SerializeField] private TextMeshProUGUI pauseTitleText;
        [SerializeField] private Button winNextLevelButton;
        [SerializeField] private Button winRetryButton;
        [SerializeField] private Button winMenuButton;
        [SerializeField] private Button loseRetryButton;
        [SerializeField] private Button loseMenuButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button pauseMenuButton;
        [SerializeField] private Image[] starsDisplay;
        [SerializeField] private WorkspacePanel workspacePanel;

        // Add serialized references to all the panels that need to be managed
        [SerializeField] private GameObject progressPanel;
        [SerializeField] private GameObject palettePanel;
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject hintPanel;

        [Header("Display Settings")] [SerializeField]
        private Color activeStarColor = Color.yellow;

        [SerializeField] private Color inactiveStarColor = Color.gray;

        [Header("Animation Settings")] [SerializeField]
        private float panelAnimationSpeed = 2f;

        [SerializeField] private AnimationCurve panelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] private int _currentCommandCount = 0;
        private int _maxCommands = 10;
        private int _currentLevel = 1;
        [SerializeField] private bool _isGamePaused = false;
        private bool _isGameRunning = false;
        private int _lastScore = 0;
        private float _lastTime = 0f;

        public System.Action OnStartProgram;
        public System.Action OnResetProgram;
        public System.Action OnPauseProgram;
        public System.Action<float> OnSpeedChanged;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded, reinitializing UI");
            InitializeUI();
            SetupEventListeners();
            SetupGameplayActions();

            if (startButton != null)
                startButton.interactable = true;
        }

        private void Start()
        {
            _isGamePaused = false;
            Time.timeScale = 1f;

            InitializeUI();
            SetupEventListeners();

            if (startButton != null)
                startButton.interactable = true;

            SetupGameplayActions();
        }

        private void InitializeUI()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);

            UpdateCommandCounter(0);
            UpdateStarsDisplay(0);

            // Show all game panels at start
            ShowGamePanels();
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
        }

        private void ClearEventListeners()
        {
            startButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();
            pauseButton.onClick.RemoveAllListeners();
            menuButton.onClick.RemoveAllListeners();

            winNextLevelButton.onClick.RemoveAllListeners();
            winRetryButton.onClick.RemoveAllListeners();
            winMenuButton.onClick.RemoveAllListeners();

            loseRetryButton.onClick.RemoveAllListeners();
            loseMenuButton.onClick.RemoveAllListeners();

            resumeButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            pauseMenuButton.onClick.RemoveAllListeners();
        }

        private void SetupGameplayActions()
        {
            var gameManager = GameManager.Instance;
            OnStartProgram = gameManager.StartProgram;
            OnResetProgram = gameManager.ResetProgram;
            OnPauseProgram = gameManager.PauseProgram;
            Debug.Log("Gameplay actions setup completed");
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
            Debug.Log(
                $"OnStartButtonClicked: _isGameRunning={_isGameRunning}, _onStartProgram={(object)OnStartProgram}, _onPauseProgram={(object)OnPauseProgram}");

            if (_isGameRunning)
            {
                OnPauseProgram.Invoke();
                SetGameRunning(false);
            }
            else
            {
                OnStartProgram.Invoke();
                SetGameRunning(true);
            }

            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnResetButtonClicked()
        {
            OnResetProgram.Invoke();
            SetGameRunning(false);
            UpdateCommandCounter(0);
            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnPauseButtonClicked()
        {
            _isGamePaused = true;
            ShowPausePanel(true);

            OnPauseProgram.Invoke();
            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnResumeButtonClicked()
        {
            _isGamePaused = false;
            ShowPausePanel(false);
            if (_isGameRunning)
            {
                OnStartProgram.Invoke();
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

            // Instead of reloading the scene, use the GameManager's reset functionality
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.ResetProgram();
                // Reinitialize event listeners and gameplay actions
                gameManager.ReinitializeEventListeners();
                gameManager.ReinitializeGameplayActions();
            }

            // Reset the UI state
            SetGameRunning(false);
            UpdateCommandCounter(0);

            // Reset progress panel if it exists
            if (progressPanel != null)
            {
                var progressPanelComponent = progressPanel.GetComponent<ProgressPanel>();
                if (progressPanelComponent != null)
                {
                    progressPanelComponent.ResetProgress();
                }
            }

            // Ensure all game panels are visible after restart
            ShowGamePanels();
        }

        private void ShowGamePanels()
        {
            // Show all essential game panels after restart
            if (progressPanel != null) progressPanel.SetActive(true);
            if (palettePanel != null) palettePanel.SetActive(true);
            if (controlPanel != null) controlPanel.SetActive(true);
            if (workspacePanel != null) workspacePanel.gameObject.SetActive(true);
            if (hintPanel != null) hintPanel.SetActive(false); // Hint panel should be hidden by default

            // Hide win/lose/pause panels
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);

            // Show pause button
            if (pauseButton != null) pauseButton.gameObject.SetActive(true);
        }

        public void SetGameRunning(bool running)
        {
            _isGameRunning = running;

            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            string key = running ? "STOP" : "START";
            buttonText.text = LocalizationManager.Instance?.GetText(key) ?? (running ? "СТОП" : "СТАРТ");

            startButton.interactable = true;
            startButton.gameObject.SetActive(true);

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
            // Hide all other panels and show only win panel
            if (losePanel != null) losePanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            // Hide all game panels
            if (progressPanel != null) progressPanel.SetActive(false);
            if (palettePanel != null) palettePanel.SetActive(false);
            if (controlPanel != null) controlPanel.SetActive(false);
            if (workspacePanel != null) workspacePanel.gameObject.SetActive(false);
            if (hintPanel != null) hintPanel.SetActive(false);

            if (winPanel != null) winPanel.SetActive(true);

            UpdateStarsDisplay(starsEarned);
            if (winPanel != null) StartCoroutine(AnimatePanel(winPanel, true));

            LevelButton.SaveLevelProgress(_currentLevel, starsEarned);
            MainMenuManager.UnlockLevel(_currentLevel + 1);

            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowWinPanelDetailed(int starsEarned, int score, float timeSeconds)
        {
            // Store the last score and time for localization updates
            _lastScore = score;
            _lastTime = timeSeconds;

            // Hide all other panels and show only win panel
            if (losePanel != null) losePanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            // Hide all game panels
            if (progressPanel != null) progressPanel.SetActive(false);
            if (palettePanel != null) palettePanel.SetActive(false);
            if (controlPanel != null) controlPanel.SetActive(false);
            if (workspacePanel != null) workspacePanel.gameObject.SetActive(false);
            if (hintPanel != null) hintPanel.SetActive(false);

            if (winPanel != null) winPanel.SetActive(true);

            if (winTitleText != null)
            {
                winTitleText.text = LocalizationManager.Instance?.GetText("WIN_TITLE") ?? "ПОБЕДА!";
            }

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

            if (scoreText != null)
            {
                string scoreLabel = LocalizationManager.Instance?.GetText("SCORE") ?? "Очки";
                scoreText.text = $"{scoreLabel}: {score}";
            }

            if (timeText != null)
            {
                string timeLabel = LocalizationManager.Instance?.GetText("TIME") ?? "Время";
                timeText.text = $"{timeLabel}: {FormatTime(timeSeconds)}";
            }

            if (winPanel != null) StartCoroutine(AnimatePanel(winPanel, true));

            LevelButton.SaveLevelProgress(_currentLevel, starsEarned);
            MainMenuManager.UnlockLevel(_currentLevel + 1);

            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowLosePanel()
        {
            // Hide all other panels and show only lose panel
            if (winPanel != null) winPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            // Hide all game panels
            if (progressPanel != null) progressPanel.SetActive(false);
            if (palettePanel != null) palettePanel.SetActive(false);
            if (controlPanel != null) controlPanel.SetActive(false);
            if (workspacePanel != null) workspacePanel.gameObject.SetActive(false);
            if (hintPanel != null) hintPanel.SetActive(false);

            if (losePanel != null) losePanel.SetActive(true);

            if (loseTitleText != null)
            {
                loseTitleText.text = LocalizationManager.Instance?.GetText("LOSE_TITLE") ?? "НЕУДАЧА";
            }

            if (loseMessageText != null)
            {
                loseMessageText.text = LocalizationManager.Instance?.GetText("TRY_AGAIN") ?? "Попробуй еще раз!";
            }

            if (losePanel != null) StartCoroutine(AnimatePanel(losePanel, true));

            AudioManager.Instance?.PlaySound("fail");
        }

        public void ShowPausePanel(bool show)
        {
            if (show)
            {
                // Hide all other panels and show only pause panel
                if (winPanel != null) winPanel.SetActive(false);
                if (losePanel != null) losePanel.SetActive(false);
                if (pauseButton != null) pauseButton.gameObject.SetActive(false);

                // Hide all game panels
                if (progressPanel != null) progressPanel.SetActive(false);
                if (palettePanel != null) palettePanel.SetActive(false);
                if (controlPanel != null) controlPanel.SetActive(false);
                if (workspacePanel != null) workspacePanel.gameObject.SetActive(false);
                if (hintPanel != null) hintPanel.SetActive(false);

                if (pausePanel != null) pausePanel.SetActive(true);

                if (pauseTitleText != null)
                {
                    pauseTitleText.text = LocalizationManager.Instance?.GetText("PAUSE") ?? "ПАУЗА";
                }
            }
            else
            {
                if (pausePanel != null) StartCoroutine(AnimatePanel(pausePanel, false));
                if (pauseButton != null) pauseButton.gameObject.SetActive(true);

                // Show all game panels
                if (progressPanel != null) progressPanel.SetActive(true);
                if (palettePanel != null) palettePanel.SetActive(true);
                if (controlPanel != null) controlPanel.SetActive(true);
                if (workspacePanel != null) workspacePanel.gameObject.SetActive(true);
            }

            _isGamePaused = show;
            Time.timeScale = show ? 0f : 1f;
        }

        private void ResumeGame()
        {
            _isGamePaused = false;
            Time.timeScale = 1f;

            if (pausePanel != null) pausePanel.SetActive(false);
            if (pauseButton != null) pauseButton.gameObject.SetActive(true);

            // Hide win and lose panels
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);

            // Show all essential game panels
            if (progressPanel != null) progressPanel.SetActive(true);
            if (palettePanel != null) palettePanel.SetActive(true);
            if (controlPanel != null) controlPanel.SetActive(true);
            if (workspacePanel != null) workspacePanel.gameObject.SetActive(true);
            if (hintPanel != null) hintPanel.SetActive(false); // Hint panel should be hidden by default
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
            if (panel == null) yield break;

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
            SceneManager.sceneLoaded -= OnSceneLoaded;

            ClearEventListeners();
            Time.timeScale = 1f;

            var gameManager = GameManager.Instance;
            OnStartProgram -= gameManager.StartProgram;
            OnResetProgram -= gameManager.ResetProgram;
            OnPauseProgram -= gameManager.PauseProgram;
        }
    }
} 

