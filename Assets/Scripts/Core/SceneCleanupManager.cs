using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneCleanupManager : MonoBehaviour
    {
        public static SceneCleanupManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
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
            
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"Сцена выгружена: {scene.name}");
        }
        

        public void ForceCleanup()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            Debug.Log("Принудительная очистка памяти выполнена");
        }
        

        public void LoadSceneWithCleanup(string sceneName)
        {
            Debug.Log($"Загрузка сцены с очисткой: {sceneName}");
            
            Time.timeScale = 1f;
            
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            SceneManager.LoadScene(sceneName);
        }
        

        public void LoadSceneWithCleanup(int sceneIndex)
        {
            
            Time.timeScale = 1f;
            
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            SceneManager.LoadScene(sceneIndex);
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }
}