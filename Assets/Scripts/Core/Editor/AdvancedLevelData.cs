using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "AdvancedLevel", menuName = "Robot Coder/Advanced Level", order = 1)]
    public class AdvancedLevelData : LevelData
    {
        private void OnEnable()
        {
            levelIndex = 2;
            levelName = "Продвинутый уровень";
            description = "Используйте прыжки и повторения";
            
            difficulty = 3;
            maxCommands = 20;
            optimalCommands = 12;
            
            startPosition = new Vector2Int(0, 0);
            startDirection = 1; 
            
            gridWidth = 8;
            gridHeight = 8;
            
            allowJump = true;
            allowRepeat = true;
            
            gridLayout = new TileType[gridWidth, gridHeight];
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    gridLayout[x, y] = TileType.Empty;
                }
            }
            
            for (int x = 1; x < 7; x++)
            {
                gridLayout[x, 2] = TileType.Wall;
                gridLayout[x, 5] = TileType.Wall;
            }
            
            gridLayout[3, 2] = TileType.Empty;
            gridLayout[4, 2] = TileType.Empty;
            
            goalPositions = new Vector2Int[] { new Vector2Int(7, 7) };
            
            SerializeGrid();
        }
    }
}