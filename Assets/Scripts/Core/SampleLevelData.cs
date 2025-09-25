using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "SampleLevel", menuName = "Robot Coder/Sample Level", order = 0)]
    public class SampleLevelData : LevelData
    {
        private void OnEnable()
        {
            // Set default values for a sample level
            levelIndex = 1;
            levelName = "Обучающий уровень";
            description = "Научитесь перемещать робота по полю";
            
            difficulty = 1;
            maxCommands = 10;
            optimalCommands = 5;
            
            startPosition = new Vector2Int(0, 0);
            startDirection = 1; // Facing right
            
            gridWidth = 8;
            gridHeight = 8;
            
            // Create a simple grid layout
            gridLayout = new TileType[gridWidth, gridHeight];
            
            // Fill with empty tiles
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    gridLayout[x, y] = TileType.Empty;
                }
            }
            
            // Add some walls
            for (int x = 2; x < 6; x++)
            {
                gridLayout[x, 3] = TileType.Wall;
            }
            
            // Add goal position
            goalPositions = new Vector2Int[] { new Vector2Int(7, 7) };
            
            // Serialize the grid
            SerializeGrid();
        }
    }
}