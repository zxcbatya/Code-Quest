using Core;
using RobotCoder.Core;
using UnityEngine;
using RobotCoder.UI;
using RobotCoder.Core;
using UI;

namespace RobotCoder.Core
{
    /// <summary>
    /// Простой скрипт для тестирования системы без полной настройки UI
    /// </summary>
    public class TestGameSetup : MonoBehaviour
    {
        [Header("Тест компонентов")]
        [SerializeField] private bool initializeOnStart = true;
        
        private void Start()
        {
            if (initializeOnStart)
            {
                SetupBasicComponents();
            }
        }
        
        private void SetupBasicComponents()
        {
            if (AudioManager.Instance == null)
            {
                GameObject audioManagerObj = new GameObject("AudioManager");
                audioManagerObj.AddComponent<AudioSource>();
                audioManagerObj.AddComponent<AudioManager>();
                Debug.Log("AudioManager создан");
            }
            
            if (LocalizationManager.Instance == null)
            {
                GameObject localizationObj = new GameObject("LocalizationManager");
                localizationObj.AddComponent<LocalizationManager>();
                Debug.Log("LocalizationManager создан");
            }
            
            if (InputManager.Instance == null)
            {
                GameObject inputObj = new GameObject("InputManager");
                inputObj.AddComponent<InputManager>();
                Debug.Log("InputManager создан");
            }
            
            if (RobotController.Instance == null)
            {
                GameObject robotObj = new GameObject("Robot");
                robotObj.AddComponent<RobotController>();
                Debug.Log("RobotController создан");
            }
            
            if (ProgramInterpreter.Instance == null)
            {
                GameObject interpreterObj = new GameObject("ProgramInterpreter");
                interpreterObj.AddComponent<ProgramInterpreter>();
                Debug.Log("ProgramInterpreter создан");
            }
            
            Debug.Log("✅ Базовые компоненты системы инициализированы!");
        }
        
        [ContextMenu("Test System")]
        public void TestSystem()
        {
            SetupBasicComponents();
            
            // Простой тест - создаем блок команды
            GameObject testBlockObj = new GameObject("TestBlock");
            var moveCommand = testBlockObj.AddComponent<MoveForwardCommand>();
            
            if (moveCommand != null)
            {
                Debug.Log("✅ Блок команды создан успешно!");
                
                // Тестируем выполнение
                if (RobotController.Instance != null)
                {
                    bool result = moveCommand.Execute(RobotController.Instance);
                    Debug.Log($"Результат выполнения команды: {result}");
                }
            }
            
            // Очищаем тестовый объект
            if (Application.isPlaying)
                Destroy(testBlockObj);
            else
                DestroyImmediate(testBlockObj);
        }
    }
}