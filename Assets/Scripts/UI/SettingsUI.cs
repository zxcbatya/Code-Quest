using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle hintsToggle;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Dropdown languageDropdown;
        
        private void Start()
        {
            InitializeSettingsUI();
        }
        
        private void InitializeSettingsUI()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(HideSettings);
                
            if (SettingsManager.Instance != null)
            {
                LoadSettings();
                SetupEventListeners();
            }
        }
        
        private void LoadSettings()
        {
            if (SettingsManager.Instance == null) return;
            
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = SettingsManager.Instance.GetMasterVolume();
                
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = SettingsManager.Instance.GetMusicVolume();
                
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = SettingsManager.Instance.GetSFXVolume();
                
            if (soundToggle != null)
                soundToggle.isOn = SettingsManager.Instance.IsSoundEnabled();
                
            if (musicToggle != null)
                musicToggle.isOn = SettingsManager.Instance.IsMusicEnabled();
                
            if (hintsToggle != null)
                hintsToggle.isOn = SettingsManager.Instance.GetShowHints();
                
            if (languageDropdown != null)
            {
                // Set language selection (0 = Russian, 1 = English)
                languageDropdown.value = SettingsManager.Instance.GetLanguage() == "ru" ? 0 : 1;
            }
        }
        
        private void SetupEventListeners()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
                
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                
            if (soundToggle != null)
                soundToggle.onValueChanged.AddListener(OnSoundToggled);
                
            if (musicToggle != null)
                musicToggle.onValueChanged.AddListener(OnMusicToggled);
                
            if (hintsToggle != null)
                hintsToggle.onValueChanged.AddListener(OnHintsToggled);
                
            if (languageDropdown != null)
                languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
        
        private void OnMasterVolumeChanged(float value)
        {
            SettingsManager.Instance?.SetMasterVolume(value);
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            SettingsManager.Instance?.SetMusicVolume(value);
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            SettingsManager.Instance?.SetSFXVolume(value);
        }
        
        private void OnSoundToggled(bool isOn)
        {
            SettingsManager.Instance?.SetSoundEnabled(isOn);
        }
        
        private void OnMusicToggled(bool isOn)
        {
            SettingsManager.Instance?.SetMusicEnabled(isOn);
        }
        
        private void OnHintsToggled(bool isOn)
        {
            SettingsManager.Instance?.SetShowHints(isOn);
        }
        
        private void OnLanguageChanged(int index)
        {
            string language = index == 0 ? "ru" : "en";
            SettingsManager.Instance?.SetLanguage(language);
        }
        
        public void ShowSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
                
            LoadSettings();
        }
        
        public void HideSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
        
        private void OnDestroy()
        {
            // Remove event listeners
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
                
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
                
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
                
            if (soundToggle != null)
                soundToggle.onValueChanged.RemoveListener(OnSoundToggled);
                
            if (musicToggle != null)
                musicToggle.onValueChanged.RemoveListener(OnMusicToggled);
                
            if (hintsToggle != null)
                hintsToggle.onValueChanged.RemoveListener(OnHintsToggled);
                
            if (languageDropdown != null)
                languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
        }
    }
}