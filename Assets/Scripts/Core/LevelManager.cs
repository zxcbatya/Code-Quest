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

        public System.Action<LevelData> OnLevelLoaded;
        public System.Action OnLevelCompleted;
        public System.Action OnLevelFailed;
        public System.Action OnGameCompleted;

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
            // Add listener for scene loaded event to handle restarts properly
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            InitializeLevelManager();
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            // Reinitialize the level manager when scene is loaded
            InitializeLevelManager();
        }

        private void InitializeLevelManager()
        {
            // Ensure we get fresh instances after scene reload
            _robotController = RobotController.Instance;
            _gridManager = GridManager.Instance;

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
            
            // Update command palette based on level settings
            UpdateCommandPalette();
        }
        
        private void UpdateCommandPalette()
        {
            if (_currentLevel == null) return;
            
            var blockPalette = FindObjectOfType<BlockPalette>();
            if (blockPalette != null)
            {
                blockPalette.SetAvailableCommands(_currentLevel);
            }
        }

        public void CheckLevelCompletion()
        {
            if (_robotController != null && _currentLevel != null)
            {
                Vector2Int robotPosition = _robotController.GetCurrentPosition();
                Debug.Log($"CheckLevelCompletion: Robot position is {robotPosition}");

                // Проверяем достижение целей
                if (_currentLevel.goalPositions != null)
                {
                    foreach (Vector2Int goal in _currentLevel.goalPositions)
                    {
                        if (robotPosition == goal)
                        {
                            Debug.Log($"Robot reached goal at {robotPosition}");
                            CompleteLevel();
                            return;
                        }
                    }
                }

                // Проверяем достижение ворот для завершения игры
                if (_gridManager != null)
                {
                    LevelData.TileType tileType = _gridManager.GetTileType(robotPosition.x, robotPosition.y);
                    Debug.Log($"Robot is on tile type: {tileType} at position {robotPosition}");
                    if (tileType == LevelData.TileType.Door)
                    {
                        Debug.Log($"Robot reached door at {robotPosition}");
                        CompleteLevel();
                        return;
                    }
                }
            }
        }
        
        private void CompleteLevel()
        {
            Debug.Log($"CompleteLevel called. _levelCompleted was {_levelCompleted}");
            if (!_levelCompleted)
            {
                _levelCompleted = true;
                Debug.Log("Level completed - invoking OnLevelCompleted event");
                OnLevelCompleted?.Invoke();
                
                // If this is not the last level, unlock the next one
                if (currentLevelIndex + 1 < levels.Length)
                {
                    UnlockLevel(currentLevelIndex + 1);
                }
            }
            else
            {
                Debug.Log("Level already completed, skipping completion");
            }
        }
        
        public bool IsCurrentLevelCompleted()
        {
            return _levelCompleted;
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

        public bool IsLastLevel()
        {
            return levels != null && currentLevelIndex >= levels.Length - 1;
        }

        public LevelData[] GetAllLevels()
        {
            return levels;
        }

        private void OnDestroy()
        {
            // Unsubscribe from scene loaded event
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            
            // Очищаем ссылки при уничтожении объекта
            OnLevelLoaded = null;
            OnLevelCompleted = null;
            OnLevelFailed = null;
            OnGameCompleted = null;
        }
    }
}            
