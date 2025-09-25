using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        
        [System.Serializable]
        public class GameData
        {
            public int currentLevel = 1;
            public int coins = 0;
            public int stars = 0;
            public Dictionary<string, bool> achievements = new Dictionary<string, bool>();
            public Dictionary<string, int> levelStars = new Dictionary<string, int>();
            public Dictionary<string, bool> purchasedItems = new Dictionary<string, bool>();
        }
        
        private GameData currentData = new GameData();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void SaveGame()
        {
            // Save to PlayerPrefs for simplicity
            PlayerPrefs.SetInt("CurrentLevel", currentData.currentLevel);
            PlayerPrefs.SetInt("Coins", currentData.coins);
            PlayerPrefs.SetInt("Stars", currentData.stars);
            
            // Save achievements
            foreach (var achievement in currentData.achievements)
            {
                PlayerPrefs.SetInt($"Achievement_{achievement.Key}", achievement.Value ? 1 : 0);
            }
            
            // Save level stars
            foreach (var levelStar in currentData.levelStars)
            {
                PlayerPrefs.SetInt($"LevelStar_{levelStar.Key}", levelStar.Value);
            }
            
            // Save purchased items
            foreach (var item in currentData.purchasedItems)
            {
                PlayerPrefs.SetInt($"PurchasedItem_{item.Key}", item.Value ? 1 : 0);
            }
            
            PlayerPrefs.Save();
            Debug.Log("Игра сохранена");
        }
        
        public void LoadGame()
        {
            currentData.currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            currentData.coins = PlayerPrefs.GetInt("Coins", 0);
            currentData.stars = PlayerPrefs.GetInt("Stars", 0);
            
            // Load achievements
            // In a real implementation, you would know the achievement IDs
            // For now, we'll leave the dictionary empty
            
            // Load level stars
            // In a real implementation, you would know the level IDs
            // For now, we'll leave the dictionary empty
            
            // Load purchased items
            // In a real implementation, you would know the item IDs
            // For now, we'll leave the dictionary empty
            
            Debug.Log("Игра загружена");
        }
        
        public void SetCurrentLevel(int level)
        {
            currentData.currentLevel = level;
        }
        
        public int GetCurrentLevel()
        {
            return currentData.currentLevel;
        }
        
        public void AddCoins(int amount)
        {
            currentData.coins += amount;
        }
        
        public int GetCoins()
        {
            return currentData.coins;
        }
        
        public void AddStars(int amount)
        {
            currentData.stars += amount;
        }
        
        public int GetStars()
        {
            return currentData.stars;
        }
        
        public void UnlockAchievement(string id)
        {
            currentData.achievements[id] = true;
        }
        
        public bool IsAchievementUnlocked(string id)
        {
            return currentData.achievements.ContainsKey(id) && currentData.achievements[id];
        }
        
        public void SetLevelStars(string levelId, int stars)
        {
            if (currentData.levelStars.ContainsKey(levelId))
            {
                if (stars > currentData.levelStars[levelId])
                {
                    currentData.levelStars[levelId] = stars;
                }
            }
            else
            {
                currentData.levelStars[levelId] = stars;
            }
        }
        
        public int GetLevelStars(string levelId)
        {
            return currentData.levelStars.ContainsKey(levelId) ? currentData.levelStars[levelId] : 0;
        }
        
        public void SetItemPurchased(string itemId, bool purchased)
        {
            currentData.purchasedItems[itemId] = purchased;
        }
        
        public bool IsItemPurchased(string itemId)
        {
            return currentData.purchasedItems.ContainsKey(itemId) && currentData.purchasedItems[itemId];
        }
    }
}