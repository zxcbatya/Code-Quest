using UnityEngine;

namespace Core
{
    public enum GameState
    {
        MainMenu,
        LevelSelect,
        Playing,
        Paused,
        LevelCompleted,
        GameOver
    }
    
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        
        public System.Action<GameState> OnGameStateChanged;
        
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
        
        public void ChangeState(GameState newState)
        {
            GameState previousState = currentState;
            currentState = newState;
            
            OnGameStateChanged?.Invoke(currentState);
            
            Debug.Log($"Game state changed from {previousState} to {newState}");
        }
        
        public GameState GetCurrentState()
        {
            return currentState;
        }
        
        public bool IsGameState(GameState state)
        {
            return currentState == state;
        }
        
        public bool IsPlaying()
        {
            return currentState == GameState.Playing;
        }
        
        public bool IsPaused()
        {
            return currentState == GameState.Paused;
        }
    }
}