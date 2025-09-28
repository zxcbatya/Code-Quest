#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Core.Editor
{
    [CustomEditor(typeof(LevelData), true)]
    public class LevelEditor : UnityEditor.Editor
    {
        private LevelData _levelData;
        private bool _showGridEditor = true;
        private Vector2Int _selectedTile = Vector2Int.zero;
        
        private void OnEnable()
        {
            _levelData = (LevelData)target;
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            _showGridEditor = EditorGUILayout.Foldout(_showGridEditor, "Grid Editor", true);
            if (_showGridEditor)
            {
                DrawGridEditor();
            }
            
            EditorGUILayout.Space();
            
            GUILayout.Label("Utilities", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Serialize Grid"))
            {
                _levelData.SerializeGrid();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
            
            if (GUILayout.Button("Deserialize Grid"))
            {
                _levelData.DeserializeGrid();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
            
            if (GUILayout.Button("Clear Grid"))
            {
                ClearGrid();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
        
        private void DrawGridEditor()
        {
            if (_levelData.gridLayout == null || 
                _levelData.gridWidth <= 0 || _levelData.gridHeight <= 0)
            {
                EditorGUILayout.HelpBox("Grid not initialized. Set grid dimensions and click Deserialize Grid.", 
                                      MessageType.Info);
                return;
            }
            
            EditorGUILayout.LabelField("Selected Tile", 
                                     $"({_selectedTile.x}, {_selectedTile.y})");
            
            LevelData.TileType currentTileType = LevelData.TileType.Empty;
            if (_selectedTile.x >= 0 && _selectedTile.x < _levelData.gridWidth &&
                _selectedTile.y >= 0 && _selectedTile.y < _levelData.gridHeight)
            {
                currentTileType = _levelData.gridLayout[_selectedTile.x, _selectedTile.y];
            }
            
            LevelData.TileType newTileType = (LevelData.TileType)EditorGUILayout.EnumPopup(
                "Tile Type", currentTileType);
            
            if (newTileType != currentTileType)
            {
                if (_selectedTile.x >= 0 && _selectedTile.x < _levelData.gridWidth &&
                    _selectedTile.y >= 0 && _selectedTile.y < _levelData.gridHeight)
                {
                    _levelData.gridLayout[_selectedTile.x, _selectedTile.y] = newTileType;
                    EditorUtility.SetDirty(target);
                }
            }
            
            EditorGUILayout.Space();
            
            // Grid visualization
            DrawGridVisualization();
        }
        
        private void DrawGridVisualization()
        {
            GUIStyle gridStyle = new GUIStyle(GUI.skin.box);
            gridStyle.alignment = TextAnchor.MiddleCenter;
            gridStyle.fontSize = 8;
            
            GUIStyle selectedStyle = new GUIStyle(gridStyle);
            selectedStyle.normal.textColor = Color.yellow;
            selectedStyle.fontStyle = FontStyle.Bold;
            
            const int cellSize = 20;
            const int gridSize = 8;
            
            // Create a scroll view for the grid
            GUIStyle scrollViewStyle = new GUIStyle();
            scrollViewStyle.padding = new RectOffset(10, 10, 10, 10);
            
            EditorGUILayout.BeginVertical(scrollViewStyle);
            
            for (int y = _levelData.gridHeight - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                
                for (int x = 0; x < _levelData.gridWidth; x++)
                {
                    LevelData.TileType tileType = _levelData.gridLayout[x, y];
                    string tileLabel = GetTileSymbol(tileType);
                    
                    GUIStyle style = (x == _selectedTile.x && y == _selectedTile.y) ? 
                                   selectedStyle : gridStyle;
                    
                    Color tileColor = GetTileColor(tileType);
                    Color originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = tileColor;
                    
                    if (GUILayout.Button(tileLabel, style, 
                                       GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        _selectedTile = new Vector2Int(x, y);
                    }
                    
                    GUI.backgroundColor = originalColor;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private string GetTileSymbol(LevelData.TileType tileType)
        {
            switch (tileType)
            {
                case LevelData.TileType.Empty: return " ";
                case LevelData.TileType.Wall: return "W";
                case LevelData.TileType.Goal: return "G";
                case LevelData.TileType.Pit: return "P";
                case LevelData.TileType.Button: return "B";
                case LevelData.TileType.Door: return "D";
                case LevelData.TileType.Key: return "K";
                default: return "?";
            }
        }
        
        private Color GetTileColor(LevelData.TileType tileType)
        {
            switch (tileType)
            {
                case LevelData.TileType.Empty: return Color.white;
                case LevelData.TileType.Wall: return Color.gray;
                case LevelData.TileType.Goal: return Color.green;
                case LevelData.TileType.Pit: return Color.black;
                case LevelData.TileType.Button: return Color.blue;
                case LevelData.TileType.Door: return Color.red;
                case LevelData.TileType.Key: return Color.yellow;
                default: return Color.magenta;
            }
        }
        
        private void ClearGrid()
        {
            if (_levelData.gridLayout == null) return;
            
            for (int x = 0; x < _levelData.gridWidth; x++)
            {
                for (int y = 0; y < _levelData.gridHeight; y++)
                {
                    _levelData.gridLayout[x, y] = LevelData.TileType.Empty;
                }
            }
        }
    }
}
#endif