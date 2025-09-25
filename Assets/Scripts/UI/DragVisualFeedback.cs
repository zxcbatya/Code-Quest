using UnityEngine;
using UnityEngine.UI;

namespace RobotCoder.UI
{
    public class DragVisualFeedback : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color validDropColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        [SerializeField] private Color invalidDropColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);
        
        private Color originalColor;
        
        private void Awake()
        {
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();
            
            if (backgroundImage != null)
                originalColor = backgroundImage.color;
        }
        
        public void ShowDragStart()
        {
            if (backgroundImage != null)
                backgroundImage.color = originalColor * 1.2f;
        }
        
        public void ShowValidDropZone()
        {
            if (backgroundImage != null)
                backgroundImage.color = validDropColor;
        }
        
        public void ShowInvalidDropZone()
        {
            if (backgroundImage != null)
                backgroundImage.color = invalidDropColor;
        }
        
        public void HideFeedback()
        {
            if (backgroundImage != null)
                backgroundImage.color = originalColor;
        }
        
        public void ShowSuccessFeedback()
        {
            if (backgroundImage != null)
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