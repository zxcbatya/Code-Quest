using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "AdvancedLevel", menuName = "Robot Coder/Advanced Level", order = 1)]
    public class AdvancedLevelData : LevelData
    {
        private void OnEnable()
        {
            // Set values for an advanced level
            levelIndex = 2;
            levelName = "Продвинутый уровень";
            description = "Используйте прыжки и повторения";
            
            difficulty = 3;
            maxCommands = 20;
            optimalCommands = 12;
            
            startPosition = new Vector2Int(0, 0);
            startDirection = 1; // Facing right
            
            gridWidth = 8;
            gridHeight = 8;
            
            // Enable advanced commands
            allowJump = true;
            allowRepeat = true;
            
            // Create a more complex grid layout
            gridLayout = new TileType[gridWidth, gridHeight];
            
            // Fill with empty tiles
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    gridLayout[x, y] = TileType.Empty;
                }
            }
            
            // Add walls to create a path
            for (int x = 1; x < 7; x++)
            {
                gridLayout[x, 2] = TileType.Wall;
                gridLayout[x, 5] = TileType.Wall;
            }
            
            // Create gaps that require jumping
            gridLayout[3, 2] = TileType.Empty;
            gridLayout[4, 2] = TileType.Empty;
            
            // Add goal position
            goalPositions = new Vector2Int[] { new Vector2Int(7, 7) };
            
            // Serialize the grid
            SerializeGrid();
        }
    }
}