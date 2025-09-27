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
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI levelTitleText;
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

        [Header("Display Settings")]
        [SerializeField] private Color activeStarColor = Color.yellow;
        [SerializeField] private Color inactiveStarColor = Color.gray;

        [Header("Animation Settings")]
        [SerializeField] private float panelAnimationSpeed = 2f;
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
            // Гарантируем, что пауза снята при запуске
            _isGamePaused = false;
            Time.timeScale = 1f;
            
            InitializeUI();
            SetupEventListeners();
            
            // Гарантируем, что кнопки активны при запуске
            if (startButton != null)
                startButton.interactable = true;
                
            // Устанавливаем действия после перезапуска
            SetupGameplayActions();
        }

        private void InitializeUI()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);

            UpdateCommandCounter(0);
            UpdateStarsDisplay(0);
        }

        private void SetupEventListeners()
        {
            ClearEventListeners();

            if (startButton != null)
                startButton.onClick.AddListener(OnStartButtonClicked);
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetButtonClicked);
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseButtonClicked);
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuButtonClicked);

            if (winNextLevelButton != null)
                winNextLevelButton.onClick.AddListener(OnNextLevelClicked);
            if (winRetryButton != null)
                winRetryButton.onClick.AddListener(OnRetryButtonClicked);
            if (winMenuButton != null)
                winMenuButton.onClick.AddListener(OnMenuButtonClicked);

            if (loseRetryButton != null)
                loseRetryButton.onClick.AddListener(OnRetryButtonClicked);
            if (loseMenuButton != null)
                loseMenuButton.onClick.AddListener(OnMenuButtonClicked);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRetryButtonClicked);
            if (pauseMenuButton != null)
                pauseMenuButton.onClick.AddListener(OnMenuButtonClicked);

            var inputManager = InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.RegisterKeyAction(KeyCode.Space, OnStartButtonClicked);
                inputManager.RegisterKeyAction(KeyCode.R, OnResetButtonClicked);
                inputManager.RegisterKeyAction(KeyCode.Escape, OnPauseButtonClicked);
            }
        }

        private void ClearEventListeners()
        {
            if (startButton != null)
                startButton.onClick.RemoveAllListeners();
            if (resetButton != null)
                resetButton.onClick.RemoveAllListeners();
            if (pauseButton != null)
                pauseButton.onClick.RemoveAllListeners();
            if (menuButton != null)
                menuButton.onClick.RemoveAllListeners();

            if (winNextLevelButton != null) 
                winNextLevelButton.onClick.RemoveAllListeners();
            if (winRetryButton != null) 
                winRetryButton.onClick.RemoveAllListeners();
            if (winMenuButton != null) 
                winMenuButton.onClick.RemoveAllListeners();

            if (loseRetryButton != null) 
                loseRetryButton.onClick.RemoveAllListeners();
            if (loseMenuButton != null) 
                loseMenuButton.onClick.RemoveAllListeners();

            if (resumeButton != null) 
                resumeButton.onClick.RemoveAllListeners();
            if (restartButton != null) 
                restartButton.onClick.RemoveAllListeners();
            if (pauseMenuButton != null) 
                pauseMenuButton.onClick.RemoveAllListeners();

            var inputManager = InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.UnregisterKeyAction(KeyCode.Space);
                inputManager.UnregisterKeyAction(KeyCode.R);
                inputManager.UnregisterKeyAction(KeyCode.Escape);
            }
        }

        private void SetupGameplayActions()
        {
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                OnStartProgram = gameManager.StartProgram;
                OnResetProgram = gameManager.ResetProgram;
                OnPauseProgram = gameManager.PauseProgram;
                Debug.Log("Gameplay actions setup completed");
            }
            else
            {
                Debug.LogError("GameManager not found!");
            }
        }

        private void LoadLevelData()
        {
            string levelName = SceneManager.GetActiveScene().name;
            if (levelName != null && levelName.StartsWith("Level_"))
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
            if (levelTitleText != null)
            {
                string localizedTitle = LocalizationManager.Instance?.GetText("LEVEL") ?? "Уровень";
                levelTitleText.text = $"{localizedTitle} {_currentLevel}";
            }

            LevelData levelData = Resources.Load<LevelData>($"Levels/Level_{_currentLevel:D2}");
            if (levelData != null)
            {
                _maxCommands = levelData.maxCommands;
                UpdateMaxCommandsDisplay();
            }
        }

        private void OnStartButtonClicked()
        {
            Debug.Log($"OnStartButtonClicked: _isGameRunning={_isGameRunning}, _onStartProgram={(object)OnStartProgram}, _onPauseProgram={(object)OnPauseProgram}");
            
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
            
            // Получаем GameManager и повторно инициализируем действия
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.ReinitializeEventListeners();
                gameManager.ReinitializeGameplayActions();
            }
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SetGameRunning(bool running)
        {
            _isGameRunning = running;

            if (startButton != null)
            {
                TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    string key = running ? "STOP" : "START";
                    buttonText.text = LocalizationManager.Instance?.GetText(key) ?? (running ? "СТОП" : "СТАРТ");
                }
                
                startButton.interactable = true;
                startButton.gameObject.SetActive(true);
            }

            if (resetButton != null)
            {
                resetButton.interactable = !running;
            }
        }

        public void UpdateCommandCounter(int count)
        {
            _currentCommandCount = count;

            if (commandCounterText != null)
            {
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
        }

        private void UpdateMaxCommandsDisplay()
        {
            if (maxCommandsText != null)
            {
                maxCommandsText.text = $"/ {_maxCommands}";
            }
        }

        public void UpdateStarsDisplay(int stars)
        {
            if (starsDisplay != null)
            {
                for (int i = 0; i < starsDisplay.Length; i++)
                {
                    starsDisplay[i].color = i < stars ? activeStarColor : inactiveStarColor;
                }
            }
        }

        public void ShowWinPanel(int starsEarned)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
                UpdateStarsDisplay(starsEarned);
                StartCoroutine(AnimatePanel(winPanel, true));
            }

            LevelButton.SaveLevelProgress(_currentLevel, starsEarned);
            MainMenuManager.UnlockLevel(_currentLevel + 1);

            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowWinPanelDetailed(int starsEarned, int score, float timeSeconds)
        {
            // Store the last score and time for localization updates
            _lastScore = score;
            _lastTime = timeSeconds;
            
            if (winPanel != null)
            {
                winPanel.SetActive(true);

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

                StartCoroutine(AnimatePanel(winPanel, true));
            }

            LevelButton.SaveLevelProgress(_currentLevel, starsEarned);
            MainMenuManager.UnlockLevel(_currentLevel + 1);

            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowLosePanel()
        {
            if (losePanel != null)
            {
                losePanel.SetActive(true);
                
                if (loseTitleText != null)
                {
                    loseTitleText.text = LocalizationManager.Instance?.GetText("LOSE_TITLE") ?? "НЕУДАЧА";
                }
                
                if (loseMessageText != null)
                {
                    loseMessageText.text = LocalizationManager.Instance?.GetText("TRY_AGAIN") ?? "Попробуй еще раз!";
                }
                
                StartCoroutine(AnimatePanel(losePanel, true));
            }

            AudioManager.Instance?.PlaySound("fail");
        }

        public void ShowPausePanel(bool show)
        {
            if (show)
            {
                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                    StartCoroutine(AnimatePanel(pausePanel, true));
                }
                
                if (pauseTitleText != null)
                {
                    pauseTitleText.text = LocalizationManager.Instance?.GetText("PAUSE") ?? "ПАУЗА";
                }
                
                if (pauseButton != null)
                {
                    pauseButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (pausePanel != null)
                {
                    StartCoroutine(AnimatePanel(pausePanel, false));
                }
                
                if (pauseButton != null)
                {
                    pauseButton.gameObject.SetActive(true);
                }
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
            
            // Гарантируем, что кнопка старта активна
            if (startButton != null)
            {
                startButton.interactable = true;
                startButton.gameObject.SetActive(true);
            }
            
            // Также убедимся, что другие кнопки активны
            if (resetButton != null)
            {
                resetButton.interactable = true;
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

                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.Lerp(startScale, endScale, curveProgress);
                }
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curveProgress);

                yield return null;
            }

            if (rectTransform != null)
            {
                rectTransform.localScale = endScale;
            }
            
            canvasGroup.alpha = endAlpha;

            if (!show)
            {
                panel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // Отписываемся от события загрузки сцены
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            // Очищаем все слушатели событий при уничтожении объекта
            ClearEventListeners();
            Time.timeScale = 1f;
            
            // Отписываемся от событий GameManager
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                OnStartProgram -= gameManager.StartProgram;
                OnResetProgram -= gameManager.ResetProgram;
                OnPauseProgram -= gameManager.PauseProgram;
            }
        }

        private void OnDisable()
        {
            // Очищаем слушатели событий при отключении объекта
            ClearEventListeners();
        }
    }
}
