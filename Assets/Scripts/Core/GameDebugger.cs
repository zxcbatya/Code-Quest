using UnityEngine;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class GameDebugger : MonoBehaviour
    {
        public static GameDebugger Instance { get; private set; }
        
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugging = true;
        [SerializeField] private bool showConsole = false;
        [SerializeField] private int maxLogEntries = 100;
        
        [Header("Debug Visualization")]
        [SerializeField] private bool showGridOverlay = false;
        [SerializeField] private bool showPathfinding = false;
        [SerializeField] private bool showRobotInfo = false;
        
        private List<string> logEntries = new List<string>();
        private RobotController robotController;
        private LevelManager levelManager;
        
        public System.Action<string> OnLogEntryAdded;
        public System.Action OnDebugSettingsChanged;
        
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
        
        private void Start()
        {
            robotController = RobotController.Instance;
            levelManager = LevelManager.Instance;
        }
        
        // Log a debug message
        public void Log(string message)
        {
            if (!enableDebugging) return;
            
            string logEntry = $"[{System.DateTime.Now:HH:mm:ss}] {message}";
            logEntries.Add(logEntry);
            
            // Limit log size
            if (logEntries.Count > maxLogEntries)
            {
                logEntries.RemoveAt(0);
            }
            
            Debug.Log(message);
            OnLogEntryAdded?.Invoke(logEntry);
        }
        
        // Log a warning
        public void LogWarning(string message)
        {
            if (!enableDebugging) return;
            
            string logEntry = $"[WARNING] [{System.DateTime.Now:HH:mm:ss}] {message}";
            logEntries.Add(logEntry);
            
            // Limit log size
            if (logEntries.Count > maxLogEntries)
            {
                logEntries.RemoveAt(0);
            }
            
            Debug.LogWarning(message);
            OnLogEntryAdded?.Invoke(logEntry);
        }
        
        // Log an error
        public void LogError(string message)
        {
            if (!enableDebugging) return;
            
            string logEntry = $"[ERROR] [{System.DateTime.Now:HH:mm:ss}] {message}";
            logEntries.Add(logEntry);
            
            // Limit log size
            if (logEntries.Count > maxLogEntries)
            {
                logEntries.RemoveAt(0);
            }
            
            Debug.LogError(message);
            OnLogEntryAdded?.Invoke(logEntry);
        }
        
        // Get recent log entries
        public string[] GetRecentLogEntries(int count)
        {
            int startIndex = Mathf.Max(0, logEntries.Count - count);
            int actualCount = Mathf.Min(count, logEntries.Count);
            
            string[] recentEntries = new string[actualCount];
            for (int i = 0; i < actualCount; i++)
            {
                recentEntries[i] = logEntries[startIndex + i];
            }
            
            return recentEntries;
        }
        
        // Clear log entries
        public void ClearLog()
        {
            logEntries.Clear();
        }
        
        // Get robot debug info
        public string GetRobotDebugInfo()
        {
            if (robotController == null) return "Robot controller not found";
            
            Vector2Int position = robotController.GetCurrentPosition();
            int direction = robotController.GetCurrentDirection();
            bool isMoving = robotController.IsMoving();
            bool onGoal = robotController.IsOnGoal();
            
            string directionName = GetDirectionName(direction);
            
            return $"Позиция: ({position.x}, {position.y})\n" +
                   $"Направление: {directionName} ({direction})\n" +
                   $"Движение: {isMoving}\n" +
                   $"На цели: {onGoal}";
        }
        
        // Get level debug info
        public string GetLevelDebugInfo()
        {
            if (levelManager == null) return "Level manager not found";
            
            LevelData currentLevel = levelManager.GetCurrentLevel();
            if (currentLevel == null) return "No current level";
            
            return $"Уровень: {currentLevel.levelName}\n" +
                   $"Индекс: {currentLevel.levelIndex}\n" +
                   $"Сложность: {currentLevel.difficulty}\n" +
                   $"Размер: {currentLevel.gridWidth}x{currentLevel.gridHeight}";
        }
        
        // Get direction name
        private string GetDirectionName(int direction)
        {
            switch (direction)
            {
                case 0: return "Север";
                case 1: return "Восток";
                case 2: return "Юг";
                case 3: return "Запад";
                default: return "Неизвестно";
            }
        }
        
        // Toggle debugging enabled
        public void ToggleDebugging()
        {
            enableDebugging = !enableDebugging;
            OnDebugSettingsChanged?.Invoke();
        }
        
        // Set debugging enabled
        public void SetDebuggingEnabled(bool enabled)
        {
            enableDebugging = enabled;
            OnDebugSettingsChanged?.Invoke();
        }
        
        // Toggle console visibility
        public void ToggleConsole()
        {
            showConsole = !showConsole;
            OnDebugSettingsChanged?.Invoke();
        }
        
        // Set console visibility
        public void SetConsoleVisible(bool visible)
        {
            showConsole = visible;
            OnDebugSettingsChanged?.Invoke();
        }
        
        // Toggle grid overlay
        public void ToggleGridOverlay()
        {
            showGridOverlay = !showGridOverlay;
            OnDebugSettingsChanged?.Invoke();
        }
        
        // Set grid overlay visibility
        public void SetGridOverlayVisible(bool visible)
        {
            showGridOverlay = visible;
            OnDebugSettingsChanged?.Invoke();
        }
        
        // Check if debugging is enabled
        public bool IsDebuggingEnabled()
        {
            return enableDebugging;
        }
        
        // Check if console is visible
        public bool IsConsoleVisible()
        {
            return showConsole;
        }
        
        // Check if grid overlay is visible
        public bool IsGridOverlayVisible()
        {
            return showGridOverlay;
        }
    }
}