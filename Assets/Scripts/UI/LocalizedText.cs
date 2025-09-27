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
        
        private TextMeshProUGUI _textComponent;

        private void Start()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
            UpdateText();
        }

        public void SetKey(string key)
        {
            localizationKey = key;
            UpdateText();
        }

        public void UpdateText()
        {
            if (_textComponent != null && !string.IsNullOrEmpty(localizationKey))
            {
                if (LocalizationManager.Instance != null)
                {
                    _textComponent.text = LocalizationManager.Instance.GetText(localizationKey);
                }
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying && _textComponent != null)
            {
                UpdateText();
            }
        }
    }
}