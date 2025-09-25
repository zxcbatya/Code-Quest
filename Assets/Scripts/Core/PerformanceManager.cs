using UnityEngine;

namespace Core
{
    public class PerformanceManager : MonoBehaviour
    {
        public static PerformanceManager Instance { get; private set; }
        
        [Header("Performance Settings")]
        [SerializeField] private bool enablePerformanceMonitoring = true;
        [SerializeField] private float targetFrameRate = 60f;
        [SerializeField] private bool vSyncEnabled = true;
        
        [Header("Monitoring")]
        [SerializeField] private float currentFPS = 0f;
        [SerializeField] private float averageFPS = 0f;
        [SerializeField] private float minFPS = Mathf.Infinity;
        [SerializeField] private float maxFPS = 0f;
        
        private float lastTime;
        private int frameCount;
        private float frameRateUpdateInterval = 0.5f;
        private float frameRateUpdateTimer;
        
        public System.Action<float> OnFPSUpdated;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePerformanceSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializePerformanceSettings()
        {
            QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
            Application.targetFrameRate = (int)targetFrameRate;
        }
        
        private void Update()
        {
            if (!enablePerformanceMonitoring) return;
            
            CalculateFPS();
        }
        
        private void CalculateFPS()
        {
            frameCount++;
            frameRateUpdateTimer += Time.unscaledDeltaTime;
            
            if (frameRateUpdateTimer >= frameRateUpdateInterval)
            {
                currentFPS = frameCount / frameRateUpdateTimer;
                averageFPS = (averageFPS + currentFPS) / 2;
                
                if (currentFPS < minFPS) minFPS = currentFPS;
                if (currentFPS > maxFPS) maxFPS = currentFPS;
                
                frameCount = 0;
                frameRateUpdateTimer = 0f;
                
                OnFPSUpdated?.Invoke(currentFPS);
            }
        }
        
        public void SetTargetFrameRate(float target)
        {
            targetFrameRate = target;
            Application.targetFrameRate = (int)target;
        }
        
        public void SetVSyncEnabled(bool enabled)
        {
            vSyncEnabled = enabled;
            QualitySettings.vSyncCount = enabled ? 1 : 0;
        }
        
        public void SetPerformanceMonitoringEnabled(bool enabled)
        {
            enablePerformanceMonitoring = enabled;
        }
        
        public float GetCurrentFPS()
        {
            return currentFPS;
        }
        
        public float GetAverageFPS()
        {
            return averageFPS;
        }
        
        public float GetMinFPS()
        {
            return minFPS;
        }
        
        public float GetMaxFPS()
        {
            return maxFPS;
        }
        
        public bool IsPerformanceMonitoringEnabled()
        {
            return enablePerformanceMonitoring;
        }
    }
}