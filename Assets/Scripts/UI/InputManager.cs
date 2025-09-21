using UnityEngine;
using System.Collections.Generic;

namespace RobotCoder.UI
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private Dictionary<KeyCode, System.Action> keyActions = new Dictionary<KeyCode, System.Action>();
        private Dictionary<string, System.Action> namedActions = new Dictionary<string, System.Action>();

        [Header("Настройки ввода")]
        [SerializeField] private bool enableKeyboardShortcuts = true;
        [SerializeField] private bool enableMobileTouch = false;

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

            HandleKeyboardInput();
        }

        private void HandleKeyboardInput()
        {
            foreach (var kvp in keyActions)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    kvp.Value?.Invoke();
                    break; // Выполняем только одно действие за кадр
                }
            }
        }

        public void RegisterKeyAction(KeyCode key, System.Action action)
        {
            if (keyActions.ContainsKey(key))
            {
                keyActions[key] = action;
            }
            else
            {
                keyActions.Add(key, action);
            }
        }

        public void UnregisterKeyAction(KeyCode key)
        {
            if (keyActions.ContainsKey(key))
            {
                keyActions.Remove(key);
            }
        }

        public void RegisterNamedAction(string actionName, System.Action action)
        {
            if (namedActions.ContainsKey(actionName))
            {
                namedActions[actionName] = action;
            }
            else
            {
                namedActions.Add(actionName, action);
            }
        }

        public void ExecuteNamedAction(string actionName)
        {
            if (namedActions.TryGetValue(actionName, out System.Action action))
            {
                action?.Invoke();
            }
        }

        public void SetEnableKeyboardShortcuts(bool enable)
        {
            enableKeyboardShortcuts = enable;
        }

        public void SetEnableMobileTouch(bool enable)
        {
            enableMobileTouch = enable;
        }

        private void OnDestroy()
        {
            keyActions.Clear();
            namedActions.Clear();
        }
    }
}