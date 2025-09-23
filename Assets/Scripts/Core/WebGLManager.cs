using UnityEngine;
using UnityEngine.Rendering;

namespace Core
{
    public class WebGLManager : MonoBehaviour
    {
        public static WebGLManager Instance { get; private set; }
        
        [Header("WebGL Optimization")]
        [SerializeField] private bool optimizeForWebGL = true;
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool enableSrpBatcher = true;
        [SerializeField] private float memoryWarningThreshold = 0.8f; // 80% от доступной памяти
        
        [Header("Quality Settings")]
        [SerializeField] private int antiAliasing = 2;
        [SerializeField] private bool enableShadows = true;
        [SerializeField] private int shadowResolution = 2; // 0=Low, 1=Medium, 2=High, 3=VeryHigh
        
        private bool isWebGL;
        private float lastMemoryWarningTime;
        private const float MEMORY_WARNING_COOLDOWN = 30f; // 30 секунд между предупреждениями
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Проверяем, запущена ли игра в WebGL
            isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
            
            if (isWebGL && optimizeForWebGL)
            {
                OptimizeForWebGL();
            }
        }
        
        private void OptimizeForWebGL()
        {
            // Устанавливаем целевой FPS
            Application.targetFrameRate = targetFrameRate;
            
            // Включаем SRP Batcher, если проект использует SRP (URP/HDRP)
            if (enableSrpBatcher)
            {
                GraphicsSettings.useScriptableRenderPipelineBatching = true;
            }
            
            // Настраиваем сглаживание
            QualitySettings.antiAliasing = antiAliasing;
            
            // Настраиваем тени
            QualitySettings.shadows = enableShadows ? ShadowQuality.All : ShadowQuality.Disable;
            QualitySettings.shadowResolution = (ShadowResolution)shadowResolution;
            
            // Оптимизируем настройки качества
            QualitySettings.vSyncCount = 0; // Отключаем VSync для лучшей производительности
            QualitySettings.maxQueuedFrames = 1; // Минимизируем задержку
            
            Debug.Log("WebGL optimization applied");
        }
        
        private void Update()
        {
            // Проверяем использование памяти
            CheckMemoryUsage();
        }
        
        private void CheckMemoryUsage()
        {
            if (!isWebGL) return;
            
            // В WebGL сложно точно измерить использование памяти,
            // но мы можем отслеживать общую производительность
            if (Time.time - lastMemoryWarningTime > MEMORY_WARNING_COOLDOWN)
            {
                // Примерный расчет нагрузки
                float fps = 1.0f / Time.deltaTime;
                if (fps < targetFrameRate * 0.7f) // Если FPS ниже 70% от целевого
                {
                    HandleMemoryWarning();
                }
            }
        }
        
        private void HandleMemoryWarning()
        {
            lastMemoryWarningTime = Time.time;
            
            // Реагируем на предупреждение о памяти
            Debug.Log("Memory warning detected. Optimizing...");
            
            // Уменьшаем качество графики
            if (QualitySettings.antiAliasing > 0)
            {
                QualitySettings.antiAliasing = Mathf.Max(0, QualitySettings.antiAliasing - 2);
            }
            
            // Отключаем тени при необходимости
            if (QualitySettings.shadows != ShadowQuality.Disable)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
            
            // Вызываем сборку мусора
            System.GC.Collect();
        }
        
        public void ForceGarbageCollection()
        {
            if (isWebGL)
            {
                System.GC.Collect();
                Debug.Log("Garbage collection forced");
            }
        }
        
        public bool IsWebGL()
        {
            return isWebGL;
        }
        
        public void SetQualityLevel(int level)
        {
            if (isWebGL)
            {
                QualitySettings.SetQualityLevel(level, true);
            }
        }
        
        public int GetCurrentQualityLevel()
        {
            return QualitySettings.GetQualityLevel();
        }
        
        public void ReduceQuality()
        {
            if (isWebGL)
            {
                int currentLevel = QualitySettings.GetQualityLevel();
                if (currentLevel > 0)
                {
                    QualitySettings.SetQualityLevel(currentLevel - 1, true);
                }
            }
        }
        
        public void IncreaseQuality()
        {
            if (isWebGL)
            {
                int currentLevel = QualitySettings.GetQualityLevel();
                int maxLevel = QualitySettings.names.Length - 1;
                if (currentLevel < maxLevel)
                {
                    QualitySettings.SetQualityLevel(currentLevel + 1, true);
                }
            }
        }
    }
}