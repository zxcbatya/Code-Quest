using UnityEngine;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class GameAnalytics : MonoBehaviour
    {
        public static GameAnalytics Instance { get; private set; }
        
        [System.Serializable]
        public class LevelAnalytics
        {
            public int levelIndex;
            public int timesPlayed;
            public int timesCompleted;
            public float totalTimePlayed;
            public int totalCommandsUsed;
            public float averageCompletionTime;
            public int[] starDistribution = new int[4]; // 0, 1, 2, 3 stars
        }
        
        [System.Serializable]
        public class SessionAnalytics
        {
            public System.DateTime sessionStart;
            public System.DateTime sessionEnd;
            public float sessionDuration;
            public int levelsPlayed;
            public int levelsCompleted;
            public int totalCommandsUsed;
        }
        
        [Header("Analytics Settings")]
        [SerializeField] private bool enableAnalytics = true;
        [SerializeField] private string playerPrefsKey = "GameAnalytics";
        [SerializeField] private int maxSessionsTracked = 10;
        
        private Dictionary<int, LevelAnalytics> levelAnalytics = new Dictionary<int, LevelAnalytics>();
        private List<SessionAnalytics> sessionAnalytics = new List<SessionAnalytics>();
        private SessionAnalytics currentSession;
        
        private RobotController robotController;
        private LevelManager levelManager;
        
        public System.Action<LevelAnalytics> OnLevelAnalyticsUpdated;
        public System.Action<SessionAnalytics> OnSessionAnalyticsUpdated;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAnalytics();
                StartNewSession();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            robotController = RobotController.Instance;
            levelManager = LevelManager.Instance;
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                EndCurrentSession();
            }
            else
            {
                StartNewSession();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                EndCurrentSession();
            }
            else
            {
                StartNewSession();
            }
        }
        
        private void OnApplicationQuit()
        {
            EndCurrentSession();
            SaveAnalytics();
        }
        
        // Start a new analytics session
        private void StartNewSession()
        {
            if (!enableAnalytics) return;
            
            currentSession = new SessionAnalytics
            {
                sessionStart = System.DateTime.Now,
                sessionDuration = 0f,
                levelsPlayed = 0,
                levelsCompleted = 0,
                totalCommandsUsed = 0
            };
        }
        
        // End the current analytics session
        private void EndCurrentSession()
        {
            if (!enableAnalytics || currentSession == null) return;
            
            currentSession.sessionEnd = System.DateTime.Now;
            currentSession.sessionDuration = (float)(currentSession.sessionEnd - currentSession.sessionStart).TotalSeconds;
            
            sessionAnalytics.Add(currentSession);
            OnSessionAnalyticsUpdated?.Invoke(currentSession);
            
            // Limit tracked sessions
            while (sessionAnalytics.Count > maxSessionsTracked)
            {
                sessionAnalytics.RemoveAt(0);
            }
        }
        
        // Record level start
        public void RecordLevelStart(int levelIndex)
        {
            if (!enableAnalytics) return;
            
            if (!levelAnalytics.ContainsKey(levelIndex))
            {
                levelAnalytics[levelIndex] = new LevelAnalytics { levelIndex = levelIndex };
            }
            
            levelAnalytics[levelIndex].timesPlayed++;
            currentSession.levelsPlayed++;
        }
        
        // Record level completion
        public void RecordLevelCompletion(int levelIndex, int stars, int commandsUsed, float timeTaken)
        {
            if (!enableAnalytics) return;
            
            if (!levelAnalytics.ContainsKey(levelIndex))
            {
                levelAnalytics[levelIndex] = new LevelAnalytics { levelIndex = levelIndex };
            }
            
            LevelAnalytics analytics = levelAnalytics[levelIndex];
            analytics.timesCompleted++;
            analytics.totalTimePlayed += timeTaken;
            analytics.totalCommandsUsed += commandsUsed;
            analytics.averageCompletionTime = analytics.totalTimePlayed / analytics.timesCompleted;
            
            // Update star distribution
            if (stars >= 0 && stars <= 3)
            {
                analytics.starDistribution[stars]++;
            }
            
            currentSession.levelsCompleted++;
            currentSession.totalCommandsUsed += commandsUsed;
            
            OnLevelAnalyticsUpdated?.Invoke(analytics);
        }
        
        // Record command usage
        public void RecordCommandUsage(CommandType commandType)
        {
            if (!enableAnalytics) return;
            
            // In a more detailed implementation, you might track specific command usage
            // For now, we'll just increment the session command count
            currentSession.totalCommandsUsed++;
        }
        
        // Get analytics for a specific level
        public LevelAnalytics GetLevelAnalytics(int levelIndex)
        {
            if (levelAnalytics.ContainsKey(levelIndex))
            {
                return levelAnalytics[levelIndex];
            }
            
            return null;
        }
        
        // Get all level analytics
        public LevelAnalytics[] GetAllLevelAnalytics()
        {
            LevelAnalytics[] analyticsArray = new LevelAnalytics[levelAnalytics.Values.Count];
            levelAnalytics.Values.CopyTo(analyticsArray, 0);
            return analyticsArray;
        }
        
        // Get recent session analytics
        public SessionAnalytics[] GetRecentSessions(int count)
        {
            int startIndex = Mathf.Max(0, sessionAnalytics.Count - count);
            int actualCount = Mathf.Min(count, sessionAnalytics.Count);
            
            SessionAnalytics[] recentSessions = new SessionAnalytics[actualCount];
            for (int i = 0; i < actualCount; i++)
            {
                recentSessions[i] = sessionAnalytics[startIndex + i];
            }
            
            return recentSessions;
        }
        
        // Get total play time
        public float GetTotalPlayTime()
        {
            float total = 0f;
            foreach (SessionAnalytics session in sessionAnalytics)
            {
                total += session.sessionDuration;
            }
            if (currentSession != null)
            {
                total += (float)(System.DateTime.Now - currentSession.sessionStart).TotalSeconds;
            }
            return total;
        }
        
        // Get total levels completed
        public int GetTotalLevelsCompleted()
        {
            int total = 0;
            foreach (LevelAnalytics analytics in levelAnalytics.Values)
            {
                total += analytics.timesCompleted;
            }
            return total;
        }
        
        // Get total commands used
        public int GetTotalCommandsUsed()
        {
            int total = 0;
            foreach (SessionAnalytics session in sessionAnalytics)
            {
                total += session.totalCommandsUsed;
            }
            if (currentSession != null)
            {
                total += currentSession.totalCommandsUsed;
            }
            return total;
        }
        
        // Save analytics to PlayerPrefs
        private void SaveAnalytics()
        {
            if (!enableAnalytics) return;
            
            // Save level analytics
            string levelData = JsonUtility.ToJson(GetAllLevelAnalytics());
            PlayerPrefs.SetString(playerPrefsKey + "_Levels", levelData);
            
            // Save session analytics
            string sessionData = JsonUtility.ToJson(sessionAnalytics.ToArray());
            PlayerPrefs.SetString(playerPrefsKey + "_Sessions", sessionData);
            
            PlayerPrefs.Save();
        }
        
        // Load analytics from PlayerPrefs
        private void LoadAnalytics()
        {
            if (!enableAnalytics) return;
            
            // Load level analytics
            string levelData = PlayerPrefs.GetString(playerPrefsKey + "_Levels", "");
            if (!string.IsNullOrEmpty(levelData))
            {
                try
                {
                    LevelAnalytics[] loadedLevels = JsonUtility.FromJson<LevelAnalytics[]>(levelData);
                    foreach (LevelAnalytics level in loadedLevels)
                    {
                        levelAnalytics[level.levelIndex] = level;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Failed to load level analytics: " + e.Message);
                }
            }
            
            // Load session analytics
            string sessionData = PlayerPrefs.GetString(playerPrefsKey + "_Sessions", "");
            if (!string.IsNullOrEmpty(sessionData))
            {
                try
                {
                    SessionAnalytics[] loadedSessions = JsonUtility.FromJson<SessionAnalytics[]>(sessionData);
                    sessionAnalytics.Clear();
                    sessionAnalytics.AddRange(loadedSessions);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Failed to load session analytics: " + e.Message);
                }
            }
        }
        
        // Reset all analytics
        public void ResetAnalytics()
        {
            levelAnalytics.Clear();
            sessionAnalytics.Clear();
            StartNewSession();
        }
    }
}