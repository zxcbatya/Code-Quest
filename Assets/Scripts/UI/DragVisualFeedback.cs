using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DragVisualFeedback : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color validDropColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        [SerializeField] private Color invalidDropColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);
        
        private Color _originalColor;
        
        private void Awake()
        {
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();
            
            if (backgroundImage != null)
                _originalColor = backgroundImage.color;
        }
        
        public void ShowDragStart()
        {
                backgroundImage.color = _originalColor * 1.2f;
        }
        
        public void ShowValidDropZone()
        {
                backgroundImage.color = validDropColor;
        }
        
        public void ShowInvalidDropZone()
        {
                backgroundImage.color = invalidDropColor;
        }
        
        public void HideFeedback()
        {
                backgroundImage.color = _originalColor;
        }
        
        public void ShowSuccessFeedback()
        {
                StartCoroutine(QuickSuccessAnimation());
        }
        
        private System.Collections.IEnumerator QuickSuccessAnimation()
        {
            if (backgroundImage == null) yield break;
            
            Color originalColor = backgroundImage.color;
            backgroundImage.color = validDropColor;
            yield return new WaitForSeconds(0.1f);
            backgroundImage.color = originalColor;
        }
    }
}