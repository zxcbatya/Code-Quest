using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private readonly Dictionary<KeyCode, System.Action> _keyActions = new Dictionary<KeyCode, System.Action>();
        private readonly Dictionary<string, System.Action> _namedActions = new Dictionary<string, System.Action>();

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
            foreach (var kvp in _keyActions)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    kvp.Value?.Invoke();
                    break;
                }
            }
        }

        public void RegisterKeyAction(KeyCode key, System.Action action)
        {
            if (_keyActions.ContainsKey(key))
            {
                _keyActions[key] = action;
            }
            else
            {
                _keyActions.Add(key, action);
            }
        }

        public void UnregisterKeyAction(KeyCode key)
        {
            if (_keyActions.ContainsKey(key))
            {
                _keyActions.Remove(key);
            }
        }

        public void RegisterNamedAction(string actionName, System.Action action)
        {
            if (_namedActions.ContainsKey(actionName))
            {
                _namedActions[actionName] = action;
            }
            else
            {
                _namedActions.Add(actionName, action);
            }
        }

        public void ExecuteNamedAction(string actionName)
        {
            if (_namedActions.TryGetValue(actionName, out System.Action action))
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
            _keyActions.Clear();
            _namedActions.Clear();
        }
    }
}