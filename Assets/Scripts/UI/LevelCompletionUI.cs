using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class LevelCompletionUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject completionPanel;
        [SerializeField] private TextMeshProUGUI levelCompleteText;
        [SerializeField] private TextMeshProUGUI commandsText;
        [SerializeField] private TextMeshProUGUI optimalCommandsText;
        [SerializeField] private Image[] starImages;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;
        
        [Header("Visual Settings")]
        [SerializeField] private Color starActiveColor = Color.yellow;
        [SerializeField] private Color starInactiveColor = Color.gray;
        
        private int earnedStars = 0;
        private int levelIndex = 0;
        
        private void Start()
        {
            SetupEventListeners();
        }
        
        private void SetupEventListeners()
        {
            if (nextLevelButton != null)
                nextLevelButton.onClick.AddListener(OnNextLevelClicked);
                
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);
                
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
        }
        
        public void ShowLevelCompletion(int levelIdx, int commandsUsed, int optimalCommands, int maxCommands)
        {
            levelIndex = levelIdx;
            
            // Calculate stars
            earnedStars = CalculateStars(commandsUsed, optimalCommands, maxCommands);
            
            // Save progress
            LevelProgression.Instance?.SaveLevelStars(levelIndex, earnedStars);
            LevelProgression.Instance?.UnlockLevel(levelIndex + 1);
            
            // Update UI
            if (levelCompleteText != null)
                levelCompleteText.text = $"Уровень {levelIndex} пройден!";
                
            if (commandsText != null)
                commandsText.text = $"Команд использовано: {commandsUsed}";
                
            if (optimalCommandsText != null)
                optimalCommandsText.text = $"Оптимально: {optimalCommands}";
                
            // Update star display
            UpdateStarDisplay();
            
            // Show panel
            if (completionPanel != null)
                completionPanel.SetActive(true);
        }
        
        private int CalculateStars(int commandsUsed, int optimalCommands, int maxCommands)
        {
            // 3 stars for optimal or better
            if (commandsUsed <= optimalCommands)
                return 3;
                
            // 2 stars for close to optimal
            if (commandsUsed <= optimalCommands + 2)
                return 2;
                
            // 1 star for completing within max commands
            if (commandsUsed <= maxCommands)
                return 1;
                
            // 0 stars if exceeded max commands
            return 0;
        }
        
        private void UpdateStarDisplay()
        {
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    starImages[i].color = i < earnedStars ? starActiveColor : starInactiveColor;
                }
            }
        }
        
        private void OnNextLevelClicked()
        {
            GameManager.Instance?.NextLevel();
            HideCompletionPanel();
        }
        
        private void OnRetryClicked()
        {
            GameManager.Instance?.ResetProgram();
            HideCompletionPanel();
        }
        
        private void OnMenuClicked()
        {
            // Return to main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        
        private void HideCompletionPanel()
        {
            if (completionPanel != null)
                completionPanel.SetActive(false);
        }
    }
}