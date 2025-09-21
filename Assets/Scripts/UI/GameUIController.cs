using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RobotCoder.Core;

namespace RobotCoder.UI
{
    public class GameUIController : MonoBehaviour
    {
        [Header("UI Панели")]
        [SerializeField] private GameplayUIManager gameplayUI;
        [SerializeField] private WorkspacePanel workspacePanel;
        [SerializeField] private ProgressPanel progressPanel;
        [SerializeField] private BlockPalette blockPalette;
        
        [Header("Настройки уровня")]
        [SerializeField] private LevelData currentLevelData;
        
        public static GameUIController Instance { get; private set; }
        
        public GameplayUIManager GameplayUI => gameplayUI;
        public WorkspacePanel WorkspacePanel => workspacePanel;
        public ProgressPanel ProgressPanel => progressPanel;
        public BlockPalette BlockPalette => blockPalette;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadCurrentLevel();
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
        
        private void LoadCurrentLevel()
        {
            // Загружаем данные текущего уровня
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName.StartsWith("Level_"))
            {
                string levelNumber = sceneName.Substring(6);
                currentLevelData = Resources.Load<LevelData>("Levels/Level_" + levelNumber);
            }
        }
        
        private void InitializeUI()
        {
            if (currentLevelData != null)
            {
                // Настраиваем палитру блоков
                if (blockPalette != null)
                {
                    blockPalette.SetAvailableCommands(currentLevelData);
                }
                
                // Настраиваем прогресс панель
                if (progressPanel != null)
                {
                    string levelTitle = "Уровень " + currentLevelData.levelIndex;
                    string objective = currentLevelData.description;
                    int goalCount = currentLevelData.goalPositions?.Length ?? 1;
                    
                    progressPanel.SetLevelInfo(levelTitle, objective, goalCount);
                }
                
                // Обновляем счетчик команд в геймплей UI
                if (gameplayUI != null)
                {
                    gameplayUI.UpdateCommandCounter(0);
                }
            }
        }
        
        private void SetupEventListeners()
        {
            // События геймплей UI
            if (gameplayUI != null)
            {
                gameplayUI.OnStartProgram += HandleStartProgram;
                gameplayUI.OnResetProgram += HandleResetProgram;
                gameplayUI.OnPauseProgram += HandlePauseProgram;
                gameplayUI.OnSpeedChanged += HandleSpeedChanged;
            }
            
            // События рабочей области
            if (workspacePanel != null)
            {
                workspacePanel.OnCommandCountChanged += HandleCommandCountChanged;
                workspacePanel.OnWorkspaceCleared += HandleWorkspaceCleared;
            }
            
            // События прогресс панели
            if (progressPanel != null)
            {
                progressPanel.OnAllGoalsCompleted += HandleAllGoalsCompleted;
            }
        }
        
        private void HandleStartProgram()
        {
            Debug.Log("Запуск программы");
            
            // Получаем все блоки из рабочей области
            if (workspacePanel != null)
            {
                var blocks = workspacePanel.GetAllBlocks();
                if (blocks.Length > 0)
                {
                    // Здесь будет запуск интерпретатора программы
                    // ProgramInterpreter.Instance?.ExecuteProgram(blocks);
                }
            }
        }
        
        private void HandleResetProgram()
        {
            Debug.Log("Сброс программы");
            
            // Сбрасываем прогресс
            if (progressPanel != null)
            {
                progressPanel.ResetProgress();
            }
            
            // Сбрасываем состояние робота
            // RobotController.Instance?.ResetToStart();
        }
        
        private void HandlePauseProgram()
        {
            Debug.Log("Пауза программы");
            
            // Приостанавливаем выполнение
            // ProgramInterpreter.Instance?.PauseExecution();
        }
        
        private void HandleSpeedChanged(float speed)
        {
            Debug.Log("Изменена скорость: " + speed);
            
            // Изменяем скорость выполнения
            // ProgramInterpreter.Instance?.SetExecutionSpeed(speed);
        }
        
        private void HandleCommandCountChanged(int count)
        {
            if (gameplayUI != null)
            {
                gameplayUI.UpdateCommandCounter(count);
            }
            
            // Проверяем лимит команд
            if (currentLevelData != null && count > currentLevelData.maxCommands)
            {
                Debug.Log("Превышен лимит команд!");
                // Можно показать предупреждение
            }
        }
        
        private void HandleWorkspaceCleared()
        {
            Debug.Log("Рабочая область очищена");
            
            if (gameplayUI != null)
            {
                gameplayUI.UpdateCommandCounter(0);
            }
        }
        
        private void HandleAllGoalsCompleted()
        {
            Debug.Log("Все цели достигнуты!");
            
            // Вычисляем количество звезд
            int stars = CalculateStars();
            
            if (gameplayUI != null)
            {
                gameplayUI.ShowWinPanel(stars);
            }
        }
        
        private int CalculateStars()
        {
            if (currentLevelData == null || workspacePanel == null) return 1;
            
            int commandCount = workspacePanel.GetAllBlocks().Length;
            int optimalCommands = currentLevelData.optimalCommands;
            int maxCommands = currentLevelData.maxCommands;
            
            // 3 звезды - оптимальное решение
            if (commandCount <= optimalCommands)
                return 3;
            
            // 2 звезды - хорошее решение
            if (commandCount <= (optimalCommands + maxCommands) / 2)
                return 2;
            
            // 1 звезда - просто прошел уровень
            return 1;
        }
        
        public void ShowLevelCompleted(int stars)
        {
            if (gameplayUI != null)
            {
                gameplayUI.ShowWinPanel(stars);
            }
        }
        
        public void ShowLevelFailed()
        {
            if (gameplayUI != null)
            {
                gameplayUI.ShowLosePanel();
            }
        }
        
        public void UpdateProgress(int completedGoals)
        {
            if (progressPanel != null)
            {
                progressPanel.UpdateProgress(completedGoals);
            }
        }
        
        private void OnDestroy()
        {
            // Отписываемся от событий
            if (gameplayUI != null)
            {
                gameplayUI.OnStartProgram -= HandleStartProgram;
                gameplayUI.OnResetProgram -= HandleResetProgram;
                gameplayUI.OnPauseProgram -= HandlePauseProgram;
                gameplayUI.OnSpeedChanged -= HandleSpeedChanged;
            }
            
            if (workspacePanel != null)
            {
                workspacePanel.OnCommandCountChanged -= HandleCommandCountChanged;
                workspacePanel.OnWorkspaceCleared -= HandleWorkspaceCleared;
            }
            
            if (progressPanel != null)
            {
                progressPanel.OnAllGoalsCompleted -= HandleAllGoalsCompleted;
            }
        }
    }
}