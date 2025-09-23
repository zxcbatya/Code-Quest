using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using System.Collections.Generic;

namespace Core
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }
        
        [Header("Resource Paths")]
        [SerializeField] private string levelsPath = "Levels/";
        [SerializeField] private string prefabsPath = "Prefabs/";
        [SerializeField] private string materialsPath = "Materials/";
        [SerializeField] private string audioPath = "Audio/";
        
        [Header("Memory Management (from UnityResourceManager)")]
        [SerializeField] private bool autoUnloadUnusedAssets = true;
        [SerializeField] private float unloadInterval = 30f;
        private float lastUnloadTime = 0f;
        
        private Dictionary<string, Object> loadedResources = new Dictionary<string, Object>();
        
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
        
        private void Update()
        {
            if (autoUnloadUnusedAssets && Time.time - lastUnloadTime > unloadInterval)
            {
                UnloadUnusedAssets();
                lastUnloadTime = Time.time;
            }
        }
        
        public T LoadResource<T>(string path) where T : Object
        {
            // Проверяем, загружен ли уже ресурс
            if (loadedResources.ContainsKey(path))
            {
                return loadedResources[path] as T;
            }
            
            // Загружаем ресурс
            T resource = Resources.Load<T>(path);
            if (resource != null)
            {
                loadedResources[path] = resource;
            }
            
            return resource;
        }
        
        public LevelData LoadLevelData(int levelNumber)
        {
            string path = $"{levelsPath}Level_{levelNumber:D2}";
            return LoadResource<LevelData>(path);
        }
        
        public GameObject LoadPrefab(string prefabName)
        {
            string path = $"{prefabsPath}{prefabName}";
            return LoadResource<GameObject>(path);
        }
        
        public Material LoadMaterial(string materialName)
        {
            string path = $"{materialsPath}{materialName}";
            return LoadResource<Material>(path);
        }
        
        public AudioClip LoadAudioClip(string audioName)
        {
            string path = $"{audioPath}{audioName}";
            return LoadResource<AudioClip>(path);
        }
        
        public void UnloadResource(string path)
        {
            if (loadedResources.ContainsKey(path))
            {
                loadedResources.Remove(path);
            }
        }
        
        public void UnloadAllResources()
        {
            loadedResources.Clear();
        }
        
        public int GetLoadedResourcesCount()
        {
            return loadedResources.Count;
        }
        
        // --- API adapted from UnityResourceManager ---
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        
        public long GetTotalMemoryUsage()
        {
            #if !UNITY_WEBGL
            return Profiler.GetTotalAllocatedMemoryLong();
            #else
            return 0;
            #endif
        }
        
        public void PreloadSceneAssets(string sceneName)
        {
            // Заглушка под предзагрузку (сценарий можно расширить при необходимости)
        }
        
        public void UnloadSceneAssets(string sceneName)
        {
            // Заглушка под выгрузку ресурсов сцены
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PreloadSceneAssets(scene.name);
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            UnloadSceneAssets(scene.name);
        }
    }
}