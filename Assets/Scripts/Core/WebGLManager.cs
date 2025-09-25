using UnityEngine;
using UnityEngine.Rendering;

namespace Core
{
    public class WebGLManager : MonoBehaviour
    {
        public static WebGLManager Instance { get; private set; }
        
        [Header("WebGL Settings")]
        [SerializeField] private bool enableWebGLFeatures = true;
        [SerializeField] private bool enableFullscreen = true;
        [SerializeField] private bool enableCursorLock = true;
        
        private bool isFullscreen = false;
        
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
            if (enableWebGLFeatures)
            {
                InitializeWebGLFeatures();
            }
        }
        
        private void InitializeWebGLFeatures()
        {
            // Set default quality settings for WebGL
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            
            Debug.Log("WebGL функции инициализированы");
        }
        
        public void ToggleFullscreen()
        {
            if (!enableWebGLFeatures || !enableFullscreen) return;
            
            #if UNITY_WEBGL && !UNITY_EDITOR
            // In a real implementation, you would use JavaScript to toggle fullscreen
            isFullscreen = !isFullscreen;
            Debug.Log($"Полноэкранный режим: {isFullscreen}");
            #endif
        }
        
        public void LockCursor()
        {
            if (!enableWebGLFeatures || !enableCursorLock) return;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        public void UnlockCursor()
        {
            if (!enableWebGLFeatures || !enableCursorLock) return;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        public void SetQualityLevel(int level)
        {
            if (!enableWebGLFeatures) return;
            
            QualitySettings.SetQualityLevel(level);
        }
        
        public void SetFrameRate(int frameRate)
        {
            if (!enableWebGLFeatures) return;
            
            Application.targetFrameRate = frameRate;
        }
        
        public bool IsWebGLFeaturesEnabled()
        {
            return enableWebGLFeatures;
        }
        
        public bool IsFullscreen()
        {
            return isFullscreen;
        }
    }
}