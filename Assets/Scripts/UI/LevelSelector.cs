using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class LevelSelector : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private LevelData levelData;
        [SerializeField] private int levelIndex;
        
        [Header("UI Components")]
        [SerializeField] private Button levelButton;
        [SerializeField] private TextMeshProUGUI levelNumberText;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Image[] starImages;
        [SerializeField] private Image lockIcon;
        
        [Header("Visual Settings")]
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private Color starActiveColor = Color.yellow;
        [SerializeField] private Color starInactiveColor = Color.gray;
        
        private bool isUnlocked;
        
        private void Start()
        {
            InitializeLevelSelector();
        }
        
        private void InitializeLevelSelector()
        {
            // Check if level is unlocked
            isUnlocked = LevelProgression.Instance?.IsLevelUnlocked(levelIndex) ?? (levelIndex == 1);
            
            // Setup UI elements
            if (levelNumberText != null)
                levelNumberText.text = levelIndex.ToString();
                
            if (levelNameText != null && levelData != null)
                levelNameText.text = levelData.levelName;
                
            // Setup button
            if (levelButton != null)
            {
                if (isUnlocked)
                {
                    levelButton.interactable = true;
                    levelButton.onClick.AddListener(OnLevelSelected);
                }
                else
                {
                    levelButton.interactable = false;
                }
            }
            
            // Update visual state
            UpdateVisualState();
            
            // Load star progress
            LoadStarProgress();
        }
        
        private void UpdateVisualState()
        {
            Color targetColor = isUnlocked ? unlockedColor : lockedColor;
            
            if (levelButton != null && levelButton.targetGraphic != null)
            {
                levelButton.targetGraphic.color = targetColor;
            }
            
            if (levelNumberText != null)
            {
                levelNumberText.color = isUnlocked ? Color.white : Color.gray;
            }
            
            if (levelNameText != null)
            {
                levelNameText.color = isUnlocked ? Color.white : Color.gray;
            }
            
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!isUnlocked);
            }
        }
        
        private void LoadStarProgress()
        {
            if (!isUnlocked || starImages.Length == 0) return;
            
            int starsEarned = LevelProgression.Instance?.GetLevelStars(levelIndex) ?? 0;
            
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    starImages[i].color = i < starsEarned ? starActiveColor : starInactiveColor;
                }
            }
        }
        
        private void OnLevelSelected()
        {
            if (!isUnlocked) return;
            
            // Load the level
            GameManager.Instance?.SetCurrentLevel(levelIndex);
            
            // You could also load a specific scene here
            // SceneManager.LoadScene($"Level_{levelIndex:D2}");
        }
        
        public void SetLevelData(LevelData data, int index)
        {
            levelData = data;
            levelIndex = index;
            InitializeLevelSelector();
        }
    }
}