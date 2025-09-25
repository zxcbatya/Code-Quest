using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// Менеджер очистки памяти при переходах между сценами
    /// </summary>
    public class SceneCleanupManager : MonoBehaviour
    {
        public static SceneCleanupManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Подписываемся на события SceneManager
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.sceneUnloaded += OnSceneUnloaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Сцена загружена: {scene.name}");
            
            // Очищаем неиспользуемые ресурсы
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"Сцена выгружена: {scene.name}");
        }
        
        /// <summary>
        /// Принудительная очистка памяти
        /// </summary>
        public void ForceCleanup()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            Debug.Log("Принудительная очистка памяти выполнена");
        }
        
        /// <summary>
        /// Загрузка сцены с очисткой
        /// </summary>
        public void LoadSceneWithCleanup(string sceneName)
        {
            Debug.Log($"Загрузка сцены с очисткой: {sceneName}");
            
            // Сбрасываем Time.timeScale
            Time.timeScale = 1f;
            
            // Очищаем ресурсы
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            // Загружаем сцену
            SceneManager.LoadScene(sceneName);
        }
        
        /// <summary>
        /// Загрузка сцены с очисткой по индексу
        /// </summary>
        public void LoadSceneWithCleanup(int sceneIndex)
        {
            Debug.Log($"Загрузка сцены с очисткой по индексу: {sceneIndex}");
            
            // Сбрасываем Time.timeScale
            Time.timeScale = 1f;
            
            // Очищаем ресурсы
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            // Загружаем сцену
            SceneManager.LoadScene(sceneIndex);
        }
        
        private void OnDestroy()
        {
            // Отписываемся от событий
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }
}