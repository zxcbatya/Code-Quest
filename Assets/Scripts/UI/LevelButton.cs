using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RobotCoder.UI
{
    public class LevelButton : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI levelNumberText;
        [SerializeField] private Image lockIcon;
        [SerializeField] private Image[] stars;

        [Header("Visual States")]
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private Color starActiveColor = Color.yellow;
        [SerializeField] private Color starInactiveColor = Color.gray;

        private int levelIndex;
        private bool isUnlocked;
        private Action<int> onLevelSelected;

        public void Initialize(int levelIndex, bool isUnlocked, Action<int> onLevelSelected)
        {
            this.levelIndex = levelIndex;
            this.isUnlocked = isUnlocked;
            this.onLevelSelected = onLevelSelected;

            SetupButton();
            UpdateVisualState();
            LoadLevelProgress();
        }

        private void SetupButton()
        {
            if (button == null)
                button = GetComponent<Button>();

            levelNumberText.text = levelIndex.ToString();

            if (isUnlocked)
            {
                button.onClick.AddListener(() => onLevelSelected?.Invoke(levelIndex));
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }

        private void UpdateVisualState()
        {
            Color targetColor = isUnlocked ? unlockedColor : lockedColor;
            
            if (button.targetGraphic != null)
            {
                button.targetGraphic.color = targetColor;
            }

            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!isUnlocked);
            }

            levelNumberText.color = isUnlocked ? Color.white : Color.white;
        }

        private void LoadLevelProgress()
        {
            if (!isUnlocked) return;

            string progressKey = $"Level_{levelIndex}_Stars";
            int starsEarned = PlayerPrefs.GetInt(progressKey, 0);

            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    stars[i].color = i < starsEarned ? starActiveColor : starInactiveColor;
                }
            }
        }

        public static void SaveLevelProgress(int levelIndex, int starsEarned)
        {
            string progressKey = $"Level_{levelIndex}_Stars";
            int currentStars = PlayerPrefs.GetInt(progressKey, 0);
            
            if (starsEarned > currentStars)
            {
                PlayerPrefs.SetInt(progressKey, starsEarned);
                PlayerPrefs.Save();
            }
        }
    }
}