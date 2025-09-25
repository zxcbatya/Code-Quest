using Core;
using RobotCoder.Core;
using UnityEngine;
using RobotCoder.UI;
using UI;

namespace RobotCoder.Core
{
    public class TestGameSetup : MonoBehaviour
    {
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
            }
            
            if (LocalizationManager.Instance == null)
            {
                GameObject localizationObj = new GameObject("LocalizationManager");
                localizationObj.AddComponent<LocalizationManager>();
            }
            
            if (InputManager.Instance == null)
            {
                GameObject inputObj = new GameObject("InputManager");
                inputObj.AddComponent<InputManager>();
            }
            
            if (RobotController.Instance == null)
            {
                GameObject robotObj = new GameObject("Robot");
                robotObj.AddComponent<RobotController>();
            }
            
            if (ProgramInterpreter.Instance == null)
            {
                GameObject interpreterObj = new GameObject("ProgramInterpreter");
                interpreterObj.AddComponent<ProgramInterpreter>();
            }
        }
    }
}