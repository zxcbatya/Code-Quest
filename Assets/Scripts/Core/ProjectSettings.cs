using UnityEngine;

namespace Core
{
    /// <summary>
    /// Глобальные настройки проекта
    /// </summary>
    [CreateAssetMenu(fileName = "ProjectSettings", menuName = "Robot Coder/Project Settings")]
    public class ProjectSettings : ScriptableObject
    {
        [Header("Игровые настройки")]
        [SerializeField] private int maxLevels = 50;
        [SerializeField] private int defaultMaxCommands = 20;
        [SerializeField] private float defaultExecutionSpeed = 1.0f;
        [SerializeField] private bool enableAdvancedCommands = false;
        
        [Header("Настройки UI")]
        [SerializeField] private float uiAnimationSpeed = 1.0f;
        [SerializeField] private bool enableTooltips = true;
        [SerializeField] private string defaultLanguage = "RU";
        
        [Header("Настройки производительности")]
        [SerializeField] private bool enableObjectPooling = true;
        [SerializeField] private int commandBlockPoolSize = 50;
        [SerializeField] private bool enableMemoryMonitoring = true;
        [SerializeField] private float memoryCheckInterval = 10.0f;
        
        [Header("Настройки отладки")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool enableSystemIntegrityCheck = true;
        [SerializeField] private bool autoRunIntegrationTest = false;
        
        // Статический экземпляр для быстрого доступа
        private static ProjectSettings _instance;
        public static ProjectSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ProjectSettings>("ProjectSettings");
                    if (_instance == null)
                    {
                        Debug.LogWarning("ProjectSettings не найдены в Resources. Создаются настройки по умолчанию.");
                        _instance = ScriptableObject.CreateInstance<ProjectSettings>();
                    }
                }
                return _instance;
            }
        }
        
        // Свойства для доступа к настройкам
        public int MaxLevels => maxLevels;
        public int DefaultMaxCommands => defaultMaxCommands;
        public float DefaultExecutionSpeed => defaultExecutionSpeed;
        public bool EnableAdvancedCommands => enableAdvancedCommands;
        public float UIAnimationSpeed => uiAnimationSpeed;
        public bool EnableTooltips => enableTooltips;
        public string DefaultLanguage => defaultLanguage;
        public bool EnableObjectPooling => enableObjectPooling;
        public int CommandBlockPoolSize => commandBlockPoolSize;
        public bool EnableMemoryMonitoring => enableMemoryMonitoring;
        public float MemoryCheckInterval => memoryCheckInterval;
        public bool EnableDebugLogs => enableDebugLogs;
        public bool EnableSystemIntegrityCheck => enableSystemIntegrityCheck;
        public bool AutoRunIntegrationTest => autoRunIntegrationTest;
        
        /// <summary>
        /// Применение настроек к системе
        /// </summary>
        public void ApplySettings()
        {
            if (enableDebugLogs)
            {
                Debug.Log("Применение глобальных настроек проекта...");
            }
            
            // Применяем настройки производительности
            if (EnableMemoryMonitoring)
            {
                var memoryMonitor = MemoryMonitor.Instance;
                if (memoryMonitor != null)
                {
                    // Настройки мониторинга памяти
                }
            }
            
            // Применяем настройки UI
            // Здесь можно добавить код для применения настроек UI
            
            if (enableDebugLogs)
            {
                Debug.Log("Глобальные настройки применены");
            }
        }
        
        /// <summary>
        /// Сброс к настройкам по умолчанию
        /// </summary>
        public void ResetToDefaults()
        {
            maxLevels = 50;
            defaultMaxCommands = 20;
            defaultExecutionSpeed = 1.0f;
            enableAdvancedCommands = false;
            uiAnimationSpeed = 1.0f;
            enableTooltips = true;
            defaultLanguage = "RU";
            enableObjectPooling = true;
            commandBlockPoolSize = 50;
            enableMemoryMonitoring = true;
            memoryCheckInterval = 10.0f;
            enableDebugLogs = false;
            enableSystemIntegrityCheck = true;
            autoRunIntegrationTest = false;
            
            Debug.Log("Настройки сброшены к значениям по умолчанию");
        }
        
        private void OnValidate()
        {
            // Проверяем корректность значений
            maxLevels = Mathf.Max(1, maxLevels);
            defaultMaxCommands = Mathf.Max(1, defaultMaxCommands);
            defaultExecutionSpeed = Mathf.Clamp(defaultExecutionSpeed, 0.1f, 5.0f);
            uiAnimationSpeed = Mathf.Max(0.1f, uiAnimationSpeed);
            commandBlockPoolSize = Mathf.Max(10, commandBlockPoolSize);
            memoryCheckInterval = Mathf.Max(1.0f, memoryCheckInterval);
        }
    }
}