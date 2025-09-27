using UnityEngine;

namespace Core
{
    public class CommandCounter : MonoBehaviour
    {
        public static CommandCounter Instance { get; private set; }
        
        [Header("Command Tracking")]
        [SerializeField] private int totalCommandsExecuted = 0;
        [SerializeField] private int currentProgramCommands = 0;
        
        public System.Action<int> OnCommandCountChanged;
        public System.Action<int> OnTotalCommandCountChanged;
        
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
        
        private void Start()
        {
            // Subscribe to program interpreter events
            if (ProgramInterpreter.Instance != null)
            {
                ProgramInterpreter.Instance.OnProgramStarted += OnProgramStarted;
                ProgramInterpreter.Instance.OnCommandExecuted += OnCommandExecuted;
                ProgramInterpreter.Instance.OnProgramCompleted += OnProgramCompleted;
                ProgramInterpreter.Instance.OnProgramFailed += OnProgramCompleted;
            }
        }
        
        private void OnProgramStarted()
        {
            currentProgramCommands = 0;
            OnCommandCountChanged?.Invoke(currentProgramCommands);
        }
        
        private void OnCommandExecuted(CommandBlock command)
        {
            currentProgramCommands++;
            totalCommandsExecuted++;
            
            OnCommandCountChanged?.Invoke(currentProgramCommands);
            OnTotalCommandCountChanged?.Invoke(totalCommandsExecuted);
        }
        
        private void OnProgramCompleted()
        {
            // Program finished, keep the command count for scoring
        }
        
        public int GetCurrentProgramCommandCount()
        {
            return currentProgramCommands;
        }
        
        public int GetTotalCommandCount()
        {
            return totalCommandsExecuted;
        }
        
        public void ResetCurrentProgramCount()
        {
            currentProgramCommands = 0;
            OnCommandCountChanged?.Invoke(currentProgramCommands);
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (ProgramInterpreter.Instance != null)
            {
                ProgramInterpreter.Instance.OnProgramStarted -= OnProgramStarted;
                ProgramInterpreter.Instance.OnCommandExecuted -= OnCommandExecuted;
                ProgramInterpreter.Instance.OnProgramCompleted -= OnProgramCompleted;
                ProgramInterpreter.Instance.OnProgramFailed -= OnProgramCompleted;
            }
        }
    }
}