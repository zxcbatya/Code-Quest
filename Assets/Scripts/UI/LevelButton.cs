using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
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

        private int _levelIndex;
        private bool _isUnlocked;
        private Action<int> _onLevelSelected;

        public void Initialize(int levelIndex, bool isUnlocked, Action<int> onLevelSelected)
        {
            this._levelIndex = levelIndex;
            this._isUnlocked = isUnlocked;
            this._onLevelSelected = onLevelSelected;

            SetupButton();
            UpdateVisualState();
            LoadLevelProgress();
        }

        private void SetupButton()
        {
            if (button == null)
                button = GetComponent<Button>();

            levelNumberText.text = _levelIndex.ToString();

            if (_isUnlocked)
            {
                button.onClick.AddListener(() => _onLevelSelected?.Invoke(_levelIndex));
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }

        private void UpdateVisualState()
        {
            Color targetColor = _isUnlocked ? unlockedColor : lockedColor;
            
            if (button.targetGraphic != null)
            {
                button.targetGraphic.color = targetColor;
            }

            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!_isUnlocked);
            }

            levelNumberText.color = Color.white;
        }

        private void LoadLevelProgress()
        {
            if (!_isUnlocked) return;

            string progressKey = $"Level_{_levelIndex}_Stars";
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