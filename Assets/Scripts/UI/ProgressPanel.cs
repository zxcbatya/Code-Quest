using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Core;

namespace RobotCoder.UI
{
    public class ProgressPanel : MonoBehaviour
    {
        [Header("UI Элементы")]
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image[] goalIcons;
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private Button hintButton;

        [Header("Цвета")]
        [SerializeField] private Color completedGoalColor = Color.green;
        [SerializeField] private Color incompleteGoalColor = Color.gray;
        [SerializeField] private Color progressBarColor = Color.blue;

        [Header("Анимации")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve progressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private int totalGoals = 1;
        private int completedGoals = 0;
        private string[] levelHints;
        private int currentHintIndex = 0;

        public System.Action OnAllGoalsCompleted;

        private void Start()
        {
            InitializePanel();
            SetupEventListeners();
        }

        private void InitializePanel()
        {
            if (hintPanel) hintPanel.SetActive(false);
            UpdateProgressDisplay();
            LoadLevelHints();
        }

        private void SetupEventListeners()
        {
            if (hintButton) hintButton.onClick.AddListener(ShowNextHint);
        }

        public void SetLevelInfo(string title, string objective, int goalCount)
        {
            if (levelTitleText) levelTitleText.text = title;
            if (objectiveText) objectiveText.text = objective;
            
            totalGoals = goalCount;
            completedGoals = 0;
            
            SetupGoalIcons();
            UpdateProgressDisplay();
        }

        private void SetupGoalIcons()
        {
            if (goalIcons == null) return;

            for (int i = 0; i < goalIcons.Length; i++)
            {
                if (goalIcons[i] != null)
                {
                    goalIcons[i].gameObject.SetActive(i < totalGoals);
                    goalIcons[i].color = incompleteGoalColor;
                }
            }
        }

        public void UpdateProgress(int completed)
        {
            int oldCompleted = completedGoals;
            completedGoals = Mathf.Clamp(completed, 0, totalGoals);
            
            StartCoroutine(AnimateProgress(oldCompleted, completedGoals));
            UpdateGoalIcons();
            
            if (completedGoals >= totalGoals)
            {
                OnAllGoalsCompleted?.Invoke();
            }
        }

        private void UpdateGoalIcons()
        {
            if (goalIcons == null) return;

            for (int i = 0; i < goalIcons.Length && i < totalGoals; i++)
            {
                if (goalIcons[i] != null)
                {
                    Color targetColor = i < completedGoals ? completedGoalColor : incompleteGoalColor;
                    StartCoroutine(AnimateIconColor(goalIcons[i], targetColor));
                }
            }
        }

        private IEnumerator AnimateProgress(int fromProgress, int toProgress)
        {
            float startTime = Time.time;
            float fromValue = (float)fromProgress / totalGoals;
            float toValue = (float)toProgress / totalGoals;

            while (Time.time - startTime < animationDuration)
            {
                float progress = (Time.time - startTime) / animationDuration;
                float curveProgress = progressCurve.Evaluate(progress);
                
                float currentValue = Mathf.Lerp(fromValue, toValue, curveProgress);
                
                if (progressSlider) progressSlider.value = currentValue;
                UpdateProgressText(currentValue);
                
                yield return null;
            }

            if (progressSlider) progressSlider.value = toValue;
            UpdateProgressText(toValue);
        }

        private void UpdateProgressText(float progress)
        {
            if (progressText != null)
            {
                int displayProgress = Mathf.RoundToInt(progress * totalGoals);
                progressText.text = $"{displayProgress}/{totalGoals}";
            }
        }

        private IEnumerator AnimateIconColor(Image icon, Color targetColor)
        {
            Color startColor = icon.color;
            float startTime = Time.time;

            while (Time.time - startTime < animationDuration * 0.5f)
            {
                float progress = (Time.time - startTime) / (animationDuration * 0.5f);
                icon.color = Color.Lerp(startColor, targetColor, progress);
                yield return null;
            }

            icon.color = targetColor;
        }

        private void UpdateProgressDisplay()
        {
            if (progressSlider) 
            {
                progressSlider.value = totalGoals > 0 ? (float)completedGoals / totalGoals : 0f;
                progressSlider.fillRect.GetComponent<Image>().color = progressBarColor;
            }
            
            UpdateProgressText(totalGoals > 0 ? (float)completedGoals / totalGoals : 0f);
        }

        private void LoadLevelHints()
        {
            // Загружаем подсказки для текущего уровня
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Примеры подсказок (в реальном проекте загружать из локализации)
            levelHints = new string[]
            {
                "Перетащите блоки из палитры в рабочую область",
                "Используйте кнопку СТАРТ для запуска программы",
                "Робот выполняет команды по порядку сверху вниз",
                "Кнопка СБРОС возвращает робота в начальную позицию"
            };
        }

        public void ShowNextHint()
        {
            if (levelHints == null || levelHints.Length == 0) return;
            
            if (hintPanel) hintPanel.SetActive(true);
            
            if (hintText)
            {
                hintText.text = levelHints[currentHintIndex];
                currentHintIndex = (currentHintIndex + 1) % levelHints.Length;
            }
            
            StartCoroutine(HideHintAfterDelay(3f));
            AudioManager.Instance?.PlaySound("button_click");
        }

        private IEnumerator HideHintAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (hintPanel) 
            {
                StartCoroutine(AnimateHintPanel(false));
            }
        }

        private IEnumerator AnimateHintPanel(bool show)
        {
            if (!hintPanel) yield break;
            
            CanvasGroup canvasGroup = hintPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = hintPanel.AddComponent<CanvasGroup>();
            
            float startAlpha = show ? 0f : 1f;
            float endAlpha = show ? 1f : 0f;
            float duration = 0.3f;
            float startTime = Time.time;
            
            while (Time.time - startTime < duration)
            {
                float progress = (Time.time - startTime) / duration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
                yield return null;
            }
            
            canvasGroup.alpha = endAlpha;
            
            if (!show)
            {
                hintPanel.SetActive(false);
            }
        }

        public void CompleteGoal(int goalIndex)
        {
            if (goalIndex >= 0 && goalIndex < totalGoals)
            {
                UpdateProgress(completedGoals + 1);
            }
        }

        public void ResetProgress()
        {
            completedGoals = 0;
            UpdateProgressDisplay();
            UpdateGoalIcons();
        }
    }
}