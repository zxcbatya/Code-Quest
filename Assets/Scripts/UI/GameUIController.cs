using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;
using RobotCoder.UI;

namespace UI
{
    public class GameUIController : MonoBehaviour
    {
        public static GameUIController Instance { get; private set; }
        
        [Header("UI Panels")]
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject levelCompletePanel;
        
        [Header("Gameplay UI")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI commandCountText;
        [SerializeField] private WorkspacePanel workspacePanel;
        
        [Header("Level Complete UI")]
        [SerializeField] private TextMeshProUGUI levelCompleteText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button menuButton;
        
        private GameManager gameManager;
        private LevelManager levelManager;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
        }
        
        private void InitializeUI()
        {
            gameManager = GameManager.Instance;
            levelManager = LevelManager.Instance;
            
            ShowGameplayPanel();
            UpdateLevelText();
            
            if (workspacePanel != null)
            {
                workspacePanel.OnCommandCountChanged += OnCommandCountChanged;
                OnCommandCountChanged(workspacePanel.HasBlocks() ? workspacePanel.GetAllBlocks().Length : 0);
            }
        }
        
        private void SetupEventListeners()
        {
            // Очищаем существующие слушатели
            ClearEventListeners();
            
            // Button events
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueButtonClicked);
                
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuButtonClicked);
                
            // Game manager events
            if (gameManager != null)
            {
                gameManager.OnGameStarted += OnGameStarted;
                gameManager.OnGamePaused += OnGamePaused;
                gameManager.OnGameReset += OnGameReset;
                gameManager.OnLevelChanged += OnLevelChanged;
            }
            
            // Level manager events
            if (levelManager != null)
            {
                levelManager.OnLevelCompleted += OnLevelCompleted;
                levelManager.OnLevelLoaded += OnLevelLoaded;
            }
        }
        
        private void ClearEventListeners()
        {
            // Удаляем все существующие слушатели событий
            if (continueButton != null) continueButton.onClick.RemoveAllListeners();
            if (menuButton != null) menuButton.onClick.RemoveAllListeners();
        }
        
        public void ShowGameplayPanel()
        {
            if (gameplayPanel != null) gameplayPanel.SetActive(true);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        }
        
        public void ShowLevelCompletePanel()
        {
            if (gameplayPanel != null) gameplayPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
            
            if (levelCompleteText != null && levelManager != null)
            {
                levelCompleteText.text = $"Уровень {levelManager.GetCurrentLevelIndex() + 1} пройден!";
            }
        }
        
        private void UpdateLevelText()
        {
            if (levelText != null && gameManager != null)
            {
                levelText.text = $"Уровень {gameManager.GetCurrentLevel()}";
            }
        }
        
        private void OnCommandCountChanged(int count)
        {
            if (commandCountText != null)
            {
                commandCountText.text = $"Команд: {count}";
            }
        }
        
        // Button event handlers
        private void OnContinueButtonClicked()
        {
            gameManager?.NextLevel();
            ShowGameplayPanel();
        }
        
        private void OnMenuButtonClicked()
        {
            // Return to main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        
        // Game event handlers
        private void OnGameStarted()
        {
            // Здесь можно добавить логику при начале игры
        }
        
        private void OnGamePaused()
        {
            // Здесь можно добавить логику при паузе
        }
        
        private void OnGameReset()
        {
            // Здесь можно добавить логику при сбросе
        }
        
        private void OnLevelChanged(int level)
        {
            UpdateLevelText();
        }
        
        private void OnLevelCompleted()
        {
            ShowLevelCompletePanel();
        }
        
        private void OnLevelLoaded(LevelData level)
        {
            UpdateLevelText();
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            ClearEventListeners();
            
            if (workspacePanel != null)
            {
                workspacePanel.OnCommandCountChanged -= OnCommandCountChanged;
            }
            
            if (gameManager != null)
            {
                gameManager.OnGameStarted -= OnGameStarted;
                gameManager.OnGamePaused -= OnGamePaused;
                gameManager.OnGameReset -= OnGameReset;
                gameManager.OnLevelChanged -= OnLevelChanged;
            }
            
            if (levelManager != null)
            {
                levelManager.OnLevelCompleted -= OnLevelCompleted;
                levelManager.OnLevelLoaded -= OnLevelLoaded;
            }
        }
        
        private void OnDisable()
        {
            // Unsubscribe from events when disabled
            ClearEventListeners();
        }
    }
}