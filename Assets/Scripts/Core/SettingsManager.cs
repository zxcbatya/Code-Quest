using UnityEngine;

namespace Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }
        
        [Header("Audio Settings")]
        [SerializeField] private float masterVolume = 1.0f;
        [SerializeField] private float sfxVolume = 1.0f;
        [SerializeField] private float musicVolume = 1.0f;
        
        [Header("Gameplay Settings")]
        [SerializeField] private string language = "RU";
        [SerializeField] private bool showGrid = true;
        [SerializeField] private float cameraSensitivity = 1.0f;
        
        [Header("Performance Settings")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool vSync = true;
        
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string LANGUAGE_KEY = "Language";
        private const string SHOW_GRID_KEY = "ShowGrid";
        private const string CAMERA_SENSITIVITY_KEY = "CameraSensitivity";
        private const string TARGET_FRAMERATE_KEY = "TargetFrameRate";
        private const string VSYNC_KEY = "VSync";
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1.0f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1.0f);
            language = PlayerPrefs.GetString(LANGUAGE_KEY, "RU");
            showGrid = PlayerPrefs.GetInt(SHOW_GRID_KEY, 1) == 1;
            cameraSensitivity = PlayerPrefs.GetFloat(CAMERA_SENSITIVITY_KEY, 1.0f);
            targetFrameRate = PlayerPrefs.GetInt(TARGET_FRAMERATE_KEY, 60);
            vSync = PlayerPrefs.GetInt(VSYNC_KEY, 1) == 1;
            
            ApplySettings();
        }
        
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.SetString(LANGUAGE_KEY, language);
            PlayerPrefs.SetInt(SHOW_GRID_KEY, showGrid ? 1 : 0);
            PlayerPrefs.SetFloat(CAMERA_SENSITIVITY_KEY, cameraSensitivity);
            PlayerPrefs.SetInt(TARGET_FRAMERATE_KEY, targetFrameRate);
            PlayerPrefs.SetInt(VSYNC_KEY, vSync ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        private void ApplySettings()
        {
            // Применяем настройки аудио
            AudioListener.volume = masterVolume;
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(sfxVolume);
            }
            
            // Применяем настройки производительности
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSync ? 1 : 0;
            
            // Применяем настройки локализации
            if (UI.LocalizationManager.Instance != null)
            {
                UI.LocalizationManager.Instance.SetLanguage(language);
            }
        }
        
        // Методы доступа к настройкам
        
        public float GetMasterVolume() => masterVolume;
        public void SetMasterVolume(float volume) 
        { 
            masterVolume = Mathf.Clamp01(volume);
            AudioListener.volume = masterVolume;
            SaveSettings();
        }
        
        public float GetSFXVolume() => sfxVolume;
        public void SetSFXVolume(float volume) 
        { 
            sfxVolume = Mathf.Clamp01(volume);
            AudioManager.Instance?.SetSFXVolume(sfxVolume);
            SaveSettings();
        }
        
        public float GetMusicVolume() => musicVolume;
        public void SetMusicVolume(float volume) 
        { 
            musicVolume = Mathf.Clamp01(volume);
            SaveSettings();
        }
        
        public string GetLanguage() => language;
        public void SetLanguage(string lang) 
        { 
            language = lang;
            UI.LocalizationManager.Instance?.SetLanguage(language);
            SaveSettings();
        }
        
        public bool GetShowGrid() => showGrid;
        public void SetShowGrid(bool show) 
        { 
            showGrid = show;
            SaveSettings();
        }
        
        public float GetCameraSensitivity() => cameraSensitivity;
        public void SetCameraSensitivity(float sensitivity) 
        { 
            cameraSensitivity = sensitivity;
            SaveSettings();
        }
        
        public int GetTargetFrameRate() => targetFrameRate;
        public void SetTargetFrameRate(int frameRate) 
        { 
            targetFrameRate = frameRate;
            Application.targetFrameRate = targetFrameRate;
            SaveSettings();
        }
        
        public bool GetVSync() => vSync;
        public void SetVSync(bool enabled) 
        { 
            vSync = enabled;
            QualitySettings.vSyncCount = vSync ? 1 : 0;
            SaveSettings();
        }
    }
}