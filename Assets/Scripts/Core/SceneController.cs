using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }
        
        [Header("Loading Settings")]
        [SerializeField] private float minLoadingTime = 1.0f;
        [SerializeField] private bool showLoadingScreen = true;
        [SerializeField] private GameObject loadingScreenPrefab;
        
        private AsyncOperation currentLoadingOperation;
        private List<string> loadedScenes = new List<string>();
        private GameObject loadingScreenInstance;
        
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
            // Создаем экран загрузки, если он есть
            if (loadingScreenPrefab != null)
            {
                loadingScreenInstance = Instantiate(loadingScreenPrefab);
                loadingScreenInstance.SetActive(false);
                DontDestroyOnLoad(loadingScreenInstance);
            }
        }
        
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // Показываем экран загрузки, если включено
            if (showLoadingScreen)
            {
                ShowLoadingScreen();
            }
            
            // Начинаем асинхронную загрузку сцены
            currentLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
            currentLoadingOperation.allowSceneActivation = false;
            
            // Минимальное время загрузки для плавного перехода
            float startTime = Time.time;
            
            // Ждем завершения загрузки
            while (!currentLoadingOperation.isDone)
            {
                // Если загрузка почти завершена и прошло минимальное время
                if (currentLoadingOperation.progress >= 0.9f && Time.time - startTime >= minLoadingTime)
                {
                    currentLoadingOperation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            // Добавляем сцену в список загруженных
            if (!loadedScenes.Contains(sceneName))
            {
                loadedScenes.Add(sceneName);
            }
            
            // Скрываем экран загрузки
            if (showLoadingScreen)
            {
                HideLoadingScreen();
            }
        }
        
        public void LoadNextLevel()
        {
            int currentLevel = GameManager.Instance?.GetCurrentLevel() ?? 1;
            int nextLevel = currentLevel + 1;
            
            string nextSceneName = $"Level_{nextLevel:D2}";
            
            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                LoadScene(nextSceneName);
            }
            else
            {
                // Возвращаемся в главное меню
                LoadScene("MainMenu");
            }
        }
        
        public void ReloadCurrentScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }
        
        public void LoadMainMenu()
        {
            LoadScene("MainMenu");
        }
        
        public float GetLoadingProgress()
        {
            return currentLoadingOperation?.progress ?? 0f;
        }
        
        public bool IsLoading()
        {
            return currentLoadingOperation != null && !currentLoadingOperation.isDone;
        }
        
        private void ShowLoadingScreen()
        {
            if (loadingScreenInstance != null)
            {
                loadingScreenInstance.SetActive(true);
            }
            Debug.Log("Loading screen shown");
        }
        
        private void HideLoadingScreen()
        {
            if (loadingScreenInstance != null)
            {
                loadingScreenInstance.SetActive(false);
            }
            Debug.Log("Loading screen hidden");
        }
        
        public void LoadAdditiveScene(string sceneName)
        {
            StartCoroutine(LoadAdditiveSceneAsync(sceneName));
        }
        
        private IEnumerator LoadAdditiveSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return operation;
            
            if (!loadedScenes.Contains(sceneName))
            {
                loadedScenes.Add(sceneName);
            }
        }
        
        public void UnloadScene(string sceneName)
        {
            if (loadedScenes.Contains(sceneName))
            {
                StartCoroutine(UnloadSceneAsync(sceneName));
            }
        }
        
        private IEnumerator UnloadSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
            yield return operation;
            
            loadedScenes.Remove(sceneName);
        }
        
        public List<string> GetLoadedScenes()
        {
            return new List<string>(loadedScenes);
        }
        
        public bool IsSceneLoaded(string sceneName)
        {
            return loadedScenes.Contains(sceneName);
        }
        
        public void LoadSceneWithProgress(string sceneName, System.Action<float> onProgress)
        {
            StartCoroutine(LoadSceneWithProgressAsync(sceneName, onProgress));
        }
        
        private IEnumerator LoadSceneWithProgressAsync(string sceneName, System.Action<float> onProgress)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            
            while (!operation.isDone)
            {
                onProgress?.Invoke(operation.progress);
                yield return null;
            }
            
            onProgress?.Invoke(1f);
        }
    }
}