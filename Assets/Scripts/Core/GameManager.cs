using UnityEngine;
using RobotCoder.Core;
using RobotCoder.UI;
using UI;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private int currentLevel = 1;
        [SerializeField] private RobotController robotController;
        [SerializeField] private ProgramInterpreter programInterpreter;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private WorkspacePanel workspacePanel;
        
        public System.Action OnGameStarted;
        public System.Action OnGamePaused;
        public System.Action OnGameReset;
        public System.Action<int> OnLevelChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
                
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene loaded: {scene.name}, mode: {mode}");
            InitializeGame();
        }

        private void Start()
        {
            Time.timeScale = 1f;
            InitializeGame();
        }

        private void InitializeGame()
        {
            Debug.Log("Initializing game...");
            
            // Always update references on initialization
            robotController = RobotController.Instance;
            programInterpreter = ProgramInterpreter.Instance;
            levelManager = LevelManager.Instance;
            
            // Find workspace panel if not set
            if (workspacePanel == null)
            {
                workspacePanel = FindObjectOfType<WorkspacePanel>();
            }
            
            SetupEventListeners();
            SetupGameplayActions();
            
            Debug.Log($"Game initialized. Robot: {robotController != null}, ProgramInterpreter: {programInterpreter != null}, LevelManager: {levelManager != null}, WorkspacePanel: {workspacePanel != null}");
        }
        
        private void SetupGameplayActions()
        {
            var gameplayUI = FindObjectOfType<GameplayUIManager>();
            if (gameplayUI != null)
            {
                gameplayUI.OnStartProgram = StartProgram;
                gameplayUI.OnResetProgram = ResetProgram;
                gameplayUI.OnPauseProgram = PauseProgram;
                Debug.Log("Gameplay actions setup completed");
            }
            else
            {
                Debug.LogError("GameplayUIManager not found!");
            }
        }

        private void SetupEventListeners()
        {
            // Clear existing listeners before setting up new ones
            if (programInterpreter != null)
            {
                programInterpreter.OnProgramStarted -= OnProgramStarted;
                programInterpreter.OnProgramCompleted -= OnProgramCompleted;
                programInterpreter.OnProgramFailed -= OnProgramFailed;
                programInterpreter.OnProgramStarted += OnProgramStarted;
                programInterpreter.OnProgramCompleted += OnProgramCompleted;
                programInterpreter.OnProgramFailed += OnProgramFailed;
            }
            
            if (levelManager != null)
            {
                levelManager.OnLevelCompleted -= OnLevelCompleted;
                levelManager.OnLevelFailed -= OnLevelFailed;
                levelManager.OnLevelCompleted += OnLevelCompleted;
                levelManager.OnLevelFailed += OnLevelFailed;
            }
        }

        public void StartProgram()
        {
            Debug.Log($"StartProgram called. workspacePanel: {workspacePanel != null}, programInterpreter: {programInterpreter != null}");
            
            if (workspacePanel == null || programInterpreter == null) 
            {
                Debug.LogError("StartProgram failed: workspacePanel or programInterpreter is null");
                return;
            }
            
            if (workspacePanel.HasBlocks())
            {
                CommandBlock[] commands = workspacePanel.GetAllBlocks();
                Debug.Log($"Starting program with {commands.Length} commands");
                
                // Make sure robot is not moving before start
                if (robotController != null && robotController.IsMoving())
                {
                    robotController.ResetToStart();
                }
                
                programInterpreter.ExecuteProgram(commands);
                OnGameStarted?.Invoke();
            }
            else
            {
                Debug.Log("No blocks in workspace");
            }
        }

        public void PauseProgram()
        {
            if (programInterpreter != null)
            {
                programInterpreter.PauseExecution();
                OnGamePaused?.Invoke();
            }
        }

        public void ResetProgram()
        {
            if (programInterpreter != null)
            {
                programInterpreter.StopExecution();
            }
            
            if (robotController != null)
            {
                robotController.ResetToStart();
            }
            
            // Make sure game time scale is normalized
            Time.timeScale = 1f;
            
            OnGameReset?.Invoke();
        }

        public void NextLevel()
        {
            currentLevel++;
            OnLevelChanged?.Invoke(currentLevel);
            
            if (levelManager != null)
            {
                levelManager.LoadLevel(currentLevel - 1);
            }
            
            ResetProgram();
        }

        public void PreviousLevel()
        {
            currentLevel = Mathf.Max(1, currentLevel - 1);
            OnLevelChanged?.Invoke(currentLevel);
            
            if (levelManager != null)
            {
                levelManager.LoadLevel(currentLevel - 1);
            }
            
            ResetProgram();
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public void SetCurrentLevel(int level)
        {
            currentLevel = level;
            OnLevelChanged?.Invoke(currentLevel);
            
            if (levelManager != null)
            {
                levelManager.LoadLevel(currentLevel - 1);
            }
            
            ResetProgram();
        }
        
        public void ReinitializeEventListeners()
        {
            SetupEventListeners();
        }
        
        public void ReinitializeGameplayActions()
        {
            SetupGameplayActions();
        }

        private void OnProgramStarted()
        {
            Debug.Log("Программа запущена");
        }
        
        private void OnProgramCompleted()
        {
            Debug.Log("OnProgramCompleted: Program finished execution");
            if (levelManager != null)
            {
                levelManager.CheckLevelCompletion();
                
                bool isLevelCompleted = levelManager.IsCurrentLevelCompleted();
                Debug.Log($"OnProgramCompleted: Is level completed? {isLevelCompleted}");
                
                // Only trigger failure if the level is NOT completed
                if (!isLevelCompleted)
                {
                    Debug.Log("OnProgramCompleted: Level not completed, triggering failure");
                    OnProgramFailed();
                }
                else
                {
                    Debug.Log("OnProgramCompleted: Level completed, no failure triggered");
                }
            }
        }
        
        private void OnProgramFailed()
        {
            Debug.Log("Программа провалена");
            var gameplayUI = FindObjectOfType<GameplayUIManager>();
            if (gameplayUI != null)
            {
                gameplayUI.ShowLosePanel();
            }
            
            if (robotController != null)
            {
                robotController.ResetToStart();
            }
            
            Time.timeScale = 1f;
        }
        
        private void OnLevelCompleted()
        {
            Debug.Log("OnLevelCompleted: Level completion event received");
            var gameplayUI = FindObjectOfType<GameplayUIManager>();
            if (gameplayUI != null)
            {
                Debug.Log("OnLevelCompleted: Showing win panel");
                int commandsUsed = workspacePanel != null ? workspacePanel.GetAllBlocks().Length : 0;
                LevelData currentLevelData = levelManager?.GetCurrentLevel();
                int stars = 1; 
                
                if (currentLevelData != null)
                {
                    if (commandsUsed <= currentLevelData.optimalCommands)
                        stars = 3;
                    else if (commandsUsed <= currentLevelData.optimalCommands + 2)
                        stars = 2;
                }
                
                gameplayUI.ShowWinPanel(stars);
            }
            else
            {
                Debug.LogError("OnLevelCompleted: GameplayUIManager not found!");
            }
        }
        
        private void OnLevelFailed()
        {
            Debug.Log("Уровень провален!");
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            if (programInterpreter != null)
            {
                programInterpreter.OnProgramStarted -= OnProgramStarted;
                programInterpreter.OnProgramCompleted -= OnProgramCompleted;
                programInterpreter.OnProgramFailed -= OnProgramFailed;
            }
            
            if (levelManager != null)
            {
                levelManager.OnLevelCompleted -= OnLevelCompleted;
                levelManager.OnLevelFailed -= OnLevelFailed;
            }
        }
    }
}    
               