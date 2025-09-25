using UnityEngine;
using UnityEngine.Audio;

namespace Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }
        
        [Header("Audio Settings")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private float masterVolume = 0f;
        [SerializeField] private float musicVolume = 0f;
        [SerializeField] private float sfxVolume = 0f;
        
        [Header("Game Settings")]
        [SerializeField] private bool soundEnabled = true;
        [SerializeField] private bool musicEnabled = true;
        [SerializeField] private string language = "ru";
        [SerializeField] private bool showHints = true;
        
        public System.Action OnSettingsChanged;
        
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
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0f);
            soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
            musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            language = PlayerPrefs.GetString("Language", "ru");
            showHints = PlayerPrefs.GetInt("ShowHints", 1) == 1;
            
            ApplyAudioSettings();
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
            PlayerPrefs.SetFloat("MasterVolume", volume);
            ApplyAudioSettings();
            OnSettingsChanged?.Invoke();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
            ApplyAudioSettings();
            OnSettingsChanged?.Invoke();
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            ApplyAudioSettings();
            OnSettingsChanged?.Invoke();
        }
        
        public void SetSoundEnabled(bool enabled)
        {
            soundEnabled = enabled;
            PlayerPrefs.SetInt("SoundEnabled", enabled ? 1 : 0);
            ApplyAudioSettings();
            OnSettingsChanged?.Invoke();
        }
        
        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
            PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
            ApplyAudioSettings();
            OnSettingsChanged?.Invoke();
        }
        
        public void SetLanguage(string lang)
        {
            language = lang;
            PlayerPrefs.SetString("Language", lang);
            OnSettingsChanged?.Invoke();
        }
        
        public void SetShowHints(bool show)
        {
            showHints = show;
            PlayerPrefs.SetInt("ShowHints", show ? 1 : 0);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }
        
        private void ApplyAudioSettings()
        {
            if (audioMixer != null)
            {
                audioMixer.SetFloat("MasterVolume", soundEnabled ? masterVolume : -80f);
                audioMixer.SetFloat("MusicVolume", musicEnabled ? musicVolume : -80f);
                audioMixer.SetFloat("SFXVolume", soundEnabled ? sfxVolume : -80f);
            }
        }
        
        public float GetMasterVolume()
        {
            return masterVolume;
        }
        
        public float GetMusicVolume()
        {
            return musicVolume;
        }
        
        public float GetSFXVolume()
        {
            return sfxVolume;
        }
        
        public bool IsSoundEnabled()
        {
            return soundEnabled;
        }
        
        public bool IsMusicEnabled()
        {
            return musicEnabled;
        }
        
        public string GetLanguage()
        {
            return language;
        }
        
        public bool GetShowHints()
        {
            return showHints;
        }
    }
}