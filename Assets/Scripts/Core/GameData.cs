using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    [System.Serializable]
    public class GameData
    {
        // Данные игрока
        public int unlockedLevels = 1;
        public int totalStars = 0;
        public float playTime = 0f;
        public int completedLevels = 0;
        
        // Настройки
        public float masterVolume = 1.0f;
        public float sfxVolume = 1.0f;
        public string language = "RU";
        public bool showGrid = true;
        
        // Прогресс по уровням
        public Dictionary<string, int> levelStars = new Dictionary<string, int>();
        public Dictionary<string, bool> levelCompleted = new Dictionary<string, bool>();
        
        // Достижения
        public List<string> unlockedAchievements = new List<string>();
        
        // Статистика
        public int totalCommandsExecuted = 0;
        public int totalBlocksPlaced = 0;
        public int totalRestarts = 0;
        
        public GameData()
        {
            // Инициализация по умолчанию
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
        
        private GameData currentGameData;
        private const string SAVE_KEY = "GameData";
        
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
            string jsonData = SaveManager.LoadSettings(SAVE_KEY, "");
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    currentGameData = JsonUtility.FromJson<GameData>(jsonData);
                }
                catch
                {
                    currentGameData = new GameData();
                }
            }
            else
            {
                currentGameData = new GameData();
            }
        }
        
        public void SaveGameData()
        {
            string jsonData = JsonUtility.ToJson(currentGameData);
            SaveManager.SaveSettings(SAVE_KEY, jsonData);
        }
        
        public GameData GetGameData()
        {
            return currentGameData;
        }
        
        public void UpdatePlayTime(float deltaTime)
        {
            currentGameData.playTime += deltaTime;
        }
        
        public void AddLevelStars(string levelId, int stars)
        {
            if (currentGameData.levelStars.ContainsKey(levelId))
            {
                if (stars > currentGameData.levelStars[levelId])
                {
                    // Обновляем количество звезд, если результат лучше
                    currentGameData.totalStars += stars - currentGameData.levelStars[levelId];
                    currentGameData.levelStars[levelId] = stars;
                }
            }
            else
            {
                currentGameData.levelStars.Add(levelId, stars);
                currentGameData.totalStars += stars;
            }
        }
        
        public int GetLevelStars(string levelId)
        {
            if (currentGameData.levelStars.ContainsKey(levelId))
            {
                return currentGameData.levelStars[levelId];
            }
            return 0;
        }
        
        public void SetLevelCompleted(string levelId, bool completed)
        {
            if (currentGameData.levelCompleted.ContainsKey(levelId))
            {
                currentGameData.levelCompleted[levelId] = completed;
            }
            else
            {
                currentGameData.levelCompleted.Add(levelId, completed);
            }
            
            if (completed && !currentGameData.levelCompleted.ContainsValue(false))
            {
                currentGameData.completedLevels++;
            }
        }
        
        public bool IsLevelCompleted(string levelId)
        {
            if (currentGameData.levelCompleted.ContainsKey(levelId))
            {
                return currentGameData.levelCompleted[levelId];
            }
            return false;
        }
        
        public void UnlockAchievement(string achievementId)
        {
            if (!currentGameData.unlockedAchievements.Contains(achievementId))
            {
                currentGameData.unlockedAchievements.Add(achievementId);
            }
        }
        
        public bool IsAchievementUnlocked(string achievementId)
        {
            return currentGameData.unlockedAchievements.Contains(achievementId);
        }
        
        public void IncrementCommandsExecuted()
        {
            currentGameData.totalCommandsExecuted++;
        }
        
        public void IncrementBlocksPlaced()
        {
            currentGameData.totalBlocksPlaced++;
        }
        
        public void IncrementRestarts()
        {
            currentGameData.totalRestarts++;
        }
    }
}