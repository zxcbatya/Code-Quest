#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Core.Editor
{
    [CustomEditor(typeof(LevelData), true)]
    public class LevelEditor : UnityEditor.Editor
    {
        private LevelData levelData;
        private bool showGridEditor = true;
        private Vector2Int selectedTile = Vector2Int.zero;
        
        private void OnEnable()
        {
            levelData = (LevelData)target;
        }
        
        public override void OnInspectorGUI()
        {
            // Draw default inspector
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            // Grid editor
            showGridEditor = EditorGUILayout.Foldout(showGridEditor, "Grid Editor", true);
            if (showGridEditor)
            {
                DrawGridEditor();
            }
            
            EditorGUILayout.Space();
            
            // Utility buttons
            GUILayout.Label("Utilities", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Serialize Grid"))
            {
                levelData.SerializeGrid();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
            
            if (GUILayout.Button("Deserialize Grid"))
            {
                levelData.DeserializeGrid();
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
            if (levelData.gridLayout == null || 
                levelData.gridWidth <= 0 || levelData.gridHeight <= 0)
            {
                EditorGUILayout.HelpBox("Grid not initialized. Set grid dimensions and click Deserialize Grid.", 
                                      MessageType.Info);
                return;
            }
            
            // Selected tile info
            EditorGUILayout.LabelField("Selected Tile", 
                                     $"({selectedTile.x}, {selectedTile.y})");
            
            LevelData.TileType currentTileType = LevelData.TileType.Empty;
            if (selectedTile.x >= 0 && selectedTile.x < levelData.gridWidth &&
                selectedTile.y >= 0 && selectedTile.y < levelData.gridHeight)
            {
                currentTileType = levelData.gridLayout[selectedTile.x, selectedTile.y];
            }
            
            LevelData.TileType newTileType = (LevelData.TileType)EditorGUILayout.EnumPopup(
                "Tile Type", currentTileType);
            
            if (newTileType != currentTileType)
            {
                if (selectedTile.x >= 0 && selectedTile.x < levelData.gridWidth &&
                    selectedTile.y >= 0 && selectedTile.y < levelData.gridHeight)
                {
                    levelData.gridLayout[selectedTile.x, selectedTile.y] = newTileType;
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
            
            for (int y = levelData.gridHeight - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                
                for (int x = 0; x < levelData.gridWidth; x++)
                {
                    LevelData.TileType tileType = levelData.gridLayout[x, y];
                    string tileLabel = GetTileSymbol(tileType);
                    
                    GUIStyle style = (x == selectedTile.x && y == selectedTile.y) ? 
                                   selectedStyle : gridStyle;
                    
                    Color tileColor = GetTileColor(tileType);
                    Color originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = tileColor;
                    
                    if (GUILayout.Button(tileLabel, style, 
                                       GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        selectedTile = new Vector2Int(x, y);
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
            if (levelData.gridLayout == null) return;
            
            for (int x = 0; x < levelData.gridWidth; x++)
            {
                for (int y = 0; y < levelData.gridHeight; y++)
                {
                    levelData.gridLayout[x, y] = LevelData.TileType.Empty;
                }
            }
        }
    }
}
#endif