using UnityEngine;
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
    }
}