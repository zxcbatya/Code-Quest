using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Core;

namespace RobotCoder.Core
{
    public class ProgramInterpreter : MonoBehaviour
    {
        public static ProgramInterpreter Instance { get; private set; }
        
        [Header("Настройки выполнения")]
        [SerializeField] private float executionSpeed = 1f;
        [SerializeField] private bool isPaused = false;
        [SerializeField] private bool isExecuting = false;
        
        private Queue<CommandBlock> commandQueue = new Queue<CommandBlock>();
        private CommandBlock currentCommand = null;
        private RobotController robot;
        
        public System.Action OnProgramStarted;
        public System.Action OnProgramCompleted;
        public System.Action OnProgramFailed;
        public System.Action<CommandBlock> OnCommandExecuted;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            robot = RobotController.Instance;
        }
        
        public void ExecuteProgram(CommandBlock[] commands)
        {
            if (isExecuting) return;
            
            commandQueue.Clear();
            
            foreach (var command in commands)
            {
                commandQueue.Enqueue(command);
            }
            
            if (commandQueue.Count > 0)
            {
                StartCoroutine(ExecuteProgramCoroutine());
            }
        }
        
        private IEnumerator ExecuteProgramCoroutine()
        {
            isExecuting = true;
            isPaused = false;
            OnProgramStarted?.Invoke();
            
            while (commandQueue.Count > 0 && !isPaused)
            {
                currentCommand = commandQueue.Dequeue();
                
                if (currentCommand != null)
                {
                    // Подсвечиваем текущую команду
                    currentCommand.HighlightExecution();
                    
                    // Выполняем команду
                    bool success = currentCommand.Execute(robot);
                    
                    OnCommandExecuted?.Invoke(currentCommand);
                    
                    if (!success)
                    {
                        // Команда не выполнилась - программа провалена
                        isExecuting = false;
                        OnProgramFailed?.Invoke();
                        yield break;
                    }
                    
                    // Ждем согласно скорости выполнения
                    yield return new WaitForSeconds(1f / executionSpeed);
                    
                    // Ждем пока робот закончит движение
                    while (robot != null && robot.IsMoving())
                    {
                        yield return null;
                    }
                }
            }
            
            isExecuting = false;
            
            if (!isPaused)
            {
                OnProgramCompleted?.Invoke();
            }
        }
        
        public void PauseExecution()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                StopAllCoroutines();
                isExecuting = false;
            }
        }
        
        public void StopExecution()
        {
            StopAllCoroutines();
            commandQueue.Clear();
            isExecuting = false;
            isPaused = false;
            currentCommand = null;
        }
        
        public void SetExecutionSpeed(float speed)
        {
            executionSpeed = Mathf.Clamp(speed, 0.1f, 5f);
        }
        
        public void OnProgramChanged()
        {
            // Вызывается когда программа в рабочей области изменилась
            if (isExecuting)
            {
                StopExecution();
            }
        }
        
        public bool IsExecuting()
        {
            return isExecuting;
        }
        
        public bool IsPaused()
        {
            return isPaused;
        }
        
        public CommandBlock GetCurrentCommand()
        {
            return currentCommand;
        }
        
        public int GetRemainingCommands()
        {
            return commandQueue.Count;
        }
    }
}