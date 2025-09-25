using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class AdvancedRobotBehaviors : MonoBehaviour
    {
        [Header("Robot Reference")]
        [SerializeField] private RobotController robotController;
        
        [Header("Behavior Settings")]
        [SerializeField] private float scanInterval = 1.0f;
        [SerializeField] private float reactionTime = 0.5f;
        
        private Coroutine scanCoroutine;
        private bool isScanning = false;
        
        private void Start()
        {
            if (robotController == null)
                robotController = RobotController.Instance;
        }
        
        // Start automatic environment scanning
        public void StartScanning()
        {
            if (isScanning) return;
            
            isScanning = true;
            scanCoroutine = StartCoroutine(ScanEnvironment());
        }
        
        // Stop automatic environment scanning
        public void StopScanning()
        {
            if (!isScanning) return;
            
            isScanning = false;
            if (scanCoroutine != null)
            {
                StopCoroutine(scanCoroutine);
                scanCoroutine = null;
            }
        }
        
        // Coroutine to continuously scan environment
        private IEnumerator ScanEnvironment()
        {
            while (isScanning)
            {
                ScanSurroundings();
                yield return new WaitForSeconds(scanInterval);
            }
        }
        
        // Scan the robot's immediate surroundings
        private void ScanSurroundings()
        {
            if (robotController == null) return;
            
            Vector2Int currentPosition = robotController.GetCurrentPosition();
            int currentDirection = robotController.GetCurrentDirection();
            
            // Scan in all 4 directions
            for (int i = 0; i < 4; i++)
            {
                Vector2Int directionVector = DirectionToVector(i);
                Vector2Int scanPosition = currentPosition + directionVector;
                
                // In a real implementation, you would check what's at scanPosition
                // and react accordingly
                Debug.Log($"Сканирование позиции {scanPosition} в направлении {i}");
            }
        }
        
        // Helper method to convert direction index to vector
        private Vector2Int DirectionToVector(int dir)
        {
            switch (dir)
            {
                case 0: return Vector2Int.up;    // North
                case 1: return Vector2Int.right; // East
                case 2: return Vector2Int.down;  // South
                case 3: return Vector2Int.left;  // West
                default: return Vector2Int.zero;
            }
        }
        
        // Advanced movement: Move to specific grid position
        public void MoveToPosition(Vector2Int targetPosition)
        {
            if (robotController == null) return;
            
            // In a real implementation, you would:
            // 1. Calculate path to target position
            // 2. Convert path to sequence of commands
            // 3. Execute commands
            
            Debug.Log($"Движение к позиции {targetPosition}");
        }
        
        // Advanced behavior: Follow a path
        public void FollowPath(List<Vector2Int> path)
        {
            if (robotController == null || path == null || path.Count == 0) return;
            
            // In a real implementation, you would:
            // 1. Convert path to sequence of movement commands
            // 2. Execute commands with proper timing
            
            Debug.Log($"Следование по пути из {path.Count} точек");
        }
        
        // Advanced behavior: Avoid obstacles
        public void AvoidObstacles()
        {
            if (robotController == null) return;
            
            // Check if path ahead is clear
            if (robotController.IsPathAhead())
            {
                // Move forward if path is clear
                robotController.MoveForward();
            }
            else
            {
                // Turn randomly to avoid obstacle
                bool turnLeft = Random.Range(0, 2) == 0;
                if (turnLeft)
                {
                    robotController.TurnLeft();
                }
                else
                {
                    robotController.TurnRight();
                }
            }
        }
        
        // Advanced behavior: Find nearest goal
        public void FindNearestGoal(List<Vector2Int> goals)
        {
            if (robotController == null || goals == null || goals.Count == 0) return;
            
            Vector2Int currentPosition = robotController.GetCurrentPosition();
            Vector2Int nearestGoal = goals[0];
            float nearestDistance = Vector2Int.Distance(currentPosition, nearestGoal);
            
            // Find the closest goal
            for (int i = 1; i < goals.Count; i++)
            {
                float distance = Vector2Int.Distance(currentPosition, goals[i]);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestGoal = goals[i];
                }
            }
            
            // Move toward nearest goal
            MoveToPosition(nearestGoal);
        }
        
        // Advanced behavior: Patrol between points
        public void Patrol(List<Vector2Int> patrolPoints)
        {
            if (robotController == null || patrolPoints == null || patrolPoints.Count == 0) return;
            
            // In a real implementation, you would:
            // 1. Cycle through patrol points
            // 2. Move to each point in sequence
            // 3. Wait at each point
            
            Debug.Log($"Патрулирование между {patrolPoints.Count} точками");
        }
        
        private void OnDestroy()
        {
            StopScanning();
        }
    }
}