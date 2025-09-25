using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Монитор памяти для отслеживания утечек и оптимизации
    /// </summary>
    public class MemoryMonitor : MonoBehaviour
    {
        public static MemoryMonitor Instance { get; private set; }
        
        [Header("Настройки мониторинга")]
        [SerializeField] private bool showMemoryInfo = false;
        [SerializeField] private float updateInterval = 1.0f;
        
        private float lastUpdateTime = 0f;
        
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
        
        private void Update()
        {
            if (!showMemoryInfo) return;
            
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateMemoryInfo();
                lastUpdateTime = Time.time;
            }
        }
        
        private void UpdateMemoryInfo()
        {
            long monoUsedSize = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
            long monoHeapSize = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
            long totalReservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
            long totalAllocatedMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            
            Debug.Log($"=== ИНФОРМАЦИЯ О ПАМЯТИ ===\n" +
                     $"Mono Used: {FormatBytes(monoUsedSize)}\n" +
                     $"Mono Heap: {FormatBytes(monoHeapSize)}\n" +
                     $"Total Reserved: {FormatBytes(totalReservedMemory)}\n" +
                     $"Total Allocated: {FormatBytes(totalAllocatedMemory)}\n" +
                     $"==========================");
        }
        
        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
        
        /// <summary>
        /// Принудительная сборка мусора
        /// </summary>
        public void ForceGarbageCollection()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            
        }
        
        /// <summary>
        /// Включить/выключить отображение информации о памяти
        /// </summary>
        public void ToggleMemoryInfo()
        {
            showMemoryInfo = !showMemoryInfo;
            Debug.Log($"Отображение информации о памяти: {showMemoryInfo}");
        }
        
        private void OnDisable()
        {
            // Очищаем ссылку при отключении
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}