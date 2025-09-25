using UnityEngine;

namespace Core
{
    public class GameData : MonoBehaviour
    {
        public static GameData Instance { get; private set; }
        
        [Header("Game Configuration")]
        [SerializeField] private int maxLevelCount = 15;
        [SerializeField] private int maxCommandsPerLevel = 50;
        [SerializeField] private bool enableDebugMode = false;
        [SerializeField] private bool showGrid = true;
        
        [Header("Player Progress")]
        [SerializeField] private int playerLevel = 1;
        [SerializeField] private int totalStars = 0;
        [SerializeField] private int totalCoins = 0;
        
        [Header("Game Settings")]
        [SerializeField] private float gameSpeed = 1.0f;
        [SerializeField] private bool soundEnabled = true;
        [SerializeField] private bool musicEnabled = true;
        [SerializeField] private string language = "ru";
        
        public System.Action OnGameDataChanged;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadGameData();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void LoadGameData()
        {
            // Load game data from PlayerPrefs
            playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
            totalStars = PlayerPrefs.GetInt("TotalStars", 0);
            totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
            gameSpeed = PlayerPrefs.GetFloat("GameSpeed", 1.0f);
            soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
            musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            language = PlayerPrefs.GetString("Language", "ru");
            showGrid = PlayerPrefs.GetInt("ShowGrid", 1) == 1;
        }
        
        public void SaveGameData()
        {
            // Save game data to PlayerPrefs
            PlayerPrefs.SetInt("PlayerLevel", playerLevel);
            PlayerPrefs.SetInt("TotalStars", totalStars);
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.SetFloat("GameSpeed", gameSpeed);
            PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
            PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
            PlayerPrefs.SetString("Language", language);
            PlayerPrefs.SetInt("ShowGrid", showGrid ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        // Getters and Setters
        public int GetMaxLevelCount() => maxLevelCount;
        public int GetMaxCommandsPerLevel() => maxCommandsPerLevel;
        public bool IsDebugModeEnabled() => enableDebugMode;
        public bool GetShowGrid() => showGrid;
        public int GetPlayerLevel() => playerLevel;
        public int GetTotalStars() => totalStars;
        public int GetTotalCoins() => totalCoins;
        public float GetGameSpeed() => gameSpeed;
        public bool IsSoundEnabled() => soundEnabled;
        public bool IsMusicEnabled() => musicEnabled;
        public string GetLanguage() => language;
        
        public void SetPlayerLevel(int level)
        {
            playerLevel = Mathf.Max(1, level);
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void AddStars(int stars)
        {
            totalStars += stars;
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void AddCoins(int coins)
        {
            totalCoins += coins;
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void SetGameSpeed(float speed)
        {
            gameSpeed = Mathf.Clamp(speed, 0.1f, 3.0f);
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void SetSoundEnabled(bool enabled)
        {
            soundEnabled = enabled;
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void SetLanguage(string lang)
        {
            language = lang;
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void SetShowGrid(bool show)
        {
            showGrid = show;
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
        
        public void ResetGameData()
        {
            playerLevel = 1;
            totalStars = 0;
            totalCoins = 0;
            gameSpeed = 1.0f;
            soundEnabled = true;
            musicEnabled = true;
            language = "ru";
            showGrid = true;
            SaveGameData();
            OnGameDataChanged?.Invoke();
        }
    }
}