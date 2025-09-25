using UnityEngine;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class ConditionalLogic : MonoBehaviour
    {
        [Header("Robot Reference")]
        [SerializeField] private RobotController robotController;
        [SerializeField] private GridManager gridManager;
        
        private void Start()
        {
            if (robotController == null)
                robotController = RobotController.Instance;
                
            if (gridManager == null)
                gridManager = GridManager.Instance;
        }
        
        // Check if path is clear ahead
        public bool IsPathClearAhead()
        {
            if (robotController == null) return false;
            return robotController.IsPathAhead();
        }
        
        // Check if there's a wall ahead
        public bool IsWallAhead()
        {
            if (robotController == null) return false;
            return robotController.IsWallAhead();
        }
        
        // Check if robot is on a goal
        public bool IsOnGoal()
        {
            if (robotController == null) return false;
            return robotController.IsOnGoal();
        }
        
        // Check if there are items nearby
        public bool AreItemsNearby()
        {
            if (robotController == null) return false;
            return robotController.IsItemNearby();
        }
        
        // Check if robot is at a specific position
        public bool IsAtPosition(Vector2Int position)
        {
            if (robotController == null) return false;
            return robotController.GetCurrentPosition() == position;
        }
        
        // Check if robot is facing a specific direction
        public bool IsFacingDirection(int direction)
        {
            if (robotController == null) return false;
            return robotController.GetCurrentDirection() == direction;
        }
        
        // Check if a specific tile is walkable
        public bool IsTileWalkable(Vector2Int position)
        {
            if (gridManager == null) return false;
            
            LevelData.TileType tileType = gridManager.GetTileType(position.x, position.y);
            return tileType != LevelData.TileType.Wall && 
                   tileType != LevelData.TileType.Pit;
        }
        
        // Check if a specific tile contains an item
        public bool IsTileItem(Vector2Int position)
        {
            if (gridManager == null) return false;
            
            LevelData.TileType tileType = gridManager.GetTileType(position.x, position.y);
            return tileType == LevelData.TileType.Button || 
                   tileType == LevelData.TileType.Key;
        }
        
        // Check if a specific tile is a goal
        public bool IsTileGoal(Vector2Int position)
        {
            if (gridManager == null) return false;
            
            LevelData.TileType tileType = gridManager.GetTileType(position.x, position.y);
            return tileType == LevelData.TileType.Goal;
        }
        
        // Get list of adjacent walkable tiles
        public List<Vector2Int> GetWalkableNeighbors()
        {
            List<Vector2Int> walkableNeighbors = new List<Vector2Int>();
            
            if (robotController == null || gridManager == null) return walkableNeighbors;
            
            Vector2Int currentPosition = robotController.GetCurrentPosition();
            
            // Check all 4 directions
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // North
                new Vector2Int(1, 0),   // East
                new Vector2Int(0, -1),  // South
                new Vector2Int(-1, 0)   // West
            };
            
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = currentPosition + direction;
                
                if (IsTileWalkable(neighbor))
                {
                    walkableNeighbors.Add(neighbor);
                }
            }
            
            return walkableNeighbors;
        }
        
        // Get list of adjacent items
        public List<Vector2Int> GetAdjacentItems()
        {
            List<Vector2Int> adjacentItems = new List<Vector2Int>();
            
            if (robotController == null || gridManager == null) return adjacentItems;
            
            Vector2Int currentPosition = robotController.GetCurrentPosition();
            
            // Check all 4 directions
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // North
                new Vector2Int(1, 0),   // East
                new Vector2Int(0, -1),  // South
                new Vector2Int(-1, 0)   // West
            };
            
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = currentPosition + direction;
                
                if (IsTileItem(neighbor))
                {
                    adjacentItems.Add(neighbor);
                }
            }
            
            return adjacentItems;
        }
        
        // Count the number of walls around the robot
        public int CountWallsAround()
        {
            int wallCount = 0;
            
            if (robotController == null || gridManager == null) return wallCount;
            
            Vector2Int currentPosition = robotController.GetCurrentPosition();
            
            // Check all 4 directions
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // North
                new Vector2Int(1, 0),   // East
                new Vector2Int(0, -1),  // South
                new Vector2Int(-1, 0)   // West
            };
            
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = currentPosition + direction;
                
                LevelData.TileType tileType = gridManager.GetTileType(neighbor.x, neighbor.y);
                if (tileType == LevelData.TileType.Wall)
                {
                    wallCount++;
                }
            }
            
            return wallCount;
        }
        
        // Check if robot is in a corner (2 or more adjacent walls)
        public bool IsInCorner()
        {
            return CountWallsAround() >= 2;
        }
        
        // Check if robot is at the edge of the grid
        public bool IsAtGridEdge()
        {
            if (robotController == null || gridManager == null) return false;
            
            Vector2Int position = robotController.GetCurrentPosition();
            int gridWidth = 8; // Default value, should be obtained from level data
            int gridHeight = 8; // Default value, should be obtained from level data
            
            return position.x == 0 || position.x == gridWidth - 1 ||
                   position.y == 0 || position.y == gridHeight - 1;
        }
    }
}