using UnityEngine;

namespace Core
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }
        
        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 8;
        [SerializeField] private int gridHeight = 8;
        [SerializeField] private float cellSize = 1f;
        
        [Header("Tile Prefabs")]
        [SerializeField] private GameObject floorTilePrefab;
        [SerializeField] private GameObject wallTilePrefab;
        [SerializeField] private GameObject goalTilePrefab;
        [SerializeField] private GameObject pitTilePrefab;
        
        [Header("Grid Parent")]
        [SerializeField] private Transform gridParent;
        
        private LevelData.TileType[,] gridLayout;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Проверяем, является ли объект корневым перед применением DontDestroyOnLoad
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void InitializeGrid(LevelData levelData)
        {
            if (levelData == null) return;
            
            gridWidth = levelData.gridWidth;
            gridHeight = levelData.gridHeight;
            
            // Убедимся, что gridLayout инициализирован правильного размера
            if (gridLayout == null || gridLayout.GetLength(0) != gridWidth || gridLayout.GetLength(1) != gridHeight)
            {
                gridLayout = new LevelData.TileType[gridWidth, gridHeight];
            }
            
            // Copy layout from level data
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    gridLayout[x, y] = levelData.GetTile(x, y);
                }
            }
            
            GenerateVisualGrid();
        }
        
        private void GenerateVisualGrid()
        {
            if (gridParent == null) return;
            
            // Clear existing grid
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }
            
            // Generate new grid
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                    GameObject tile = null;
                    
                    switch (gridLayout[x, y])
                    {
                        case LevelData.TileType.Wall:
                            if (wallTilePrefab != null)
                                tile = Instantiate(wallTilePrefab, position, Quaternion.identity, gridParent);
                            break;
                        case LevelData.TileType.Goal:
                            if (goalTilePrefab != null)
                                tile = Instantiate(goalTilePrefab, position, Quaternion.identity, gridParent);
                            break;
                        case LevelData.TileType.Pit:
                            if (pitTilePrefab != null)
                                tile = Instantiate(pitTilePrefab, position, Quaternion.identity, gridParent);
                            break;
                        default:
                            if (floorTilePrefab != null)
                                tile = Instantiate(floorTilePrefab, position, Quaternion.identity, gridParent);
                            break;
                    }
                    
                    if (tile != null)
                    {
                        tile.name = $"Tile_{x}_{y}";
                    }
                }
            }
        }
        
        public LevelData.TileType GetTileType(int x, int y)
        {
            // Убедимся, что gridLayout инициализирован
            if (gridLayout == null || gridLayout.GetLength(0) != gridWidth || gridLayout.GetLength(1) != gridHeight)
            {
                return LevelData.TileType.Wall;
            }
            
            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                return LevelData.TileType.Wall;
                
            return gridLayout[x, y];
        }
        
        public bool IsPositionValid(int x, int y)
        {
            // Убедимся, что gridLayout инициализирован
            if (gridLayout == null || gridLayout.GetLength(0) != gridWidth || gridLayout.GetLength(1) != gridHeight)
            {
                return false;
            }
            
            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                return false;
                
            return gridLayout[x, y] != LevelData.TileType.Wall && 
                   gridLayout[x, y] != LevelData.TileType.Pit;
        }
        
        public Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * cellSize, 0, gridPos.y * cellSize);
        }
        
        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x / cellSize);
            int y = Mathf.RoundToInt(worldPos.z / cellSize);
            return new Vector2Int(x, y);
        }
    }
}