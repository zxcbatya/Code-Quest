using UnityEngine;

namespace Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        [Header("Input Settings")]
        [SerializeField] private KeyCode resetKey = KeyCode.R;
        [SerializeField] private KeyCode startKey = KeyCode.Space;
        [SerializeField] private KeyCode pauseKey = KeyCode.P;
        [SerializeField] private KeyCode skipTutorialKey = KeyCode.T;
        [SerializeField] private bool enableKeyboardShortcuts = true;
        
        public System.Action OnResetPressed;
        public System.Action OnStartPressed;
        public System.Action OnPausePressed;
        public System.Action OnSkipTutorialPressed;
        
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
        
        private void Update()
        {
            if (!enableKeyboardShortcuts) return;
            
            if (Input.GetKeyDown(resetKey))
            {
                OnResetPressed?.Invoke();
            }
            
            if (Input.GetKeyDown(startKey))
            {
                OnStartPressed?.Invoke();
            }
            
            if (Input.GetKeyDown(pauseKey))
            {
                OnPausePressed?.Invoke();
            }
            
            if (Input.GetKeyDown(skipTutorialKey))
            {
                OnSkipTutorialPressed?.Invoke();
            }
        }
        
        public void SetResetKey(KeyCode key)
        {
            resetKey = key;
        }
        
        public void SetStartKey(KeyCode key)
        {
            startKey = key;
        }
        
        public void SetPauseKey(KeyCode key)
        {
            pauseKey = key;
        }
        
        public void SetSkipTutorialKey(KeyCode key)
        {
            skipTutorialKey = key;
        }
        
        public void SetKeyboardShortcutsEnabled(bool enabled)
        {
            enableKeyboardShortcuts = enabled;
        }
        
        public bool AreKeyboardShortcutsEnabled()
        {
            return enableKeyboardShortcuts;
        }
        
        // Добавляем методы для регистрации и отмены регистрации горячих клавиш
        public void RegisterKeyAction(KeyCode key, System.Action action)
        {
            // Этот метод для совместимости с RobotCoder.UI.InputManager
            if (key == resetKey) OnResetPressed += action;
            if (key == startKey) OnStartPressed += action;
            if (key == pauseKey) OnPausePressed += action;
            if (key == skipTutorialKey) OnSkipTutorialPressed += action;
        }
        
        public void UnregisterKeyAction(KeyCode key)
        {
            // Этот метод для совместимости с RobotCoder.UI.InputManager
            if (key == resetKey) OnResetPressed = null;
            if (key == startKey) OnStartPressed = null;
            if (key == pauseKey) OnPausePressed = null;
            if (key == skipTutorialKey) OnSkipTutorialPressed = null;
        }
    }
}