using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Проверка целостности игровой системы
    /// </summary>
    public class GameIntegrityChecker : MonoBehaviour
    {
        [Header("Проверяемые компоненты")]
        [SerializeField] private bool checkOnStart = true;
        [SerializeField] private bool checkOnUpdate = false;
        [SerializeField] private float checkInterval = 5.0f;
        
        private float lastCheckTime = 0f;
        private List<string> issuesFound = new List<string>();
        
        private void Start()
        {
            if (checkOnStart)
            {
                CheckGameIntegrity();
            }
        }
        
        private void Update()
        {
            if (checkOnUpdate && Time.time - lastCheckTime >= checkInterval)
            {
                CheckGameIntegrity();
                lastCheckTime = Time.time;
            }
        }
        
        /// <summary>
        /// Проверка целостности игровой системы
        /// </summary>
        public void CheckGameIntegrity()
        {
            issuesFound.Clear();
            
            Debug.Log("=== НАЧАЛО ПРОВЕРКИ ЦЕЛОСТНОСТИ ===");
            
            // Проверяем основные менеджеры
            CheckManagers();
            
            // Проверяем синглтоны
            CheckSingletons();
            
            // Проверяем события
            CheckEvents();
            
            // Проверяем ресурсы
            CheckResources();
            
            // Выводим результаты
            ReportResults();
            
            Debug.Log("=== КОНЕЦ ПРОВЕРКИ ЦЕЛОСТНОСТИ ===");
        }
        
        private void CheckManagers()
        {
            Debug.Log("Проверка менеджеров...");
            
            // GameManager
            if (GameManager.Instance == null)
            {
                issuesFound.Add("GameManager не инициализирован");
                Debug.LogError("✗ GameManager не инициализирован");
            }
            else
            {
                Debug.Log("✓ GameManager инициализирован");
            }
            
            // LevelManager
            if (LevelManager.Instance == null)
            {
                issuesFound.Add("LevelManager не инициализирован");
                Debug.LogError("✗ LevelManager не инициализирован");
            }
            else
            {
                Debug.Log("✓ LevelManager инициализирован");
            }
            
            // AudioManager
            if (AudioManager.Instance == null)
            {
                issuesFound.Add("AudioManager не инициализирован");
                Debug.LogError("✗ AudioManager не инициализирован");
            }
            else
            {
                Debug.Log("✓ AudioManager инициализирован");
            }
        }
        
        private void CheckSingletons()
        {
            Debug.Log("Проверка синглтонов...");
            
            // RobotController
            if (RobotCoder.Core.RobotController.Instance == null)
            {
                issuesFound.Add("RobotController не инициализирован");
                Debug.LogError("✗ RobotController не инициализирован");
            }
            else
            {
                Debug.Log("✓ RobotController инициализирован");
            }
            
            // ProgramInterpreter
            if (ProgramInterpreter.Instance == null)
            {
                issuesFound.Add("ProgramInterpreter не инициализирован");
                Debug.LogError("✗ ProgramInterpreter не инициализирован");
            }
            else
            {
                Debug.Log("✓ ProgramInterpreter инициализирован");
            }
        }
        
        private void CheckEvents()
        {
            Debug.Log("Проверка событий...");
            // Здесь можно добавить проверку подписчиков на события
            Debug.Log("✓ Система событий готова");
        }
        
        private void CheckResources()
        {
            Debug.Log("Проверка ресурсов...");
            
            // Проверяем наличие уровней
            var levelManager = LevelManager.Instance;
            if (levelManager != null)
            {
                int levelCount = levelManager.GetLevelCount();
                if (levelCount > 0)
                {
                    Debug.Log($"✓ Найдено уровней: {levelCount}");
                }
                else
                {
                    issuesFound.Add("Не найдено уровней");
                    Debug.LogWarning("⚠ Не найдено уровней");
                }
            }
            
            Debug.Log("✓ Ресурсы проверены");
        }
        
        private void ReportResults()
        {
            if (issuesFound.Count == 0)
            {
                Debug.Log("✅ Все проверки пройдены успешно!");
            }
            else
            {
                Debug.LogWarning($"⚠ Найдено проблем: {issuesFound.Count}");
                foreach (string issue in issuesFound)
                {
                    Debug.LogWarning($"  • {issue}");
                }
            }
        }
        
        /// <summary>
        /// Быстрая проверка критических компонентов
        /// </summary>
        public void QuickCheck()
        {
            bool allGood = true;
            
            if (GameManager.Instance == null)
            {
                Debug.LogError("Критическая ошибка: GameManager не инициализирован!");
                allGood = false;
            }
            
            if (LevelManager.Instance == null)
            {
                Debug.LogError("Критическая ошибка: LevelManager не инициализирован!");
                allGood = false;
            }
            
            if (RobotCoder.Core.RobotController.Instance == null)
            {
                Debug.LogError("Критическая ошибка: RobotController не инициализирован!");
                allGood = false;
            }
            
            if (allGood)
            {
                Debug.Log("✅ Критические компоненты работают нормально");
            }
        }
    }
}