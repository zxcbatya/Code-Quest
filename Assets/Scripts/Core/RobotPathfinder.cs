using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class RobotPathfinder : MonoBehaviour
    {
        public static RobotPathfinder Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Simple pathfinding using BFS (Breadth-First Search)
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, LevelData levelData)
        {
            if (levelData == null) return null;
            
            // Check if start and goal are valid
            if (!IsValidPosition(start, levelData) || !IsValidPosition(goal, levelData))
            {
                return null;
            }
            
            // If start equals goal, return empty path
            if (start == goal)
            {
                return new List<Vector2Int>();
            }
            
            // BFS setup
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            
            queue.Enqueue(start);
            visited.Add(start);
            cameFrom[start] = start;
            
            // Directions: North, East, South, West
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // North
                new Vector2Int(1, 0),   // East
                new Vector2Int(0, -1),  // South
                new Vector2Int(-1, 0)   // West
            };
            
            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                
                // Check if we reached the goal
                if (current == goal)
                {
                    return ReconstructPath(cameFrom, start, goal);
                }
                
                // Explore neighbors
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = current + direction;
                    
                    // Check if neighbor is valid and not visited
                    if (IsValidPosition(neighbor, levelData) && !visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }
            
            // No path found
            return null;
        }
        
        private bool IsValidPosition(Vector2Int position, LevelData levelData)
        {
            // Check bounds
            if (position.x < 0 || position.x >= levelData.gridWidth ||
                position.y < 0 || position.y >= levelData.gridHeight)
            {
                return false;
            }
            
            // Check if position is walkable
            LevelData.TileType tileType = levelData.GetTile(position.x, position.y);
            return tileType != LevelData.TileType.Wall && 
                   tileType != LevelData.TileType.Pit;
        }
        
        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, 
                                               Vector2Int start, Vector2Int goal)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int current = goal;
            
            // Reconstruct path from goal to start
            while (current != start)
            {
                path.Add(current);
                current = cameFrom[current];
            }
            
            // Reverse to get path from start to goal
            path.Reverse();
            return path;
        }
        
        // Check if position is adjacent to current position
        public bool IsAdjacent(Vector2Int current, Vector2Int target)
        {
            Vector2Int diff = target - current;
            return (Mathf.Abs(diff.x) + Mathf.Abs(diff.y)) == 1;
        }
        
        // Get direction from current to target position
        public int GetDirection(Vector2Int current, Vector2Int target)
        {
            Vector2Int diff = target - current;
            
            if (diff.x == 0 && diff.y == 1) return 0; // North
            if (diff.x == 1 && diff.y == 0) return 1; // East
            if (diff.x == 0 && diff.y == -1) return 2; // South
            if (diff.x == -1 && diff.y == 0) return 3; // West
            
            return -1; // Invalid direction
        }
        
        // Get position in front of current position based on direction
        public Vector2Int GetForwardPosition(Vector2Int position, int direction)
        {
            switch (direction)
            {
                case 0: return position + new Vector2Int(0, 1); // North
                case 1: return position + new Vector2Int(1, 0); // East
                case 2: return position + new Vector2Int(0, -1); // South
                case 3: return position + new Vector2Int(-1, 0); // West
                default: return position;
            }
        }
    }
}