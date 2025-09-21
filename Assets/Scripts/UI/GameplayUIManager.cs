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
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button retryButton;

        [Header("Информационные элементы")]
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private TextMeshProUGUI commandCounterText;
        [SerializeField] private TextMeshProUGUI maxCommandsText;
        [SerializeField] private Image[] starsDisplay;
        [SerializeField] private Slider speedSlider;

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

        // События
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
            // Скрываем панели результатов
            if (winPanel) winPanel.SetActive(false);
            if (losePanel) losePanel.SetActive(false);
            if (pausePanel) pausePanel.SetActive(false);

            // Устанавливаем начальные значения
            UpdateCommandCounter(0);
            UpdateStarsDisplay(0);
            
            // Настраиваем слайдер скорости
            if (speedSlider != null)
            {
                speedSlider.value = 1f;
                speedSlider.minValue = 0.5f;
                speedSlider.maxValue = 3f;
            }
        }

        private void SetupEventListeners()
        {
            // Кнопки управления
            if (startButton) startButton.onClick.AddListener(OnStartButtonClicked);
            if (resetButton) resetButton.onClick.AddListener(OnResetButtonClicked);
            if (pauseButton) pauseButton.onClick.AddListener(OnPauseButtonClicked);
            if (menuButton) menuButton.onClick.AddListener(OnMenuButtonClicked);
            if (nextLevelButton) nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            if (retryButton) retryButton.onClick.AddListener(OnRetryButtonClicked);

            // Слайдер скорости
            if (speedSlider) speedSlider.onValueChanged.AddListener(OnSpeedSliderChanged);

            // Клавиши быстрого доступа
            InputManager.Instance?.RegisterKeyAction(KeyCode.Space, OnStartButtonClicked);
            InputManager.Instance?.RegisterKeyAction(KeyCode.R, OnResetButtonClicked);
            InputManager.Instance?.RegisterKeyAction(KeyCode.Escape, OnPauseButtonClicked);
        }

        private void LoadLevelData()
        {
            // Получаем данные текущего уровня
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
                string localizedTitle = LocalizationManager.Instance?.GetText("LEVEL") ?? "Уровень";
                levelTitleText.text = $"{localizedTitle} {currentLevel}";
            }

            // Загружаем данные уровня из ScriptableObject
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
                // Остановка программы
                OnPauseProgram?.Invoke();
                SetGameRunning(false);
            }
            else
            {
                // Запуск программы
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
                // Возвращаемся в главное меню
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
                    buttonText.text = LocalizationManager.Instance?.GetText(key) ?? (running ? "СТОП" : "СТАРТ");
                }
            }

            // Блокируем/разблокируем кнопки
            if (resetButton) resetButton.interactable = !running;
        }

        public void UpdateCommandCounter(int count)
        {
            currentCommandCount = count;
            
            if (commandCounterText != null)
            {
                commandCounterText.text = $"{currentCommandCount}";
                
                // Меняем цвет если превышен лимит
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
                
                // Сохраняем прогресс
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
                }
                else
                {
                    StartCoroutine(AnimatePanel(pausePanel, false));
                }
            }
            
            isGamePaused = show;
            Time.timeScale = show ? 0f : 1f;
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
            // Восстанавливаем нормальную скорость времени
            Time.timeScale = 1f;
        }
    }
}