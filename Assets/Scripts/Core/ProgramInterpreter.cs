using System.Collections;
using System.Collections.Generic;
using RobotCoder.Core;
using UnityEngine;

namespace Core
{
    public class ProgramInterpreter : MonoBehaviour
    {
        public static ProgramInterpreter Instance { get; private set; }
        
        [Header("Настройки выполнения")]
        [SerializeField] private float executionSpeed = 1f;
        [SerializeField] private bool isPaused = false;
        [SerializeField] private bool isExecuting = false;
        
        private readonly Queue<CommandBlock> _commandQueue = new Queue<CommandBlock>();
        private CommandBlock _currentCommand = null;
        private RobotController _robot;
        
        public System.Action OnProgramStarted;
        public System.Action OnProgramCompleted;
        public System.Action OnProgramFailed;
        public System.Action<CommandBlock> OnCommandExecuted;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Проверяем, является ли объект корневым перед применением DontDestroyOnLoad
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            _robot = RobotController.Instance;
        }
        
        public void ExecuteProgram(CommandBlock[] commands)
        {
            if (isExecuting) return;
            
            _commandQueue.Clear();
            
            foreach (var command in commands)
            {
                _commandQueue.Enqueue(command);
            }
            
            if (_commandQueue.Count > 0)
            {
                StartCoroutine(ExecuteProgramCoroutine());
            }
        }
        
        private IEnumerator ExecuteProgramCoroutine()
        {
            isExecuting = true;
            isPaused = false;
            OnProgramStarted?.Invoke();
            
            while (_commandQueue.Count > 0 && !isPaused)
            {
                _currentCommand = _commandQueue.Dequeue();
                
                if (_currentCommand != null)
                {
                    // Подсвечиваем текущую команду
                    _currentCommand.HighlightExecution();
                    
                    // Выполняем команду
                    bool success = _currentCommand.Execute(_robot);
                    
                    OnCommandExecuted?.Invoke(_currentCommand);
                    
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
                    while (_robot != null && _robot.IsMoving())
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
            _commandQueue.Clear();
            isExecuting = false;
            isPaused = false;
            _currentCommand = null;
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
            return _currentCommand;
        }
        
        public int GetRemainingCommands()
        {
            return _commandQueue.Count;
        }
        
        private void OnDestroy()
        {
            // Очищаем события при уничтожении объекта
            StopExecution();
            OnProgramStarted = null;
            OnProgramCompleted = null;
            OnProgramFailed = null;
            OnCommandExecuted = null;
        }
    }
}