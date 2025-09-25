using UnityEngine;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class TimerDisplay : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI timerText;
        
        [Header("Display Settings")]
        [SerializeField] private string timerPrefix = "Время: ";
        [SerializeField] private string timeFormat = "mm\\:ss";
        
        private void Start()
        {
            InitializeTimerDisplay();
        }
        
        private void InitializeTimerDisplay()
        {
            if (LevelTimer.Instance != null)
            {
                LevelTimer.Instance.OnTimeChanged += OnTimeChanged;
                UpdateTimerDisplay(LevelTimer.Instance.GetCurrentTime());
            }
        }
        
        private void OnTimeChanged(float newTime)
        {
            UpdateTimerDisplay(newTime);
        }
        
        private void UpdateTimerDisplay(float time)
        {
            if (timerText != null)
            {
                System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);
                timerText.text = timerPrefix + timeSpan.ToString(timeFormat);
            }
        }
        
        private void OnDestroy()
        {
            if (LevelTimer.Instance != null)
            {
                LevelTimer.Instance.OnTimeChanged -= OnTimeChanged;
            }
        }
    }
}