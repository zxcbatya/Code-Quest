using System.Collections;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image[] goalIcons;
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private Button hintButton;

        [SerializeField] private Color completedGoalColor = Color.green;
        [SerializeField] private Color incompleteGoalColor = Color.gray;
        [SerializeField] private Color progressBarColor = Color.blue;

        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve progressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private int _totalGoals = 1;
        private int _completedGoals = 0;
        private string[] _levelHints;
        private int _currentHintIndex = 0;
        private Coroutine _hideHintCoroutine;

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
            
            _totalGoals = goalCount;
            _completedGoals = 0;
            
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
                    goalIcons[i].gameObject.SetActive(i < _totalGoals);
                    goalIcons[i].color = incompleteGoalColor;
                }
            }
        }

        public void UpdateProgress(int completed)
        {
            int oldCompleted = _completedGoals;
            _completedGoals = Mathf.Clamp(completed, 0, _totalGoals);
            
            StartCoroutine(AnimateProgress(oldCompleted, _completedGoals));
            UpdateGoalIcons();
            
            if (_completedGoals >= _totalGoals)
            {
                OnAllGoalsCompleted?.Invoke();
            }
        }

        private void UpdateGoalIcons()
        {
            if (goalIcons == null) return;

            for (int i = 0; i < goalIcons.Length && i < _totalGoals; i++)
            {
                if (goalIcons[i] != null)
                {
                    Color targetColor = i < _completedGoals ? completedGoalColor : incompleteGoalColor;
                    StartCoroutine(AnimateIconColor(goalIcons[i], targetColor));
                }
            }
        }

        private IEnumerator AnimateProgress(int fromProgress, int toProgress)
        {
            float startTime = Time.time;
            float fromValue = (float)fromProgress / _totalGoals;
            float toValue = (float)toProgress / _totalGoals;

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
                int displayProgress = Mathf.RoundToInt(progress * _totalGoals);
                progressText.text = $"{displayProgress}/{_totalGoals}";
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
                progressSlider.value = _totalGoals > 0 ? (float)_completedGoals / _totalGoals : 0f;
                progressSlider.fillRect.GetComponent<Image>().color = progressBarColor;
            }
            
            UpdateProgressText(_totalGoals > 0 ? (float)_completedGoals / _totalGoals : 0f);
        }

        private void LoadLevelHints()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            _levelHints = new string[]
            {
                "Перетащите блоки из палитры в рабочую область",
                "Используйте кнопку СТАРТ для запуска программы",
                "Робот выполняет команды по порядку сверху вниз",
                "Кнопка СБРОС возвращает робота в начальную позицию"
            };
        }

        public void ShowNextHint()
        {
            if (_levelHints == null || _levelHints.Length == 0) return;
            
            if (hintPanel && !hintPanel.activeInHierarchy)
            {
                hintPanel.SetActive(true);
                StartCoroutine(AnimateHintPanel(true));
            }
            
            if (hintText)
            {
                hintText.text = _levelHints[_currentHintIndex];
                _currentHintIndex = (_currentHintIndex + 1) % _levelHints.Length;
            }
            
            RestartHideTimer();
            AudioManager.Instance?.PlaySound("button_click");
        }

        private void RestartHideTimer()
        {
            if (_hideHintCoroutine != null)
            {
                StopCoroutine(_hideHintCoroutine);
            }
            
            _hideHintCoroutine = StartCoroutine(HideHintAfterDelay(3f));
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
            
            if (show)
            {
                hintPanel.SetActive(true);
            }
            
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
            if (goalIndex >= 0 && goalIndex < _totalGoals)
            {
                UpdateProgress(_completedGoals + 1);
            }
        }

        public void ResetProgress()
        {
            _completedGoals = 0;
            UpdateProgressDisplay();
            UpdateGoalIcons();
        }
    }
}