using UnityEngine;

namespace Core
{
    public class LevelCreator : MonoBehaviour
    {
        [Header("Level Creation")]
        [SerializeField] private string levelName = "New Level";
        [SerializeField] private int levelIndex = 1;
        [SerializeField] private Vector2Int gridSize = new Vector2Int(8, 8);
        
        [Header("Robot Settings")]
        [SerializeField] private Vector2Int startPosition = Vector2Int.zero;
        [SerializeField] private int startDirection = 1; // East
        
        [Header("Goal Settings")]
        [SerializeField] private Vector2Int[] goalPositions;
        
        [Header("Obstacles")]
        [SerializeField] private Vector2Int[] wallPositions;
        
        [Header("Save Settings")]
        [SerializeField] private string savePath = "Assets/Resources/Levels/";
        
        // This method demonstrates how to create a level programmatically
        public LevelData CreateLevel()
        {
            // Create a new LevelData instance
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            
            // Configure basic properties
            level.levelIndex = levelIndex;
            level.levelName = levelName;
            level.description = $"Level {levelIndex}: {levelName}";
            level.difficulty = Mathf.Clamp(levelIndex / 3 + 1, 1, 5);
            level.maxCommands = 10 + levelIndex * 2;
            level.optimalCommands = 5 + levelIndex;
            
            // Configure robot settings
            level.startPosition = startPosition;
            level.startDirection = startDirection;
            
            // Configure goals
            level.goalPositions = goalPositions;
            level.requireAllGoals = false;
            
            // Configure grid
            level.gridWidth = gridSize.x;
            level.gridHeight = gridSize.y;
            level.gridLayout = new LevelData.TileType[gridSize.x, gridSize.y];
            
            // Initialize grid with empty tiles
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    level.gridLayout[x, y] = LevelData.TileType.Empty;
                }
            }
            
            // Place walls
            if (wallPositions != null)
            {
                foreach (Vector2Int pos in wallPositions)
                {
                    if (pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y)
                    {
                        level.gridLayout[pos.x, pos.y] = LevelData.TileType.Wall;
                    }
                }
            }
            
            // Place goals
            if (goalPositions != null)
            {
                foreach (Vector2Int pos in goalPositions)
                {
                    if (pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y)
                    {
                        level.gridLayout[pos.x, pos.y] = LevelData.TileType.Goal;
                    }
                }
            }
            
            // Enable basic commands based on level index
            level.allowMoveForward = true;
            level.allowTurnLeft = levelIndex >= 1;
            level.allowTurnRight = levelIndex >= 1;
            level.allowJump = levelIndex >= 5;
            level.allowInteract = levelIndex >= 8;
            level.allowRepeat = levelIndex >= 10;
            level.allowIf = levelIndex >= 12;
            level.allowElse = levelIndex >= 14;
            
            // Serialize the grid
            level.SerializeGrid();
            
            return level;
        }
        
        // Example method to create a simple level
        public LevelData CreateSimpleLevel()
        {
            levelName = "Simple Path";
            levelIndex = 1;
            startPosition = new Vector2Int(0, 0);
            startDirection = 1; // East
            goalPositions = new Vector2Int[] { new Vector2Int(7, 0) };
            wallPositions = new Vector2Int[] { 
                new Vector2Int(2, 0), 
                new Vector2Int(3, 0), 
                new Vector2Int(4, 0), 
                new Vector2Int(5, 0) 
            };
            
            return CreateLevel();
        }
        
        // Example method to create a maze level
        public LevelData CreateMazeLevel()
        {
            levelName = "Maze Challenge";
            levelIndex = 5;
            startPosition = new Vector2Int(0, 0);
            startDirection = 1; // East
            goalPositions = new Vector2Int[] { new Vector2Int(7, 7) };
            wallPositions = GenerateMazeWalls();
            
            return CreateLevel();
        }
        
        private Vector2Int[] GenerateMazeWalls()
        {
            // Simple maze pattern
            var walls = new System.Collections.Generic.List<Vector2Int>();
            
            // Vertical walls
            for (int y = 1; y < gridSize.y - 1; y++)
            {
                walls.Add(new Vector2Int(2, y));
                walls.Add(new Vector2Int(5, y));
            }
            
            // Horizontal walls to create paths
            walls.Add(new Vector2Int(3, 2));
            walls.Add(new Vector2Int(4, 2));
            walls.Add(new Vector2Int(3, 5));
            walls.Add(new Vector2Int(4, 5));
            
            return walls.ToArray();
        }
        
        // This method would save the level to a file (editor only)
        #if UNITY_EDITOR
        public void SaveLevel(LevelData level, string fileName)
        {
            string path = savePath + fileName + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(level, path);
            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log($"Level saved to {path}");
        }
        #endif
    }
}