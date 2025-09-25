using UnityEngine;
using Core;
using RobotCoder.Core;
using RobotCoder.UI;

namespace Core
{
    /// <summary>
    /// Тестовый скрипт для проверки работы основных систем игры
    /// </summary>
    public class SystemTester : MonoBehaviour
    {
        [Header("Тестовые ссылки")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private RobotController robotController;
        [SerializeField] private ProgramInterpreter programInterpreter;
        [SerializeField] private WorkspacePanel workspacePanel;
        
        private void Start()
        {
            Debug.Log("=== ТЕСТИРОВАНИЕ СИСТЕМ ИГРЫ ===");
            
            // Проверяем основные менеджеры
            gameManager = GameManager.Instance;
            levelManager = LevelManager.Instance;
            robotController = RobotController.Instance;
            programInterpreter = ProgramInterpreter.Instance;
            
            if (gameManager != null)
                Debug.Log("✓ GameManager инициализирован");
            else
                Debug.LogError("✗ GameManager не инициализирован");
                
            if (levelManager != null)
                Debug.Log("✓ LevelManager инициализирован");
            else
                Debug.LogError("✗ LevelManager не инициализирован");
                
            if (robotController != null)
                Debug.Log("✓ RobotController инициализирован");
            else
                Debug.LogError("✗ RobotController не инициализирован");
                
            if (programInterpreter != null)
                Debug.Log("✓ ProgramInterpreter инициализирован");
            else
                Debug.LogError("✗ ProgramInterpreter не инициализирован");
                
            // Проверяем уровни
            if (levelManager != null)
            {
                int levelCount = levelManager.GetLevelCount();
                Debug.Log($"✓ Доступно уровней: {levelCount}");
                
                if (levelCount > 0)
                {
                    var currentLevel = levelManager.GetCurrentLevel();
                    if (currentLevel != null)
                    {
                        Debug.Log($"✓ Текущий уровень: {currentLevel.levelName}");
                    }
                }
            }
            
            Debug.Log("=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО ===");
        }
        
        private void Update()
        {
            // Тестовые горячие клавиши
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("=== СОСТОЯНИЕ СИСТЕМ ===");
                Debug.Log($"GameManager: {gameManager != null}");
                Debug.Log($"LevelManager: {levelManager != null}");
                Debug.Log($"RobotController: {robotController != null}");
                Debug.Log($"ProgramInterpreter: {programInterpreter != null}");
                Debug.Log($"WorkspacePanel: {workspacePanel != null}");
                
                if (programInterpreter != null)
                {
                    Debug.Log($"ProgramInterpreter.IsExecuting: {programInterpreter.IsExecuting()}");
                    Debug.Log($"ProgramInterpreter.IsPaused: {programInterpreter.IsPaused()}");
                }
                
                if (robotController != null)
                {
                    Debug.Log($"RobotController.IsMoving: {robotController.IsMoving()}");
                    Debug.Log($"RobotController.Position: {robotController.GetCurrentPosition()}");
                }
            }
        }
    }
}