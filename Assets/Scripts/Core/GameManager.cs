using UnityEngine;
using RobotCoder.Core;
using RobotCoder.UI;

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
                // Проверяем, является ли объект корневым перед применением DontDestroyOnLoad
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (robotController == null)
                robotController = RobotController.Instance;
                
            if (programInterpreter == null)
                programInterpreter = ProgramInterpreter.Instance;
                
            if (levelManager == null)
                levelManager = LevelManager.Instance;
                
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            if (programInterpreter != null)
            {
                programInterpreter.OnProgramStarted += OnProgramStarted;
                programInterpreter.OnProgramCompleted += OnProgramCompleted;
                programInterpreter.OnProgramFailed += OnProgramFailed;
            }
            
            if (levelManager != null)
            {
                levelManager.OnLevelCompleted += OnLevelCompleted;
                levelManager.OnLevelFailed += OnLevelFailed;
            }
        }

        public void StartProgram()
        {
            if (workspacePanel == null || programInterpreter == null) return;
            
            if (workspacePanel.HasBlocks())
            {
                CommandBlock[] commands = workspacePanel.GetAllBlocks();
                programInterpreter.ExecuteProgram(commands);
                OnGameStarted?.Invoke();
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
            currentLevel = Mathf.Max(1, level);
            OnLevelChanged?.Invoke(currentLevel);
            
            if (levelManager != null)
            {
                levelManager.LoadLevel(currentLevel - 1);
            }
            
            ResetProgram();
        }
        
        // Event handlers
        private void OnProgramStarted()
        {
            Debug.Log("Программа запущена");
        }
        
        private void OnProgramCompleted()
        {
            Debug.Log("Программа завершена");
        }
        
        private void OnProgramFailed()
        {
            Debug.Log("Программа провалена");
        }
        
        private void OnLevelCompleted()
        {
            Debug.Log("Уровень пройден!");
        }
        
        private void OnLevelFailed()
        {
            Debug.Log("Уровень провален!");
        }
        
        private void OnDestroy()
        {
            // Отписываемся от событий при уничтожении объекта
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