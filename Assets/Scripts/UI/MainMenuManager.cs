using System.Collections;
using RobotCoder.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Core; // Add this using directive

namespace UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Menu Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button levelsButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Level Select")]
        [SerializeField] private Transform levelGrid;
        [SerializeField] private GameObject levelButtonPrefab;
        [SerializeField] private Button levelBackButton;

        [Header("Settings")]
        [SerializeField] private Button settingsBackButton;
        [SerializeField] private Slider audioSlider;
        [SerializeField] private Toggle russianToggle;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip buttonClickSound;

        private const int TOTAL_LEVELS = 15;
        private const string LEVEL_PROGRESS_KEY = "LevelProgress";
        private const string AUDIO_VOLUME_KEY = "AudioVolume";
        private const string LANGUAGE_KEY = "Language";

        private void Start()
        {
            InitializeMenu();
            SetupEventListeners();
            LoadSettings();
        }

        private void InitializeMenu()
        {
            ShowMenuPanel();
            GenerateLevelButtons();
        }

        private void SetupEventListeners()
        {
            // Очищаем существующие слушатели
            ClearEventListeners();
            
            playButton.onClick.AddListener(() => StartGame());
            levelsButton.onClick.AddListener(() => ShowLevelSelect());
            settingsButton.onClick.AddListener(() => ShowSettings());
            quitButton.onClick.AddListener(() => QuitGame());

            levelBackButton.onClick.AddListener(() => ShowMenuPanel());
            settingsBackButton.onClick.AddListener(() => ShowMenuPanel());

            audioSlider.onValueChanged.AddListener(OnAudioVolumeChanged);
            russianToggle.onValueChanged.AddListener(OnLanguageChanged);
        }
        
        private void ClearEventListeners()
        {
            if (playButton != null) playButton.onClick.RemoveAllListeners();
            if (levelsButton != null) levelsButton.onClick.RemoveAllListeners();
            if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
            if (quitButton != null) quitButton.onClick.RemoveAllListeners();
            if (levelBackButton != null) levelBackButton.onClick.RemoveAllListeners();
            if (settingsBackButton != null) settingsBackButton.onClick.RemoveAllListeners();
        }

        private void GenerateLevelButtons()
        {
            // Очищаем существующие кнопки уровней
            if (levelGrid != null)
            {
                for (int i = levelGrid.childCount - 1; i >= 0; i--)
                {
                    Destroy(levelGrid.GetChild(i).gameObject);
                }
            }
            
            int unlockedLevels = PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 1);

            for (int i = 1; i <= TOTAL_LEVELS; i++)
            {
                if (levelButtonPrefab != null && levelGrid != null)
                {
                    GameObject levelButtonObj = Instantiate(levelButtonPrefab, levelGrid);
                    LevelButton levelButton = levelButtonObj.GetComponent<LevelButton>();
                    
                    if (levelButton != null)
                    {
                        bool isUnlocked = i <= unlockedLevels;
                        levelButton.Initialize(i, isUnlocked, OnLevelSelected);
                    }
                }
            }
        }

        private void OnLevelSelected(int levelIndex)
        {
            PlayButtonSound();
            // Используем SceneCleanupManager для загрузки сцены с очисткой
            SceneCleanupManager.Instance?.LoadSceneWithCleanup($"Level_{levelIndex:D2}");
        }

        private void StartGame()
        {
            PlayButtonSound();
            // Используем SceneCleanupManager для загрузки сцены с очисткой
            SceneCleanupManager.Instance?.LoadSceneWithCleanup("Level_01");
        }

        private void ShowMenuPanel()
        {
            PlayButtonSound();
            if (menuPanel != null) menuPanel.SetActive(true);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        private void ShowLevelSelect()
        {
            PlayButtonSound();
            if (menuPanel != null) menuPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        private void ShowSettings()
        {
            PlayButtonSound();
            if (menuPanel != null) menuPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        private void QuitGame()
        {
            PlayButtonSound();
            
            // Принудительная очистка перед выходом
            SceneCleanupManager.Instance?.ForceCleanup();
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void OnAudioVolumeChanged(float value)
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat(AUDIO_VOLUME_KEY, value);
            PlayerPrefs.Save();
        }

        private void OnLanguageChanged(bool isRussian)
        {
            string language = isRussian ? "RU" : "EN";
            PlayerPrefs.SetString(LANGUAGE_KEY, language);
            PlayerPrefs.Save();
            
            // Здесь можно добавить систему локализации
            LocalizationManager.Instance?.SetLanguage(language);
        }

        private void LoadSettings()
        {
            float audioVolume = PlayerPrefs.GetFloat(AUDIO_VOLUME_KEY, 1.0f);
            if (audioSlider != null)
            {
                audioSlider.value = audioVolume;
            }
            AudioListener.volume = audioVolume;

            string language = PlayerPrefs.GetString(LANGUAGE_KEY, "RU");
            if (russianToggle != null)
            {
                russianToggle.isOn = language == "RU";
            }
        }

        private void PlayButtonSound()
        {
            if (audioSource != null && buttonClickSound != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
            }
        }

        public static void UnlockLevel(int levelIndex)
        {
            int currentProgress = PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 1);
            if (levelIndex > currentProgress)
            {
                PlayerPrefs.SetInt(LEVEL_PROGRESS_KEY, levelIndex);
                PlayerPrefs.Save();
            }
        }
        
        private void OnDestroy()
        {
            // Очищаем слушатели событий
            ClearEventListeners();
        }
        
        private void OnDisable()
        {
            // Очищаем слушатели событий при отключении
            ClearEventListeners();
        }
    }
}