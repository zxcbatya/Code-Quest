using UnityEngine;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class CommandCountDisplay : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI commandCountText;
        
        [Header("Display Settings")]
        [SerializeField] private string commandCountPrefix = "Команды: ";
        
        private void Start()
        {
            InitializeCommandCountDisplay();
        }
        
        private void InitializeCommandCountDisplay()
        {
            if (CommandCounter.Instance != null)
            {
                CommandCounter.Instance.OnCommandCountChanged += OnCommandCountChanged;
                UpdateCommandCountDisplay(CommandCounter.Instance.GetCurrentProgramCommandCount());
            }
        }
        
        private void OnCommandCountChanged(int newCount)
        {
            UpdateCommandCountDisplay(newCount);
        }
        
        private void UpdateCommandCountDisplay(int count)
        {
            if (commandCountText != null)
            {
                commandCountText.text = commandCountPrefix + count.ToString();
            }
        }
        
        private void OnDestroy()
        {
            if (CommandCounter.Instance != null)
            {
                CommandCounter.Instance.OnCommandCountChanged -= OnCommandCountChanged;
            }
        }
    }
}