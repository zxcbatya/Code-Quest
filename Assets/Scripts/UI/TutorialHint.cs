using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class TutorialHint : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private Button closeButton;
        
        [Header("Hint Settings")]
        [SerializeField] private string[] hints;
        [SerializeField] private bool showHints = true;
        [SerializeField] private float displayTime = 5f;
        
        private int currentHintIndex = 0;
        private bool isShowingHint = false;
        
        private void Start()
        {
            SetupEventListeners();
            LoadHintSettings();
            
            if (showHints && hints.Length > 0)
            {
                ShowHint(0);
            }
        }
        
        private void SetupEventListeners()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(HideHint);
        }
        
        private void LoadHintSettings()
        {
            showHints = PlayerPrefs.GetInt("ShowHints", 1) == 1;
        }
        
        public void ShowHint(int index)
        {
            if (!showHints || index < 0 || index >= hints.Length || isShowingHint) return;
            
            currentHintIndex = index;
            isShowingHint = true;
            
            if (hintText != null)
                hintText.text = hints[index];
                
            if (hintPanel != null)
                hintPanel.SetActive(true);
                
            // Auto-hide after display time
            Invoke(nameof(HideHint), displayTime);
        }
        
        public void ShowNextHint()
        {
            int nextIndex = (currentHintIndex + 1) % hints.Length;
            ShowHint(nextIndex);
        }
        
        public void HideHint()
        {
            if (hintPanel != null)
                hintPanel.SetActive(false);
                
            isShowingHint = false;
            CancelInvoke(nameof(HideHint));
        }
        
        public void ToggleHints(bool show)
        {
            showHints = show;
            PlayerPrefs.SetInt("ShowHints", show ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        public bool AreHintsEnabled()
        {
            return showHints;
        }
    }
}