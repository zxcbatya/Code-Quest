using UnityEngine;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Header("Display Settings")]
        [SerializeField] private string scorePrefix = "Очки: ";
        
        private void Start()
        {
            InitializeScoreDisplay();
        }
        
        private void InitializeScoreDisplay()
        {
            if (LevelScoring.Instance != null)
            {
                LevelScoring.Instance.OnScoreChanged += OnScoreChanged;
                UpdateScoreDisplay(LevelScoring.Instance.GetCurrentScore());
            }
        }
        
        private void OnScoreChanged(int newScore)
        {
            UpdateScoreDisplay(newScore);
        }
        
        private void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = scorePrefix + score.ToString();
            }
        }
        
        private void OnDestroy()
        {
            if (LevelScoring.Instance != null)
            {
                LevelScoring.Instance.OnScoreChanged -= OnScoreChanged;
            }
        }
    }
}