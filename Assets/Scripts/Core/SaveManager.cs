using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core
{
    public static class SaveManager
    {
        // Система сохранения прогресса игрока
        
        public static void SavePlayerProgress()
        {
            // Сохраняем прогресс в PlayerPrefs
            PlayerPrefs.Save();
        }
        
        public static void LoadPlayerProgress()
        {
            // Загрузка происходит автоматически при обращении к PlayerPrefs
        }
        
        public static void SaveLevelProgress(int levelIndex, int stars)
        {
            string progressKey = $"Level_{levelIndex}_Stars";
            int currentStars = PlayerPrefs.GetInt(progressKey, 0);
            
            // Сохраняем только лучший результат
            if (stars > currentStars)
            {
                PlayerPrefs.SetInt(progressKey, stars);
                PlayerPrefs.Save();
            }
        }
        
        public static int LoadLevelProgress(int levelIndex)
        {
            string progressKey = $"Level_{levelIndex}_Stars";
            return PlayerPrefs.GetInt(progressKey, 0);
        }
        
        public static void SaveUnlockedLevels(int unlockedCount)
        {
            PlayerPrefs.SetInt("LevelProgress", unlockedCount);
            PlayerPrefs.Save();
        }
        
        public static int LoadUnlockedLevels()
        {
            return PlayerPrefs.GetInt("LevelProgress", 1);
        }
        
        public static void SaveSettings(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
        
        public static string LoadSettings(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        
        public static void SaveSettings(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }
        
        public static float LoadSettings(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        
        public static void SaveSettings(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }
        
        public static int LoadSettings(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        
        public static void ResetAllProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}