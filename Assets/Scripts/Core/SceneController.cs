using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Core
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }
        
        [Header("Scene Settings")]
        [SerializeField] private float sceneTransitionDelay = 1.0f;
        [SerializeField] private bool showLoadingScreen = true;
        
        public System.Action<string> OnSceneLoading;
        public System.Action<string> OnSceneLoaded;
        
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
        
        public void LoadScene(string sceneName)
        {
            OnSceneLoading?.Invoke(sceneName);
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        public void LoadScene(int sceneIndex)
        {
            string sceneName = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            OnSceneLoading?.Invoke(sceneName);
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // Add delay for transition effect
            if (sceneTransitionDelay > 0)
            {
                yield return new WaitForSeconds(sceneTransitionDelay);
            }
            
            // Load the scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            // Wait until the scene is loaded
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            OnSceneLoaded?.Invoke(sceneName);
        }
        
        public void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            LoadScene(currentScene.name);
        }
        
        public void LoadNextScene()
        {
            int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            LoadScene(nextSceneIndex);
        }
        
        public void LoadPreviousScene()
        {
            int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
            if (previousSceneIndex < 0)
            {
                previousSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
            }
            LoadScene(previousSceneIndex);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
        
        public int GetCurrentSceneIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }
}