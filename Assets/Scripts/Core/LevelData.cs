using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "Robot Coder/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Info")]
        public int levelIndex;
        public string levelName;
        public string description;
        
        [Header("Difficulty")]
        [Range(1, 5)]
        public int difficulty = 1;
        public int maxCommands = 10;
        public int optimalCommands = 5;
        
        [Header("Robot Settings")]
        public Vector2Int startPosition = Vector2Int.zero;
        public int startDirection = 0; // 0=North, 1=East, 2=South, 3=West
        
        [Header("Goal Settings")]
        public Vector2Int[] goalPositions;
        public bool requireAllGoals = true;
        
        [Header("Grid Layout")]
        public int gridWidth = 8;
        public int gridHeight = 8;
        public TileType[,] gridLayout;
        
        [Header("Available Commands")]
        public bool allowMoveForward = true;
        public bool allowTurnLeft = true;
        public bool allowTurnRight = true;
        public bool allowJump = false;
        public bool allowInteract = false;
        public bool allowRepeat = false;
        public bool allowIf = false;
        
        public enum TileType
        {
            Empty = 0,
            Wall = 1,
            Goal = 2,
            Pit = 3,
            Button = 4,
            Door = 5,
            Key = 6
        }

        [System.Serializable]
        public struct GridTile
        {
            public Vector2Int position;
            public TileType type;
            public int value; // Для дополнительных параметров
        }

        [Header("Serialized Grid")]
        public GridTile[] serializedGrid;

        private void OnValidate()
        {
            if (serializedGrid != null && serializedGrid.Length > 0)
            {
                DeserializeGrid();
            }
        }

        public void SerializeGrid()
        {
            if (gridLayout == null) return;

            var tiles = new System.Collections.Generic.List<GridTile>();
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridLayout[x, y] != TileType.Empty)
                    {
                        tiles.Add(new GridTile
                        {
                            position = new Vector2Int(x, y),
                            type = gridLayout[x, y],
                            value = 0
                        });
                    }
                }
            }

            serializedGrid = tiles.ToArray();
        }

        public void DeserializeGrid()
        {
            gridLayout = new TileType[gridWidth, gridHeight];
            
            if (serializedGrid == null) return;

            foreach (var tile in serializedGrid)
            {
                if (tile.position.x >= 0 && tile.position.x < gridWidth &&
                    tile.position.y >= 0 && tile.position.y < gridHeight)
                {
                    gridLayout[tile.position.x, tile.position.y] = tile.type;
                }
            }
        }

        public TileType GetTile(int x, int y)
        {
            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                return TileType.Wall;
                
            return gridLayout[x, y];
        }

        public bool IsGoalPosition(Vector2Int position)
        {
            if (goalPositions == null) return false;
            
            foreach (var goal in goalPositions)
            {
                if (goal == position) return true;
            }
            
            return false;
        }
    }
}