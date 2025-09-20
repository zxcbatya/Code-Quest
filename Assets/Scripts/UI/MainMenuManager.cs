using System.Collections;
using RobotCoder.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            playButton.onClick.AddListener(() => StartGame());
            levelsButton.onClick.AddListener(() => ShowLevelSelect());
            settingsButton.onClick.AddListener(() => ShowSettings());
            quitButton.onClick.AddListener(() => QuitGame());

            levelBackButton.onClick.AddListener(() => ShowMenuPanel());
            settingsBackButton.onClick.AddListener(() => ShowMenuPanel());

            audioSlider.onValueChanged.AddListener(OnAudioVolumeChanged);
            russianToggle.onValueChanged.AddListener(OnLanguageChanged);
        }

        private void GenerateLevelButtons()
        {
            int unlockedLevels = PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 1);

            for (int i = 1; i <= TOTAL_LEVELS; i++)
            {
                GameObject levelButtonObj = Instantiate(levelButtonPrefab, levelGrid);
                LevelButton levelButton = levelButtonObj.GetComponent<LevelButton>();
                
                bool isUnlocked = i <= unlockedLevels;
                levelButton.Initialize(i, isUnlocked, OnLevelSelected);
            }
        }

        private void OnLevelSelected(int levelIndex)
        {
            PlayButtonSound();
            StartCoroutine(LoadLevelWithDelay(levelIndex));
        }

        private IEnumerator LoadLevelWithDelay(int levelIndex)
        {
            yield return new WaitForSeconds(0.1f);
            SceneManager.LoadScene($"Level_{levelIndex:D2}");
        }

        private void StartGame()
        {
            PlayButtonSound();
            StartCoroutine(LoadLevelWithDelay(1));
        }

        private void ShowMenuPanel()
        {
            PlayButtonSound();
            menuPanel.SetActive(true);
            levelSelectPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }

        private void ShowLevelSelect()
        {
            PlayButtonSound();
            menuPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }

        private void ShowSettings()
        {
            PlayButtonSound();
            menuPanel.SetActive(false);
            levelSelectPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        private void QuitGame()
        {
            PlayButtonSound();
            
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
            audioSlider.value = audioVolume;
            AudioListener.volume = audioVolume;

            string language = PlayerPrefs.GetString(LANGUAGE_KEY, "RU");
            russianToggle.isOn = language == "RU";
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
    }
}