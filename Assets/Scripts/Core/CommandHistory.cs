using UnityEngine;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class CommandHistory : MonoBehaviour
    {
        public static CommandHistory Instance { get; private set; }
        
        [System.Serializable]
        public class CommandRecord
        {
            public CommandType commandType;
            public Vector2Int robotPosition;
            public int robotDirection;
            public System.DateTime timestamp;
            public bool wasSuccessful;
        }
        
        [Header("History Settings")]
        [SerializeField] private int maxHistorySize = 1000;
        [SerializeField] private bool recordHistory = true;
        
        private List<CommandRecord> commandHistory = new List<CommandRecord>();
        private RobotController robotController;
        
        public System.Action<CommandRecord> OnCommandRecorded;
        public System.Action OnHistoryCleared;
        
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
        }
        
        // Record a command execution
        public void RecordCommand(CommandType commandType, bool successful)
        {
            if (!recordHistory) return;
            
            CommandRecord record = new CommandRecord
            {
                commandType = commandType,
                robotPosition = robotController ? robotController.GetCurrentPosition() : Vector2Int.zero,
                robotDirection = robotController ? robotController.GetCurrentDirection() : 0,
                timestamp = System.DateTime.Now,
                wasSuccessful = successful
            };
            
            commandHistory.Add(record);
            
            // Limit history size
            if (commandHistory.Count > maxHistorySize)
            {
                commandHistory.RemoveAt(0);
            }
            
            OnCommandRecorded?.Invoke(record);
        }
        
        // Get the last N commands
        public CommandRecord[] GetRecentCommands(int count)
        {
            int startIndex = Mathf.Max(0, commandHistory.Count - count);
            int actualCount = Mathf.Min(count, commandHistory.Count);
            
            CommandRecord[] recentCommands = new CommandRecord[actualCount];
            for (int i = 0; i < actualCount; i++)
            {
                recentCommands[i] = commandHistory[startIndex + i];
            }
            
            return recentCommands;
        }
        
        // Get all command history
        public CommandRecord[] GetAllCommands()
        {
            return commandHistory.ToArray();
        }
        
        // Get command statistics
        public Dictionary<CommandType, int> GetCommandStatistics()
        {
            Dictionary<CommandType, int> stats = new Dictionary<CommandType, int>();
            
            foreach (CommandRecord record in commandHistory)
            {
                if (stats.ContainsKey(record.commandType))
                {
                    stats[record.commandType]++;
                }
                else
                {
                    stats[record.commandType] = 1;
                }
            }
            
            return stats;
        }
        
        // Get success rate for a specific command type
        public float GetCommandSuccessRate(CommandType commandType)
        {
            int total = 0;
            int successful = 0;
            
            foreach (CommandRecord record in commandHistory)
            {
                if (record.commandType == commandType)
                {
                    total++;
                    if (record.wasSuccessful)
                    {
                        successful++;
                    }
                }
            }
            
            return total > 0 ? (float)successful / total : 0f;
        }
        
        // Clear command history
        public void ClearHistory()
        {
            commandHistory.Clear();
            OnHistoryCleared?.Invoke();
        }
        
        // Enable or disable history recording
        public void SetRecordingEnabled(bool enabled)
        {
            recordHistory = enabled;
        }
        
        // Set maximum history size
        public void SetMaxHistorySize(int maxSize)
        {
            maxHistorySize = Mathf.Max(1, maxSize);
            
            // Trim history if necessary
            while (commandHistory.Count > maxHistorySize)
            {
                commandHistory.RemoveAt(0);
            }
        }
        
        // Export history to JSON
        public string ExportHistoryToJson()
        {
            return JsonUtility.ToJson(commandHistory.ToArray());
        }
        
        // Import history from JSON
        public void ImportHistoryFromJson(string jsonData)
        {
            try
            {
                CommandRecord[] importedHistory = JsonUtility.FromJson<CommandRecord[]>(jsonData);
                commandHistory.Clear();
                commandHistory.AddRange(importedHistory);
                
                // Ensure we don't exceed max size
                while (commandHistory.Count > maxHistorySize)
                {
                    commandHistory.RemoveAt(0);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to import command history: " + e.Message);
            }
        }
    }
}