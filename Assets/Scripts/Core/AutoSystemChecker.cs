using UnityEngine;

namespace Core
{
    /// <summary>
    /// Автоматическая проверка системы при запуске игры
    /// </summary>
    [DefaultExecutionOrder(-1000)] // Выполняется раньше других скриптов
    public class AutoSystemChecker : MonoBehaviour
    {
        [Header("Настройки проверки")]
        [SerializeField] private bool runCheckOnAwake = true;
        [SerializeField] private bool showDetailedLog = false;
        [SerializeField] private bool createRequiredManagers = true;
        
        private static bool hasRun = false;
        
        private void Awake()
        {
            // Убеждаемся, что проверка выполняется только один раз
            if (hasRun) return;
            hasRun = true;
            
            if (runCheckOnAwake)
            {
                RunSystemCheck();
            }
        }
        
        private void RunSystemCheck()
        {
            if (showDetailedLog)
                Debug.Log("=== АВТОМАТИЧЕСКАЯ ПРОВЕРКА СИСТЕМЫ ===");
            
            // Проверяем и создаем необходимые менеджеры
            CheckAndCreateManagers();
            
            // Проверяем целостность системы
            CheckSystemIntegrity();
            
            if (showDetailedLog)
                Debug.Log("=== ПРОВЕРКА ЗАВЕРШЕНА ===");
        }
        
        private void CheckAndCreateManagers()
        {
            if (showDetailedLog)
                Debug.Log("Проверка и создание менеджеров...");
            
            // GameManager
            if (GameManager.Instance == null && createRequiredManagers)
            {
                CreateManager<GameManager>("GameManager");
            }
            
            // LevelManager
            if (LevelManager.Instance == null && createRequiredManagers)
            {
                CreateManager<LevelManager>("LevelManager");
            }
            
            // AudioManager
            if (AudioManager.Instance == null && createRequiredManagers)
            {
                CreateManager<AudioManager>("AudioManager");
            }
            
            // SceneCleanupManager
            if (SceneCleanupManager.Instance == null && createRequiredManagers)
            {
                CreateManager<SceneCleanupManager>("SceneCleanupManager");
            }
            
            // MemoryMonitor
            if (MemoryMonitor.Instance == null && createRequiredManagers)
            {
                CreateManager<MemoryMonitor>("MemoryMonitor");
            }
        }
        
        private void CheckSystemIntegrity()
        {
            if (showDetailedLog)
                Debug.Log("Проверка целостности системы...");
            
            bool allGood = true;
            
            // Проверяем критические компоненты
            if (GameManager.Instance == null)
            {
                Debug.LogError("КРИТИЧЕСКАЯ ОШИБКА: GameManager не инициализирован!");
                allGood = false;
            }
            
            if (LevelManager.Instance == null)
            {
                Debug.LogError("КРИТИЧЕСКАЯ ОШИБКА: LevelManager не инициализирован!");
                allGood = false;
            }
            
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("Предупреждение: AudioManager не инициализирован");
            }
            
            if (allGood)
            {
                if (showDetailedLog)
                    Debug.Log("✅ Все критические системы инициализированы");
            }
            else
            {
                Debug.LogError("❌ Обнаружены критические ошибки в системе!");
            }
        }
        
        private void CreateManager<T>(string gameObjectName) where T : Component
        {
            // Проверяем, существует ли уже объект с таким именем
            T existing = FindObjectOfType<T>();
            if (existing != null)
            {
                if (showDetailedLog)
                    Debug.Log($"✓ {gameObjectName} уже существует");
                return;
            }
            
            // Создаем новый GameObject
            GameObject managerObject = new GameObject(gameObjectName);
            managerObject.AddComponent<T>();
            
            if (showDetailedLog)
                Debug.Log($"✓ Создан {gameObjectName}");
            
            // Делаем DontDestroyOnLoad для менеджеров
            if (typeof(T) != typeof(MemoryMonitor)) // MemoryMonitor сам управляет этим
            {
                DontDestroyOnLoad(managerObject);
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            // Сбрасываем флаг при каждой загрузке игры
            hasRun = false;
        }
    }
}