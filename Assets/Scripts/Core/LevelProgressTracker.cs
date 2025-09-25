using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class LevelProgressTracker : MonoBehaviour
    {
        public static LevelProgressTracker Instance { get; private set; }
        
        [System.Serializable]
        public class LevelProgress
        {
            public int levelIndex;
            public bool isCompleted;
            public int starsEarned;
            public int bestCommandCount;
            public float bestTime;
            public int attempts;
            public System.DateTime lastPlayed;
        }
        
        [Header("Progress Settings")]
        [SerializeField] private string playerPrefsKey = "LevelProgress";
        [SerializeField] private int totalLevels = 15;
        
        private Dictionary<int, LevelProgress> levelProgress = new Dictionary<int, LevelProgress>();
        
        public System.Action<int, int> OnLevelCompleted; // levelIndex, stars
        public System.Action OnProgressLoaded;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadProgress();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Record level completion
        public void RecordLevelCompletion(int levelIndex, int stars, int commandCount, float time)
        {
            if (!levelProgress.ContainsKey(levelIndex))
            {
                levelProgress[levelIndex] = new LevelProgress { levelIndex = levelIndex };
            }
            
            LevelProgress progress = levelProgress[levelIndex];
            bool wasCompleted = progress.isCompleted;
            
            progress.isCompleted = true;
            progress.attempts++;
            progress.lastPlayed = System.DateTime.Now;
            
            // Update stars if better than previous
            if (stars > progress.starsEarned)
            {
                progress.starsEarned = stars;
            }
            
            // Update best command count if better than previous
            if (progress.bestCommandCount == 0 || commandCount < progress.bestCommandCount)
            {
                progress.bestCommandCount = commandCount;
            }
            
            // Update best time if better than previous
            if (progress.bestTime == 0 || time < progress.bestTime)
            {
                progress.bestTime = time;
            }
            
            // Notify listeners if this is a new completion
            if (!wasCompleted)
            {
                OnLevelCompleted?.Invoke(levelIndex, stars);
            }
            
            SaveProgress();
        }
        
        // Get progress for a specific level
        public LevelProgress GetLevelProgress(int levelIndex)
        {
            if (levelProgress.ContainsKey(levelIndex))
            {
                return levelProgress[levelIndex];
            }
            
            return null;
        }
        
        // Check if a level is completed
        public bool IsLevelCompleted(int levelIndex)
        {
            if (levelProgress.ContainsKey(levelIndex))
            {
                return levelProgress[levelIndex].isCompleted;
            }
            
            return false;
        }
        
        // Get stars earned for a level
        public int GetStarsForLevel(int levelIndex)
        {
            if (levelProgress.ContainsKey(levelIndex))
            {
                return levelProgress[levelIndex].starsEarned;
            }
            
            return 0;
        }
        
        // Get total stars earned
        public int GetTotalStars()
        {
            int total = 0;
            foreach (var progress in levelProgress.Values)
            {
                total += progress.starsEarned;
            }
            return total;
        }
        
        // Get completion percentage
        public float GetCompletionPercentage()
        {
            int completedLevels = 0;
            foreach (var progress in levelProgress.Values)
            {
                if (progress.isCompleted)
                {
                    completedLevels++;
                }
            }
            
            return (float)completedLevels / totalLevels * 100f;
        }
        
        // Check if a level is unlocked
        public bool IsLevelUnlocked(int levelIndex)
        {
            // Level 1 is always unlocked
            if (levelIndex == 1) return true;
            
            // Level is unlocked if previous level is completed
            return IsLevelCompleted(levelIndex - 1);
        }
        
        // Reset progress for a specific level
        public void ResetLevelProgress(int levelIndex)
        {
            if (levelProgress.ContainsKey(levelIndex))
            {
                levelProgress.Remove(levelIndex);
                SaveProgress();
            }
        }
        
        // Reset all progress
        public void ResetAllProgress()
        {
            levelProgress.Clear();
            SaveProgress();
        }
        
        // Load progress from PlayerPrefs
        private void LoadProgress()
        {
            string jsonData = PlayerPrefs.GetString(playerPrefsKey, "");
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    LevelProgress[] progressArray = JsonUtility.FromJson<LevelProgress[]>(jsonData);
                    foreach (var progress in progressArray)
                    {
                        levelProgress[progress.levelIndex] = progress;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Failed to load level progress: " + e.Message);
                }
            }
            
            OnProgressLoaded?.Invoke();
        }
        
        // Save progress to PlayerPrefs
        private void SaveProgress()
        {
            LevelProgress[] progressArray = new LevelProgress[levelProgress.Values.Count];
            levelProgress.Values.CopyTo(progressArray, 0);
            
            string jsonData = JsonUtility.ToJson(progressArray);
            PlayerPrefs.SetString(playerPrefsKey, jsonData);
            PlayerPrefs.Save();
        }
        
        // Get list of all level progress
        public LevelProgress[] GetAllLevelProgress()
        {
            LevelProgress[] progressArray = new LevelProgress[levelProgress.Values.Count];
            levelProgress.Values.CopyTo(progressArray, 0);
            return progressArray;
        }
    }
}