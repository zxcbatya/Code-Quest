using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CommandCountDisplay : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private TextMeshProUGUI commandCountText;

        [Header("Display Settings")] [SerializeField]
        private string commandCountPrefix = "Команды: ";

        private void Start()
        {
            InitializeCommandCountDisplay();
        }

        private void InitializeCommandCountDisplay()
        {
            CommandCounter.Instance.OnCommandCountChanged += OnCommandCountChanged;
            UpdateCommandCountDisplay(CommandCounter.Instance.GetCurrentProgramCommandCount());
        }

        private void OnCommandCountChanged(int newCount)
        {
            UpdateCommandCountDisplay(newCount);
        }

        private void UpdateCommandCountDisplay(int count)
        {
            commandCountText.text = commandCountPrefix + count.ToString();
        }

        private void OnDestroy()
        {
            CommandCounter.Instance.OnCommandCountChanged -= OnCommandCountChanged;
        }
    }
}