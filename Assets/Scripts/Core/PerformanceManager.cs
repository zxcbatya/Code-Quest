using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

namespace Core
{
    public class PerformanceManager : MonoBehaviour
    {
        public static PerformanceManager Instance { get; private set; }
        
        [Header("Performance Monitoring")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private float updateInterval = 1f; // Интервал обновления в секундах
        
        [Header("Performance Thresholds")]
        [SerializeField] private float targetFrameTime = 16.67f; // 60 FPS в миллисекундах
        [SerializeField] private float warningFrameTime = 33.33f; // 30 FPS в миллисекундах
        [SerializeField] private long memoryWarningThreshold = 500000000; // 500MB
        
        private float lastUpdateTime = 0f;
        private float frameRate = 0f;
        private float averageFrameTime = 0f;
        private long memoryUsage = 0;
        
        // Статистика
        private Queue<float> frameTimes = new Queue<float>();
        private const int FRAME_TIME_HISTORY = 60; // Храним последние 60 значений
        
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
            if (!enableMonitoring) return;
            
            // Обновляем статистику производительности
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdatePerformanceStats();
                lastUpdateTime = Time.time;
            }
        }
        
        private void UpdatePerformanceStats()
        {
            // Рассчитываем FPS
            frameRate = 1.0f / Time.deltaTime;
            
            // Рассчитываем среднее время кадра
            averageFrameTime = Time.deltaTime * 1000f; // В миллисекундах
            
            // Добавляем время кадра в историю
            frameTimes.Enqueue(averageFrameTime);
            if (frameTimes.Count > FRAME_TIME_HISTORY)
            {
                frameTimes.Dequeue();
            }
            
            // Получаем использование памяти
            #if !UNITY_WEBGL
            memoryUsage = Profiler.GetTotalAllocatedMemoryLong();
            #else
            // В WebGL точное измерение памяти затруднено
            memoryUsage = 0;
            #endif
            
            // Проверяем пороговые значения
            CheckPerformanceThresholds();
        }
        
        private void CheckPerformanceThresholds()
        {
            // Проверяем FPS
            if (averageFrameTime > warningFrameTime)
            {
                OnPerformanceWarning($"Low FPS detected: {frameRate:F1} FPS");
            }
            
            // Проверяем использование памяти
            if (memoryUsage > memoryWarningThreshold)
            {
                OnPerformanceWarning($"High memory usage: {memoryUsage / 1000000f:F1} MB");
            }
        }
        
        private void OnPerformanceWarning(string warning)
        {
            Debug.LogWarning($"Performance Warning: {warning}");
            
            // Отправляем событие для других систем
            OnPerformanceIssue?.Invoke(warning);
        }
        
        public float GetFrameRate()
        {
            return frameRate;
        }
        
        public float GetAverageFrameTime()
        {
            return averageFrameTime;
        }
        
        public long GetMemoryUsage()
        {
            return memoryUsage;
        }
        
        public float GetAverageFrameTimeHistory()
        {
            if (frameTimes.Count == 0) return 0f;
            
            float sum = 0f;
            foreach (float time in frameTimes)
            {
                sum += time;
            }
            return sum / frameTimes.Count;
        }
        
        public bool IsPerformanceGood()
        {
            return averageFrameTime <= targetFrameTime;
        }
        
        public bool IsPerformanceWarning()
        {
            return averageFrameTime > targetFrameTime && averageFrameTime <= warningFrameTime;
        }
        
        public bool IsPerformanceCritical()
        {
            return averageFrameTime > warningFrameTime;
        }
        
        // Событие для уведомления о проблемах производительности
        public System.Action<string> OnPerformanceIssue;
    }
}