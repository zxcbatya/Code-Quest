using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Core
{
    public class UnityResourceManager : MonoBehaviour
    {
        public static UnityResourceManager Instance { get; private set; }
        
        [Header("Resource Management")]
        [SerializeField] private bool autoUnloadUnusedAssets = true;
        [SerializeField] private float unloadInterval = 30f; // Интервал автоочистки в секундах
        
        private Dictionary<string, Object> loadedAssets = new Dictionary<string, Object>();
        private List<string> sceneAssets = new List<string>();
        private float lastUnloadTime = 0f;
        
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
            // Автоматическая очистка неиспользуемых ресурсов
            if (autoUnloadUnusedAssets && Time.time - lastUnloadTime > unloadInterval)
            {
                UnloadUnusedAssets();
                lastUnloadTime = Time.time;
            }
        }
        
        public T LoadAsset<T>(string path) where T : Object
        {
            // Проверяем, загружен ли уже ресурс
            if (loadedAssets.ContainsKey(path))
            {
                return loadedAssets[path] as T;
            }
            
            // Загружаем ресурс
            T asset = Resources.Load<T>(path);
            if (asset != null)
            {
                loadedAssets[path] = asset;
            }
            
            return asset;
        }
        
        public void UnloadAsset(string path)
        {
            if (loadedAssets.ContainsKey(path))
            {
                // В Unity мы не можем напрямую выгрузить отдельный ресурс,
                // но можем пометить его для выгрузки
                loadedAssets.Remove(path);
            }
        }
        
        public void UnloadUnusedAssets()
        {
            // Вызываем сборку мусора Unity
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            Debug.Log("Unused assets unloaded");
        }
        
        public void PreloadSceneAssets(string sceneName)
        {
            // Предзагрузка ресурсов для сцены
            // В реальной реализации здесь будет логика предзагрузки
            Debug.Log($"Preloading assets for scene: {sceneName}");
        }
        
        public void UnloadSceneAssets(string sceneName)
        {
            // Выгрузка ресурсов сцены
            // В реальной реализации здесь будет логика выгрузки
            Debug.Log($"Unloading assets for scene: {sceneName}");
        }
        
        public int GetLoadedAssetsCount()
        {
            return loadedAssets.Count;
        }
        
        public long GetTotalMemoryUsage()
        {
            // В WebGL точное измерение памяти затруднено
            #if !UNITY_WEBGL
            return Profiler.GetTotalAllocatedMemoryLong();
            #else
            return 0;
            #endif
        }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // При загрузке сцены предзагружаем необходимые ресурсы
            PreloadSceneAssets(scene.name);
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            // При выгрузке сцены освобождаем ресурсы
            UnloadSceneAssets(scene.name);
        }
    }
}