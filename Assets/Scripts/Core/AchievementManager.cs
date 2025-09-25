using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }
        
        [System.Serializable]
        public class Achievement
        {
            public string id;
            public string name;
            public string description;
            public bool isUnlocked;
            public int progress;
            public int targetProgress;
        }
        
        [Header("Achievements")]
        [SerializeField] private Achievement[] achievements;
        
        private Dictionary<string, Achievement> achievementDictionary;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAchievements();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeAchievements()
        {
            achievementDictionary = new Dictionary<string, Achievement>();
            
            foreach (var achievement in achievements)
            {
                achievementDictionary[achievement.id] = achievement;
                LoadAchievementProgress(achievement);
            }
        }
        
        private void LoadAchievementProgress(Achievement achievement)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Unlocked", 0) == 1;
            achievement.progress = PlayerPrefs.GetInt($"Achievement_{achievement.id}_Progress", 0);
        }
        
        public void UnlockAchievement(string id)
        {
            if (achievementDictionary.TryGetValue(id, out Achievement achievement))
            {
                if (!achievement.isUnlocked)
                {
                    achievement.isUnlocked = true;
                    PlayerPrefs.SetInt($"Achievement_{id}_Unlocked", 1);
                    PlayerPrefs.Save();
                    
                    Debug.Log($"Достижение разблокировано: {achievement.name}");
                    // Here you could trigger UI notifications, sounds, etc.
                }
            }
        }
        
        public void AddProgressToAchievement(string id, int progress)
        {
            if (achievementDictionary.TryGetValue(id, out Achievement achievement))
            {
                if (!achievement.isUnlocked)
                {
                    achievement.progress += progress;
                    PlayerPrefs.SetInt($"Achievement_{id}_Progress", achievement.progress);
                    
                    if (achievement.progress >= achievement.targetProgress)
                    {
                        UnlockAchievement(id);
                    }
                    
                    PlayerPrefs.Save();
                }
            }
        }
        
        public bool IsAchievementUnlocked(string id)
        {
            if (achievementDictionary.TryGetValue(id, out Achievement achievement))
            {
                return achievement.isUnlocked;
            }
            return false;
        }
        
        public Achievement[] GetAllAchievements()
        {
            return achievements;
        }
        
        // Level-specific achievements
        public void OnLevelCompleted(int levelIndex)
        {
            // Unlock "First Level Completed" achievement
            if (levelIndex == 1)
            {
                UnlockAchievement("first_level");
            }
            
            // Unlock "Ten Levels Completed" achievement
            AddProgressToAchievement("ten_levels", 1);
            
            // Unlock "All Levels Completed" achievement
            // You would need to check the total number of levels
            // AddProgressToAchievement("all_levels", 1);
        }
    }
}