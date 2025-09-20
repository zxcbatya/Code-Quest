using RobotCoder.UI;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [Header("Localization")]
        [SerializeField] private string localizationKey;
        
        private TextMeshProUGUI textComponent;

        private void Start()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            UpdateText();
        }

        public void SetKey(string key)
        {
            localizationKey = key;
            UpdateText();
        }

        public void UpdateText()
        {
            if (textComponent != null && !string.IsNullOrEmpty(localizationKey))
            {
                if (LocalizationManager.Instance != null)
                {
                    textComponent.text = LocalizationManager.Instance.GetText(localizationKey);
                }
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying && textComponent != null)
            {
                UpdateText();
            }
        }
    }
}