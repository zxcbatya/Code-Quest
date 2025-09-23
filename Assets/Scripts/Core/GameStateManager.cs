using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        LevelComplete,
        GameOver
    }
    
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Menu;
        
        [Header("Level Info")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 15;
        
        // События изменения состояния
        public System.Action<GameState> OnGameStateChanged;
        public System.Action<int> OnLevelChanged;
        
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
            // Определяем текущий уровень из сцены
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.StartsWith("Level_"))
            {
                string levelNumberStr = sceneName.Substring(6);
                if (int.TryParse(levelNumberStr, out int level))
                {
                    currentLevel = level;
                    SetGameState(GameState.Playing);
                }
            }
        }
        
        public void SetGameState(GameState newState)
        {
            GameState previousState = currentState;
            currentState = newState;
            
            // Вызываем событие изменения состояния
            OnGameStateChanged?.Invoke(currentState);
            
            // Дополнительная логика для определенных состояний
            switch (newState)
            {
                case GameState.Menu:
                    Time.timeScale = 1f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.LevelComplete:
                    Time.timeScale = 1f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 1f;
                    break;
            }
        }
        
        public GameState GetGameState()
        {
            return currentState;
        }
        
        public void LoadLevel(int levelNumber)
        {
            if (levelNumber >= 1 && levelNumber <= maxLevel)
            {
                currentLevel = levelNumber;
                OnLevelChanged?.Invoke(currentLevel);
                
                string sceneName = $"Level_{levelNumber:D2}";
                if (Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    SceneManager.LoadScene(sceneName);
                    SetGameState(GameState.Playing);
                }
            }
        }
        
        public void LoadNextLevel()
        {
            if (currentLevel < maxLevel)
            {
                LoadLevel(currentLevel + 1);
            }
            else
            {
                // Все уровни пройдены, возвращаемся в меню
                LoadMainMenu();
            }
        }
        
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
            SetGameState(GameState.Menu);
        }
        
        public void RestartLevel()
        {
            LoadLevel(currentLevel);
        }
        
        public int GetCurrentLevel()
        {
            return currentLevel;
        }
        
        public void SetCurrentLevel(int level)
        {
            if (level >= 1 && level <= maxLevel)
            {
                currentLevel = level;
                OnLevelChanged?.Invoke(currentLevel);
            }
        }
        
        public int GetMaxLevel()
        {
            return maxLevel;
        }
        
        public void SetMaxLevel(int max)
        {
            maxLevel = max;
        }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Обновляем текущий уровень при загрузке сцены
            string sceneName = scene.name;
            if (sceneName.StartsWith("Level_"))
            {
                string levelNumberStr = sceneName.Substring(6);
                if (int.TryParse(levelNumberStr, out int level))
                {
                    currentLevel = level;
                    OnLevelChanged?.Invoke(currentLevel);
                }
            }
        }
    }
}