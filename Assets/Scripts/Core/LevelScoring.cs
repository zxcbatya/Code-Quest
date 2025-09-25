using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class LevelScoring : MonoBehaviour
    {
        public static LevelScoring Instance { get; private set; }
        
        [Header("Scoring Settings")]
        [SerializeField] private int pointsPerCommandUnderOptimal = 10;
        [SerializeField] private int pointsPerLevelCompleted = 100;
        [SerializeField] private int pointsPerStar = 50;
        [SerializeField] private int pointsPerSecondRemaining = 5;
        
        public System.Action<int> OnScoreChanged;
        
        private int currentScore = 0;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void OnLevelCompleted(int levelIndex, int commandsUsed, int optimalCommands, float timeTaken)
        {
            int levelScore = CalculateLevelScore(commandsUsed, optimalCommands, timeTaken);
            AddScore(levelScore);
            
            Debug.Log($"Уровень {levelIndex} завершен! Очки: {levelScore}");
        }
        
        private int CalculateLevelScore(int commandsUsed, int optimalCommands, float timeTaken)
        {
            int score = pointsPerLevelCompleted;
            
            // Bonus points for using fewer commands than optimal
            if (commandsUsed < optimalCommands)
            {
                int bonus = (optimalCommands - commandsUsed) * pointsPerCommandUnderOptimal;
                score += bonus;
            }
            
            // Calculate stars and add star points
            int stars = CalculateStars(commandsUsed, optimalCommands);
            score += stars * pointsPerStar;
            
            // Time bonus (if implemented)
            // score += Mathf.FloorToInt(timeTaken) * pointsPerSecondRemaining;
            
            return score;
        }
        
        private int CalculateStars(int commandsUsed, int optimalCommands)
        {
            // 3 stars for optimal or better
            if (commandsUsed <= optimalCommands)
                return 3;
                
            // 2 stars for close to optimal
            if (commandsUsed <= optimalCommands + 2)
                return 2;
                
            // 1 star for completing
            if (commandsUsed > optimalCommands)
                return 1;
                
            return 0;
        }
        
        public void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
        }
        
        public void ResetScore()
        {
            currentScore = 0;
            OnScoreChanged?.Invoke(currentScore);
        }
        
        public int GetCurrentScore()
        {
            return currentScore;
        }
        
        // Calculate stars for a completed level
        public int CalculateStarsForLevel(int commandsUsed, LevelData levelData)
        {
            if (levelData == null) return 0;
            
            return CalculateStars(commandsUsed, levelData.optimalCommands);
        }
        
        // Get score description for UI display
        public string GetScoreDescription(int commandsUsed, LevelData levelData)
        {
            if (levelData == null) return "";
            
            int stars = CalculateStarsForLevel(commandsUsed, levelData);
            int score = CalculateLevelScore(commandsUsed, levelData.optimalCommands, 0);
            
            return $"Звезды: {stars}/3 | Очки: {score}";
        }
    }
}