using UnityEngine;
using UnityEditor;

namespace Core
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(LevelData), true)]
    public class LevelEditorHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            LevelData levelData = (LevelData)target;
            
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Serialize Grid"))
            {
                levelData.SerializeGrid();
                EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("Deserialize Grid"))
            {
                levelData.DeserializeGrid();
                EditorUtility.SetDirty(target);
            }
            
            EditorGUILayout.HelpBox("Нажмите 'Serialize Grid' после изменения gridLayout вручную. " +
                                  "Нажмите 'Deserialize Grid' чтобы восстановить gridLayout из serializedGrid.", 
                                  MessageType.Info);
        }
    }
    #endif
}