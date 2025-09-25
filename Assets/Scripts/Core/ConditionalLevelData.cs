using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ConditionalLevel", menuName = "Robot Coder/Conditional Level", order = 2)]
    public class ConditionalLevelData : LevelData
    {
        private void OnEnable()
        {
            // Set values for a conditional level
            levelIndex = 3;
            levelName = "Условный уровень";
            description = "Используйте условные операторы";
            
            difficulty = 4;
            maxCommands = 25;
            optimalCommands = 15;
            
            startPosition = new Vector2Int(0, 0);
            startDirection = 1; // Facing right
            
            gridWidth = 8;
            gridHeight = 8;
            
            // Enable advanced commands
            allowJump = true;
            allowRepeat = true;
            allowIf = true;
            allowElse = true;
            
            // Create a complex grid layout
            gridLayout = new TileType[gridWidth, gridHeight];
            
            // Fill with empty tiles
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    gridLayout[x, y] = TileType.Empty;
                }
            }
            
            // Add walls in a more complex pattern
            for (int x = 1; x < 6; x++)
            {
                gridLayout[x, 3] = TileType.Wall;
            }
            
            // Create multiple paths
            gridLayout[2, 3] = TileType.Empty;
            gridLayout[4, 3] = TileType.Empty;
            
            // Add goal positions
            goalPositions = new Vector2Int[] { 
                new Vector2Int(3, 7), 
                new Vector2Int(7, 7) 
            };
            
            // Serialize the grid
            SerializeGrid();
        }
    }
}