using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public bool isUnlocked;
        public int progress;
        public int targetProgress;
    }
    
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }
        
        [Header("Achievements")]
        [SerializeField] private List<Achievement> achievements = new List<Achievement>();
        
        private const string ACHIEVEMENT_PREFIX = "Achievement_";
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAchievements();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void LoadAchievements()
        {
            // Инициализируем достижения, если их нет
            if (achievements.Count == 0)
            {
                InitializeDefaultAchievements();
            }
            
            // Загружаем прогресс достижений
            foreach (Achievement achievement in achievements)
            {
                string key = ACHIEVEMENT_PREFIX + achievement.id;
                achievement.isUnlocked = SaveManager.LoadSettings(key, 0) == 1;
                
                string progressKey = ACHIEVEMENT_PREFIX + achievement.id + "_Progress";
                achievement.progress = SaveManager.LoadSettings(progressKey, 0);
            }
        }
        
        private void InitializeDefaultAchievements()
        {
            achievements.Add(new Achievement
            {
                id = "first_level",
                title = "Первый шаг",
                description = "Пройдите первый уровень",
                targetProgress = 1
            });
            
            achievements.Add(new Achievement
            {
                id = "five_levels",
                title = "Пять уровней",
                description = "Пройдите пять уровней",
                targetProgress = 5
            });
            
            achievements.Add(new Achievement
            {
                id = "ten_levels",
                title = "Десять уровней",
                description = "Пройдите десять уровней",
                targetProgress = 10
            });
            
            achievements.Add(new Achievement
            {
                id = "perfect_level",
                title = "Идеальное выполнение",
                description = "Пройдите уровень с 3 звездами",
                targetProgress = 1
            });
            
            achievements.Add(new Achievement
            {
                id = "speed_runner",
                title = "Скоростной бегун",
                description = "Пройдите уровень за минимальное количество команд",
                targetProgress = 1
            });
        }
        
        public void UnlockAchievement(string id)
        {
            Achievement achievement = GetAchievement(id);
            if (achievement != null && !achievement.isUnlocked)
            {
                achievement.isUnlocked = true;
                SaveManager.SaveSettings(ACHIEVEMENT_PREFIX + id, 1);
                
                // Показываем уведомление о достижении
                UI.HintManager.Instance?.ShowHint($"Достижение разблокировано: {achievement.title}");
            }
        }
        
        public void UpdateAchievementProgress(string id, int progress)
        {
            Achievement achievement = GetAchievement(id);
            if (achievement != null && !achievement.isUnlocked)
            {
                achievement.progress = progress;
                SaveManager.SaveSettings(ACHIEVEMENT_PREFIX + id + "_Progress", progress);
                
                // Проверяем, достигнута ли цель
                if (achievement.progress >= achievement.targetProgress)
                {
                    UnlockAchievement(id);
                }
            }
        }
        
        private Achievement GetAchievement(string id)
        {
            foreach (Achievement achievement in achievements)
            {
                if (achievement.id == id)
                {
                    return achievement;
                }
            }
            return null;
        }
        
        public bool IsAchievementUnlocked(string id)
        {
            Achievement achievement = GetAchievement(id);
            return achievement != null && achievement.isUnlocked;
        }
        
        public List<Achievement> GetUnlockedAchievements()
        {
            List<Achievement> unlocked = new List<Achievement>();
            foreach (Achievement achievement in achievements)
            {
                if (achievement.isUnlocked)
                {
                    unlocked.Add(achievement);
                }
            }
            return unlocked;
        }
        
        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(achievements);
        }
    }
}