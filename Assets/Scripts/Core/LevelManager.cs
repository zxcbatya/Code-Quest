using UnityEngine;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Level Settings")] [SerializeField]
        private LevelData[] levels;

        [SerializeField] private int currentLevelIndex = 0;

        private LevelData _currentLevel;
        private bool _levelCompleted = false;
        private RobotController _robotController;
        private GridManager _gridManager;
        private AchievementManager _achievementManager;

        public System.Action<LevelData> OnLevelLoaded;
        public System.Action OnLevelCompleted;
        public System.Action OnLevelFailed;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
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
            InitializeLevelManager();
        }

        private void InitializeLevelManager()
        {
            if (_robotController == null)
                _robotController = RobotController.Instance;

            if (_gridManager == null)
                _gridManager = GridManager.Instance;

            if (_achievementManager == null)
                _achievementManager = AchievementManager.Instance;

            LoadLevel(currentLevelIndex);
        }

        public void LoadLevel(int levelIndex)
        {
            if (levels == null || levelIndex < 0 || levelIndex >= levels.Length) return;

            currentLevelIndex = levelIndex;
            _currentLevel = levels[levelIndex];

            SetupLevel();
            OnLevelLoaded?.Invoke(_currentLevel);
        }

        private void SetupLevel()
        {
            _levelCompleted = false;

            _gridManager.InitializeGrid(_currentLevel);

            _robotController.Initialize(_currentLevel);
        }

        public void CheckLevelCompletion()
        {
            if (_robotController.IsOnGoal())
            {
                _levelCompleted = true;
                OnLevelCompleted?.Invoke();

                _achievementManager.OnLevelCompleted(currentLevelIndex + 1);

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
            return _currentLevel;
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
            return _levelCompleted;
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