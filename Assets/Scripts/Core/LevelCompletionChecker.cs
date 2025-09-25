using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class LevelCompletionChecker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RobotController robotController;
        [SerializeField] private LevelManager levelManager;
        
        [Header("Check Settings")]
        [SerializeField] private float checkInterval = 0.5f;
        [SerializeField] private bool autoCheck = true;
        
        private float lastCheckTime = 0f;
        
        private void Start()
        {
            if (robotController == null)
                robotController = RobotController.Instance;
                
            if (levelManager == null)
                levelManager = LevelManager.Instance;
        }
        
        private void Update()
        {
            if (!autoCheck) return;
            
            // Check level completion at intervals
            if (Time.time - lastCheckTime > checkInterval)
            {
                CheckLevelCompletion();
                lastCheckTime = Time.time;
            }
        }
        
        public void CheckLevelCompletion()
        {
            if (robotController == null || levelManager == null) return;
            
            // Only check if the robot is not moving
            if (!robotController.IsMoving())
            {
                levelManager.CheckLevelCompletion();
            }
        }
        
        // Manual check for level completion
        public bool IsLevelCompleted()
        {
            if (robotController == null) return false;
            
            return robotController.IsOnGoal();
        }
        
        // Check if all goals are reached (for levels with multiple goals)
        public bool AreAllGoalsReached(LevelData levelData)
        {
            if (robotController == null || levelData == null) return false;
            
            Vector2Int robotPosition = robotController.GetCurrentPosition();
            
            if (levelData.goalPositions == null || levelData.goalPositions.Length == 0)
            {
                return false;
            }
            
            // If requireAllGoals is true, check if robot is on any goal
            // In a more complex implementation, you would track which goals have been reached
            foreach (Vector2Int goal in levelData.goalPositions)
            {
                if (robotPosition == goal)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        // Get completion percentage (for levels with multiple objectives)
        public float GetCompletionPercentage(LevelData levelData)
        {
            if (levelData == null) return 0f;
            
            // Simple implementation - either 0% or 100%
            return IsLevelCompleted() ? 100f : 0f;
        }
        
        // Set auto-check enabled/disabled
        public void SetAutoCheck(bool enabled)
        {
            autoCheck = enabled;
        }
        
        // Set check interval
        public void SetCheckInterval(float interval)
        {
            checkInterval = Mathf.Max(0.1f, interval);
        }
    }
}