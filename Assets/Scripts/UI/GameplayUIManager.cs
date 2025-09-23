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
        [Header("UI Панели")]
        [SerializeField] private GameObject palettePanel;
        [SerializeField] private GameObject workspacePanel;
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject progressPanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Кнопки управления")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button menuButton;

        [Header("Информационные элементы")]
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private TextMeshProUGUI commandCounterText;
        [SerializeField] private TextMeshProUGUI maxCommandsText;
        [SerializeField] private Image[] starsDisplay;      // для компактного отображения, используется и в WinPanel
        [SerializeField] private Slider speedSlider;

        [Header("Win Panel References")] // соответствуют структуре из ТЗ
        [SerializeField] private TextMeshProUGUI winTitleText;      // WinTitle
        [SerializeField] private Transform starsContainer;          // StarsContainer (родитель для Star1..3)
        [SerializeField] private TextMeshProUGUI scoreText;         // ScoreText
        [SerializeField] private TextMeshProUGUI timeText;          // TimeText
        [SerializeField] private Button winNextLevelButton;         // NextLevelButton
        [SerializeField] private Button winRetryButton;             // RetryButton
        [SerializeField] private Button winMenuButton;              // MenuButton

        [Header("Lose Panel References")] // соответствует LosePanel из ТЗ
        [SerializeField] private TextMeshProUGUI loseTitleText;     // LoseTitle
        [SerializeField] private TextMeshProUGUI loseMessageText;   // LoseMessage
        [SerializeField] private Button loseRetryButton;            // RetryButton
        [SerializeField] private Button loseMenuButton;             // MenuButton

        [Header("Pause Panel References")] // соответствует PausePanel из ТЗ
        [SerializeField] private TextMeshProUGUI pauseTitleText;    // PauseTitle
        [SerializeField] private Button resumeButton;               // ResumeButton
        [SerializeField] private Button restartButton;              // RestartButton
        [SerializeField] private Button pauseMenuButton;            // MenuButton (внутри PausePanel)

        [Header("Цвета звезд")]
        [SerializeField] private Color activeStarColor = Color.yellow;
        [SerializeField] private Color inactiveStarColor = Color.gray;

        [Header("Анимации")]
        [SerializeField] private float panelAnimationSpeed = 2f;
        [SerializeField] private AnimationCurve panelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private int currentCommandCount = 0;
        private int maxCommands = 10;
        private int currentLevel = 1;
        private bool isGamePaused = false;
        private bool isGameRunning = false;

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
            if (winPanel) winPanel.SetActive(false);
            if (losePanel) losePanel.SetActive(false);
            if (pausePanel) pausePanel.SetActive(false);

            UpdateCommandCounter(0);
            UpdateStarsDisplay(0);
            
            if (speedSlider != null)
            {
                speedSlider.value = 1f;
                speedSlider.minValue = 0.5f;
                speedSlider.maxValue = 3f;
            }
        }

        private void SetupEventListeners()
        {
            if (startButton) startButton.onClick.AddListener(OnStartButtonClicked);
            if (resetButton) resetButton.onClick.AddListener(OnResetButtonClicked);
            if (pauseButton) pauseButton.onClick.AddListener(OnPauseButtonClicked);
            if (menuButton) menuButton.onClick.AddListener(OnMenuButtonClicked);

            if (winNextLevelButton) winNextLevelButton.onClick.AddListener(OnNextLevelClicked);
            if (winRetryButton) winRetryButton.onClick.AddListener(OnRetryButtonClicked);
            if (winMenuButton) winMenuButton.onClick.AddListener(OnMenuButtonClicked);

            if (loseRetryButton) loseRetryButton.onClick.AddListener(OnRetryButtonClicked);
            if (loseMenuButton) loseMenuButton.onClick.AddListener(OnMenuButtonClicked);

            if (resumeButton) resumeButton.onClick.AddListener(() => ShowPausePanel(false));
            if (restartButton) restartButton.onClick.AddListener(OnRetryButtonClicked);
            if (pauseMenuButton) pauseMenuButton.onClick.AddListener(OnMenuButtonClicked);

            if (speedSlider) speedSlider.onValueChanged.AddListener(OnSpeedSliderChanged);

            InputManager.Instance?.RegisterKeyAction(KeyCode.Space, OnStartButtonClicked);
            InputManager.Instance?.RegisterKeyAction(KeyCode.R, OnResetButtonClicked);
            InputManager.Instance?.RegisterKeyAction(KeyCode.Escape, OnPauseButtonClicked);
        }

        private void LoadLevelData()
        {
            string levelName = SceneManager.GetActiveScene().name;
            if (levelName.StartsWith("Level_"))
            {
                string levelNumberStr = levelName.Substring(6);
                if (int.TryParse(levelNumberStr, out currentLevel))
                {
                    UpdateLevelInfo();
                }
            }
        }

        private void UpdateLevelInfo()
        {
            if (levelTitleText != null)
            {
                string localizedTitle = RobotCoder.UI.LocalizationManager.Instance?.GetText("LEVEL") ?? "Уровень";
                levelTitleText.text = $"{localizedTitle} {currentLevel}";
            }

            LevelData levelData = Resources.Load<LevelData>($"Levels/Level_{currentLevel:D2}");
            if (levelData != null)
            {
                maxCommands = levelData.maxCommands;
                UpdateMaxCommandsDisplay();
            }
        }

        private void OnStartButtonClicked()
        {
            if (isGameRunning)
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
            if (isGameRunning)
            {
                isGamePaused = !isGamePaused;
                ShowPausePanel(isGamePaused);
                OnPauseProgram?.Invoke();
            }
            else
            {
                ShowPausePanel(true);
            }

            AudioManager.Instance?.PlaySound("button_click");
        }

        private void OnMenuButtonClicked()
        {
            AudioManager.Instance?.PlaySound("button_click");
            SceneManager.LoadScene("MainMenu");
        }

        private void OnNextLevelClicked()
        {
            AudioManager.Instance?.PlaySound("button_click");
            
            int nextLevel = currentLevel + 1;
            string nextSceneName = $"Level_{nextLevel:D2}";
            
            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private void OnRetryButtonClicked()
        {
            AudioManager.Instance?.PlaySound("button_click");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnSpeedSliderChanged(float value)
        {
            OnSpeedChanged?.Invoke(value);
        }

        public void SetGameRunning(bool running)
        {
            isGameRunning = running;
            
            if (startButton != null)
            {
                TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    string key = running ? "STOP" : "START";
                    buttonText.text = RobotCoder.UI.LocalizationManager.Instance?.GetText(key) ?? (running ? "СТОП" : "СТАРТ");
                }
            }

            if (resetButton) resetButton.interactable = !running;
        }

        public void UpdateCommandCounter(int count)
        {
            currentCommandCount = count;
            
            if (commandCounterText != null)
            {
                commandCounterText.text = $"{currentCommandCount}";
                
                if (currentCommandCount > maxCommands)
                {
                    commandCounterText.color = Color.red;
                }
                else if (currentCommandCount == maxCommands)
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
                maxCommandsText.text = $"/ {maxCommands}";
            }
        }

        public void UpdateStarsDisplay(int stars)
        {
            if (starsDisplay == null) return;

            for (int i = 0; i < starsDisplay.Length; i++)
            {
                if (starsDisplay[i] != null)
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
                
                LevelButton.SaveLevelProgress(currentLevel, starsEarned);
                MainMenuManager.UnlockLevel(currentLevel + 1);
            }
            
            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowWinPanelDetailed(int starsEarned, int score, float timeSeconds)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);

                if (winTitleText)
                {
                    winTitleText.text = RobotCoder.UI.LocalizationManager.Instance?.GetText("WIN_TITLE") ?? "ПОБЕДА!";
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

                // Score
                if (scoreText)
                {
                    string scoreLabel = RobotCoder.UI.LocalizationManager.Instance?.GetText("SCORE") ?? "Очки";
                    scoreText.text = $"{scoreLabel}: {score}";
                }

                // Time
                if (timeText)
                {
                    string timeLabel = RobotCoder.UI.LocalizationManager.Instance?.GetText("TIME") ?? "Время";
                    timeText.text = $"{timeLabel}: {FormatTime(timeSeconds)}";
                }

                StartCoroutine(AnimatePanel(winPanel, true));

                LevelButton.SaveLevelProgress(currentLevel, starsEarned);
                MainMenuManager.UnlockLevel(currentLevel + 1);
            }

            AudioManager.Instance?.PlaySound("success");
        }

        public void ShowLosePanel()
        {
            if (losePanel != null)
            {
                losePanel.SetActive(true);
                // Title/Message
                if (loseTitleText)
                {
                    loseTitleText.text = RobotCoder.UI.LocalizationManager.Instance?.GetText("LOSE_TITLE") ?? "НЕУДАЧА";
                }
                if (loseMessageText)
                {
                    loseMessageText.text = RobotCoder.UI.LocalizationManager.Instance?.GetText("TRY_AGAIN") ?? "Попробуй еще раз!";
                }
                StartCoroutine(AnimatePanel(losePanel, true));
            }
            
            AudioManager.Instance?.PlaySound("fail");
        }

        public void ShowPausePanel(bool show)
        {
            if (pausePanel != null)
            {
                if (show)
                {
                    pausePanel.SetActive(true);
                    StartCoroutine(AnimatePanel(pausePanel, true));
                    if (pauseTitleText)
                    {
                        pauseTitleText.text = RobotCoder.UI.LocalizationManager.Instance?.GetText("PAUSE") ?? "ПАУЗА";
                    }
                }
                else
                {
                    StartCoroutine(AnimatePanel(pausePanel, false));
                }
            }
            
            isGamePaused = show;
            Time.timeScale = show ? 0f : 1f;
        }

        private static string FormatTime(float seconds)
        {
            if (seconds < 0) return "--:--";
            int s = Mathf.Max(0, Mathf.RoundToInt(seconds));
            int m = s / 60;
            int r = s % 60;
            return $"{m:00}:{r:00}";
        }

        private IEnumerator AnimatePanel(GameObject panel, bool show)
        {
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
                canvasGroup = panel.AddComponent<CanvasGroup>();

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
            Time.timeScale = 1f;
        }
    }
}