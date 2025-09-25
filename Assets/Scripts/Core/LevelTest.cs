using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class LevelTest : MonoBehaviour
    {
        [Header("Level Testing")]
        [SerializeField] private LevelData testLevel;
        [SerializeField] private RobotController robotController;
        [SerializeField] private GridManager gridManager;
        
        private void Start()
        {
            InitializeTestLevel();
        }
        
        private void InitializeTestLevel()
        {
            if (testLevel == null) return;
            
            if (gridManager != null)
            {
                gridManager.InitializeGrid(testLevel);
            }
            
            if (robotController != null)
            {
                robotController.Initialize(testLevel);
            }
            
            Debug.Log($"Тестовый уровень загружен: {testLevel.levelName}");
        }
        
        // Test methods that can be called from buttons
        public void TestMoveForward()
        {
            if (robotController != null)
            {
                robotController.MoveForward();
            }
        }
        
        public void TestTurnLeft()
        {
            if (robotController != null)
            {
                robotController.TurnLeft();
            }
        }
        
        public void TestTurnRight()
        {
            if (robotController != null)
            {
                robotController.TurnRight();
            }
        }
        
        public void TestJump()
        {
            if (robotController != null)
            {
                robotController.Jump();
            }
        }
        
        public void TestReset()
        {
            if (robotController != null)
            {
                robotController.ResetToStart();
            }
        }
        
        public void TestCheckGoal()
        {
            if (robotController != null)
            {
                bool onGoal = robotController.IsOnGoal();
                Debug.Log($"Робот на цели: {onGoal}");
            }
        }
    }
}