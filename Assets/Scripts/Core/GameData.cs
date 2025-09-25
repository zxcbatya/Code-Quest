using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    [System.Serializable]
    public class GameData
    {
        public int unlockedLevels = 1;
        public int totalStars = 0;
        public float playTime = 0f;
        public int completedLevels = 0;
        public float masterVolume = 1.0f;
        public float sfxVolume = 1.0f;
        public string language = "RU";
        public bool showGrid = true;
        public Dictionary<string, int> levelStars = new Dictionary<string, int>();
        public Dictionary<string, bool> levelCompleted = new Dictionary<string, bool>();
        public List<string> unlockedAchievements = new List<string>();
        public int totalCommandsExecuted = 0;
        public int totalBlocksPlaced = 0;
        public int totalRestarts = 0;
        
        public GameData()
        {
            unlockedLevels = 1;
            totalStars = 0;
            playTime = 0f;
            completedLevels = 0;
            masterVolume = 1.0f;
            sfxVolume = 1.0f;
            language = "RU";
            showGrid = true;
            levelStars = new Dictionary<string, int>();
            levelCompleted = new Dictionary<string, bool>();
            unlockedAchievements = new List<string>();
            totalCommandsExecuted = 0;
            totalBlocksPlaced = 0;
            totalRestarts = 0;
        }
    }
    
    public class GameDataManager : MonoBehaviour
    {
        public static GameDataManager Instance { get; private set; }
        
        [SerializeField] private GameData gameData = new GameData();
        
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
        
        public GameData GetGameData()
        {
            return gameData;
        }
        
        public void SaveGameData()
        {
            string json = JsonUtility.ToJson(gameData, true);
            PlayerPrefs.SetString("GameData", json);
            PlayerPrefs.Save();
        }
        
        public void LoadGameData()
        {
            if (PlayerPrefs.HasKey("GameData"))
            {
                string json = PlayerPrefs.GetString("GameData");
                gameData = JsonUtility.FromJson<GameData>(json);
            }
        }
        
        public void ResetGameData()
        {
            gameData = new GameData();
            SaveGameData();
        }
    }
}