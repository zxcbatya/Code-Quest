using UnityEngine;
using System.IO;

namespace Core
{
    /// <summary>
    /// Менеджер ресурсов для автоматического создания необходимых файлов
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("Автоматическое создание ресурсов")]
        [SerializeField] private bool createMissingResources = true;
        [SerializeField] private bool createDefaultSettings = true;
        [SerializeField] private bool createTestLevels = false;
        
        private void Start()
        {
            if (createMissingResources)
            {
                CreateMissingResources();
            }
        }
        
        /// <summary>
        /// Создание отсутствующих ресурсов
        /// </summary>
        private void CreateMissingResources()
        {
            Debug.Log("Проверка и создание отсутствующих ресурсов...");
            
            // Создаем папку Resources если её нет
            string resourcesPath = Path.Combine(Application.dataPath, "Resources");
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                Debug.Log("✓ Создана папка Resources");
            }
            
            // Создаем папку Levels если её нет
            string levelsPath = Path.Combine(resourcesPath, "Levels");
            if (!Directory.Exists(levelsPath))
            {
                Directory.CreateDirectory(levelsPath);
                Debug.Log("✓ Создана папка Resources/Levels");
            }
            
            // Создаем папку Audio если её нет
            string audioPath = Path.Combine(resourcesPath, "Audio");
            if (!Directory.Exists(audioPath))
            {
                Directory.CreateDirectory(audioPath);
                Debug.Log("✓ Создана папка Resources/Audio");
            }
            
            // Создаем настройки проекта если их нет
            if (createDefaultSettings)
            {
                CreateDefaultSettings();
            }
            
            // Создаем тестовые уровни если нужно
            if (createTestLevels)
            {
                CreateTestLevels();
            }
            
            Debug.Log("Проверка ресурсов завершена");
        }
        
        /// <summary>
        /// Создание настроек проекта по умолчанию
        /// </summary>
        private void CreateDefaultSettings()
        {
            string settingsPath = Path.Combine(Application.dataPath, "Resources", "ProjectSettings.asset");
            if (!File.Exists(settingsPath))
            {
                // Создаем экземпляр настроек
                ProjectSettings settings = ScriptableObject.CreateInstance<ProjectSettings>();
                
                // Сохраняем в Resources (в редакторе)
                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateAsset(settings, "Assets/Resources/ProjectSettings.asset");
                UnityEditor.AssetDatabase.SaveAssets();
                Debug.Log("✓ Созданы настройки проекта по умолчанию");
                #endif
            }
        }
        
        /// <summary>
        /// Создание тестовых уровней
        /// </summary>
        private void CreateTestLevels()
        {
            Debug.Log("Создание тестовых уровней...");
            
            // Создаем несколько тестовых уровней
            for (int i = 1; i <= 3; i++)
            {
                CreateTestLevel(i);
            }
            
            Debug.Log("✓ Созданы тестовые уровни");
        }
        
        /// <summary>
        /// Создание одного тестового уровня
        /// </summary>
        private void CreateTestLevel(int levelIndex)
        {
            string levelPath = Path.Combine(Application.dataPath, "Resources", "Levels", $"Level_{levelIndex:D2}.asset");
            if (!File.Exists(levelPath))
            {
                // Создаем экземпляр уровня
                LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
                levelData.levelIndex = levelIndex;
                levelData.levelName = $"Тестовый уровень {levelIndex}";
                levelData.description = $"Тестовый уровень для проверки системы #{levelIndex}";
                levelData.difficulty = Mathf.Min(levelIndex, 5);
                levelData.maxCommands = 10 + (levelIndex * 5);
                levelData.optimalCommands = 5 + (levelIndex * 2);
                levelData.startPosition = new Vector2Int(0, 0);
                levelData.startDirection = 1; // Восток
                levelData.goalPositions = new Vector2Int[] { new Vector2Int(7, 7) };
                levelData.gridWidth = 8;
                levelData.gridHeight = 8;
                
                // Инициализируем сетку
                levelData.gridLayout = new LevelData.TileType[8, 8];
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        levelData.gridLayout[x, y] = LevelData.TileType.Empty;
                    }
                }
                
                // Добавляем стены по краям
                for (int x = 0; x < 8; x++)
                {
                    levelData.gridLayout[x, 0] = LevelData.TileType.Wall;
                    levelData.gridLayout[x, 7] = LevelData.TileType.Wall;
                }
                for (int y = 0; y < 8; y++)
                {
                    levelData.gridLayout[0, y] = LevelData.TileType.Wall;
                    levelData.gridLayout[7, y] = LevelData.TileType.Wall;
                }
                
                levelData.SerializeGrid();
                
                // Сохраняем уровень (в редакторе)
                #if UNITY_EDITOR
                string assetPath = $"Assets/Resources/Levels/Level_{levelIndex:D2}.asset";
                UnityEditor.AssetDatabase.CreateAsset(levelData, assetPath);
                Debug.Log($"✓ Создан тестовый уровень {levelIndex}");
                #endif
            }
        }
        
        /// <summary>
        /// Проверка целостности ресурсов
        /// </summary>
        [ContextMenu("Проверить целостность ресурсов")]
        public void CheckResourceIntegrity()
        {
            Debug.Log("=== ПРОВЕРКА ЦЕЛОСТНОСТИ РЕСУРСОВ ===");
            
            // Проверяем существование папок
            string resourcesPath = Path.Combine(Application.dataPath, "Resources");
            Debug.Log($"Resources folder exists: {Directory.Exists(resourcesPath)}");
            
            string levelsPath = Path.Combine(resourcesPath, "Levels");
            Debug.Log($"Levels folder exists: {Directory.Exists(levelsPath)}");
            
            string audioPath = Path.Combine(resourcesPath, "Audio");
            Debug.Log($"Audio folder exists: {Directory.Exists(audioPath)}");
            
            // Проверяем настройки проекта
            var settings = ProjectSettings.Instance;
            Debug.Log($"Project settings loaded: {settings != null}");
            
            Debug.Log("=== ПРОВЕРКА ЗАВЕРШЕНА ===");
        }
        
        /// <summary>
        /// Очистка неиспользуемых ресурсов
        /// </summary>
        [ContextMenu("Очистить неиспользуемые ресурсы")]
        public void CleanupUnusedResources()
        {
            Debug.Log("Очистка неиспользуемых ресурсов...");
            
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            Debug.Log("✓ Очистка неиспользуемых ресурсов завершена");
        }
    }
}