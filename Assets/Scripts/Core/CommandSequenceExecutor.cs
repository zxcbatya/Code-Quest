using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class CommandSequenceExecutor : MonoBehaviour
    {
        [Header("Execution Settings")]
        [SerializeField] private float executionDelay = 0.5f;
        [SerializeField] private bool waitForRobotMovement = true;
        [SerializeField] private int maxExecutionSteps = 1000; // Safety limit
        
        private RobotController robotController;
        private bool isExecuting = false;
        private Coroutine executionCoroutine;
        
        public System.Action OnExecutionStarted;
        public System.Action OnExecutionCompleted;
        public System.Action OnExecutionCancelled;
        public System.Action<CommandType> OnCommandExecuted;
        
        private void Start()
        {
            robotController = RobotController.Instance;
        }
        
        // Execute a sequence of commands
        public void ExecuteCommandSequence(List<CommandType> commands)
        {
            if (isExecuting || commands == null || commands.Count == 0) return;
            
            isExecuting = true;
            OnExecutionStarted?.Invoke();
            executionCoroutine = StartCoroutine(ExecuteSequence(commands));
        }
        
        // Coroutine to execute command sequence
        private IEnumerator ExecuteSequence(List<CommandType> commands)
        {
            int stepsExecuted = 0;
            
            foreach (CommandType command in commands)
            {
                // Safety check
                if (stepsExecuted >= maxExecutionSteps)
                {
                    Debug.LogWarning("Превышено максимальное количество шагов выполнения");
                    break;
                }
                
                // Execute the command
                bool success = ExecuteCommand(command);
                
                // Notify listeners
                OnCommandExecuted?.Invoke(command);
                
                stepsExecuted++;
                
                // Check if command executed successfully
                if (!success)
                {
                    Debug.LogWarning($"Команда {command} не выполнена успешно");
                    // Depending on requirements, you might want to stop execution here
                }
                
                // Wait for execution delay
                if (executionDelay > 0)
                {
                    yield return new WaitForSeconds(executionDelay);
                }
                
                // Wait for robot to finish moving if needed
                if (waitForRobotMovement && robotController != null)
                {
                    while (robotController.IsMoving())
                    {
                        yield return null;
                    }
                }
            }
            
            // Execution completed
            isExecuting = false;
            executionCoroutine = null;
            OnExecutionCompleted?.Invoke();
        }
        
        // Execute a single command
        private bool ExecuteCommand(CommandType command)
        {
            if (robotController == null) return false;
            
            switch (command)
            {
                case CommandType.MoveForward:
                    return robotController.MoveForward();
                    
                case CommandType.TurnLeft:
                    return robotController.TurnLeft();
                    
                case CommandType.TurnRight:
                    return robotController.TurnRight();
                    
                case CommandType.Jump:
                    return robotController.Jump();
                    
                case CommandType.Interact:
                    return robotController.Interact();
                    
                case CommandType.Repeat:
                    // Repeat commands are handled differently
                    Debug.Log("Команда повтора требует специальной обработки");
                    return true;
                    
                case CommandType.If:
                    // If commands are handled differently
                    Debug.Log("Команда условия требует специальной обработки");
                    return true;
                    
                case CommandType.Else:
                    // Else commands are handled differently
                    Debug.Log("Команда иначе требует специальной обработки");
                    return true;
                    
                default:
                    Debug.LogWarning($"Неизвестная команда: {command}");
                    return false;
            }
        }
        
        // Cancel current execution
        public void CancelExecution()
        {
            if (!isExecuting) return;
            
            isExecuting = false;
            
            if (executionCoroutine != null)
            {
                StopCoroutine(executionCoroutine);
                executionCoroutine = null;
            }
            
            OnExecutionCancelled?.Invoke();
        }
        
        // Check if executor is currently executing
        public bool IsExecuting()
        {
            return isExecuting;
        }
        
        // Set execution delay
        public void SetExecutionDelay(float delay)
        {
            executionDelay = Mathf.Max(0, delay);
        }
        
        // Set whether to wait for robot movement
        public void SetWaitForRobotMovement(bool wait)
        {
            waitForRobotMovement = wait;
        }
        
        // Set maximum execution steps
        public void SetMaxExecutionSteps(int maxSteps)
        {
            maxExecutionSteps = Mathf.Max(1, maxSteps);
        }
        
        private void OnDestroy()
        {
            CancelExecution();
        }
    }
}