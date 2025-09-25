using UnityEngine;

namespace Core
{
    public class LevelProgression : MonoBehaviour
    {
        private const string LEVEL_PROGRESS_KEY = "LevelProgress";
        private const string LEVEL_STARS_KEY = "LevelStars_";
        
        public static LevelProgression Instance { get; private set; }
        
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
        
        public void UnlockLevel(int levelIndex)
        {
            int currentProgress = PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 1);
            if (levelIndex > currentProgress)
            {
                PlayerPrefs.SetInt(LEVEL_PROGRESS_KEY, levelIndex);
                PlayerPrefs.Save();
            }
        }
        
        public bool IsLevelUnlocked(int levelIndex)
        {
            int currentProgress = PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 1);
            return levelIndex <= currentProgress;
        }
        
        public int GetUnlockedLevelCount()
        {
            return PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 1);
        }
        
        public void SaveLevelStars(int levelIndex, int stars)
        {
            string key = LEVEL_STARS_KEY + levelIndex;
            int currentStars = PlayerPrefs.GetInt(key, 0);
            
            if (stars > currentStars)
            {
                PlayerPrefs.SetInt(key, stars);
                PlayerPrefs.Save();
            }
        }
        
        public int GetLevelStars(int levelIndex)
        {
            string key = LEVEL_STARS_KEY + levelIndex;
            return PlayerPrefs.GetInt(key, 0);
        }
        
        public int GetTotalStars()
        {
            int total = 0;
            int unlockedLevels = GetUnlockedLevelCount();
            
            for (int i = 1; i <= unlockedLevels; i++)
            {
                total += GetLevelStars(i);
            }
            
            return total;
        }
    }
}