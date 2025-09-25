using UnityEngine;
using System.Collections;
using RobotCoder.Core;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        
        [Header("Level Settings")]
        [SerializeField] private LevelData[] levels;
        [SerializeField] private int currentLevelIndex = 0;
        
        [Header("References")]
        [SerializeField] private RobotController robotController;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private AchievementManager achievementManager;
        
        private LevelData currentLevel;
        private bool levelCompleted = false;
        
        public System.Action<LevelData> OnLevelLoaded;
        public System.Action OnLevelCompleted;
        public System.Action OnLevelFailed;
        
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
            if (robotController == null)
                robotController = RobotController.Instance;
                
            if (gridManager == null)
                gridManager = GridManager.Instance;
                
            if (achievementManager == null)
                achievementManager = AchievementManager.Instance;
                
            LoadLevel(currentLevelIndex);
        }
        
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levels.Length) return;
            
            currentLevelIndex = levelIndex;
            currentLevel = levels[levelIndex];
            
            SetupLevel();
            OnLevelLoaded?.Invoke(currentLevel);
        }
        
        private void SetupLevel()
        {
            if (currentLevel == null) return;
            
            // Reset level completion status
            levelCompleted = false;
            
            // Initialize grid
            if (gridManager != null)
            {
                gridManager.InitializeGrid(currentLevel);
            }
            
            // Initialize robot
            if (robotController != null)
            {
                robotController.Initialize(currentLevel);
            }
        }
        
        public void CheckLevelCompletion()
        {
            if (levelCompleted || currentLevel == null || robotController == null) return;
            
            // Check if robot is on goal position
            if (robotController.IsOnGoal())
            {
                levelCompleted = true;
                OnLevelCompleted?.Invoke();
                
                // Notify achievement manager
                if (achievementManager != null)
                {
                    achievementManager.OnLevelCompleted(currentLevelIndex + 1);
                }
                
                // Unlock next level
                if (currentLevelIndex + 1 < levels.Length)
                {
                    UnlockLevel(currentLevelIndex + 1);
                }
            }
        }
        
        private void UnlockLevel(int levelIndex)
        {
            // In a full implementation, you would save unlocked levels
            Debug.Log($"Уровень {levelIndex + 1} разблокирован!");
        }
        
        public LevelData GetCurrentLevel()
        {
            return currentLevel;
        }
        
        public int GetCurrentLevelIndex()
        {
            return currentLevelIndex;
        }
        
        public int GetLevelCount()
        {
            return levels != null ? levels.Length : 0;
        }
        
        public bool IsCurrentLevelCompleted()
        {
            return levelCompleted;
        }
        
        public LevelData[] GetAllLevels()
        {
            return levels;
        }
        
        private void OnDestroy()
        {
            // Очищаем ссылки при уничтожении объекта
            OnLevelLoaded = null;
            OnLevelCompleted = null;
            OnLevelFailed = null;
        }
    }
}