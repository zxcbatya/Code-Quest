using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class LevelSelector : MonoBehaviour
    {
        [Header("Level Settings")] [SerializeField]
        private LevelData levelData;

        [SerializeField] private int levelIndex;

        [Header("UI Components")] [SerializeField]
        private Button levelButton;

        [SerializeField] private TextMeshProUGUI levelNumberText;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Image[] starImages;
        [SerializeField] private Image lockIcon;

        [Header("Visual Settings")] [SerializeField]
        private Color unlockedColor = Color.white;

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
            isUnlocked = LevelProgression.Instance?.IsLevelUnlocked(levelIndex) ?? (levelIndex == 1);

            levelNumberText.text = levelIndex.ToString();

            levelNameText.text = levelData.levelName;

            if (isUnlocked)
            {
                levelButton.interactable = true;
                levelButton.onClick.AddListener(OnLevelSelected);
            }
            else
            {
                levelButton.interactable = false;
            }

            UpdateVisualState();

            LoadStarProgress();
        }

        private void UpdateVisualState()
        {
            Color targetColor = isUnlocked ? unlockedColor : lockedColor;

            levelButton.targetGraphic.color = targetColor;

            levelNumberText.color = isUnlocked ? Color.white : Color.gray;

            levelNameText.color = isUnlocked ? Color.white : Color.gray;

            lockIcon.gameObject.SetActive(!isUnlocked);
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

            GameManager.Instance?.SetCurrentLevel(levelIndex);

        }

        public void SetLevelData(LevelData data, int index)
        {
            levelData = data;
            levelIndex = index;
            InitializeLevelSelector();
        }
    }
}