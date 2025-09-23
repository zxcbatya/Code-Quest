using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RobotCoder.UI
{
    /// <summary>
    /// Компонент для визуальной обратной связи при перетаскивании блоков
    /// Обеспечивает плавные анимации и четкие индикаторы состояния
    /// </summary>
    public class DragVisualFeedback : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private float highlightIntensity = 1.5f;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private Color validDropColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        [SerializeField] private Color invalidDropColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);
        [SerializeField] private Color neutralColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        
        [Header("Animation Settings")]
        [SerializeField] private float scaleAnimationDuration = 0.2f;
        [SerializeField] private float colorAnimationDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve colorCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private Image backgroundImage;
        private Image borderImage;
        private TextMeshProUGUI feedbackText;
        private RectTransform rectTransform;
        private Vector3 originalScale;
        private Color originalColor;
        private bool isAnimating = false;
        
        private void Awake()
        {
            InitializeComponents();
            SetupVisualElements();
        }
        
        private void InitializeComponents()
        {
            backgroundImage = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            
            // Ищем существующие компоненты
            borderImage = GetComponentInChildren<Image>();
            feedbackText = GetComponentInChildren<TextMeshProUGUI>();
            
            originalScale = rectTransform.localScale;
            originalColor = backgroundImage != null ? backgroundImage.color : Color.white;
        }
        
        
        private void SetupVisualElements()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = neutralColor;
            }
            
            if (borderImage != null)
            {
                borderImage.color = new Color(1f, 1f, 1f, 0f); // Прозрачный по умолчанию
            }
        }
        
        /// <summary>
        /// Показывает визуальную обратную связь при начале перетаскивания
        /// </summary>
        public void ShowDragStart()
        {
            if (isAnimating) return;
            
            StartCoroutine(AnimateScale(originalScale * 1.1f));
            StartCoroutine(AnimateColor(originalColor * highlightIntensity));
        }
        
        /// <summary>
        /// Показывает визуальную обратную связь при наведении на валидную зону сброса
        /// </summary>
        public void ShowValidDropZone()
        {
            if (isAnimating) return;
            
            StartCoroutine(AnimateColor(validDropColor));
        }
        
        /// <summary>
        /// Показывает визуальную обратную связь при наведении на невалидную зону
        /// </summary>
        public void ShowInvalidDropZone()
        {
            if (isAnimating) return;
            
            StartCoroutine(AnimateColor(invalidDropColor));
        }
        
        /// <summary>
        /// Скрывает визуальную обратную связь
        /// </summary>
        public void HideFeedback()
        {
            if (isAnimating) return;
            
            StartCoroutine(AnimateScale(originalScale));
            StartCoroutine(AnimateColor(originalColor));
        }
        
        /// <summary>
        /// Показывает успешное размещение блока
        /// </summary>
        public void ShowSuccessFeedback()
        {
            StartCoroutine(SuccessAnimation());
        }
        
        private System.Collections.IEnumerator AnimateScale(Vector3 targetScale)
        {
            isAnimating = true;
            Vector3 startScale = rectTransform.localScale;
            float elapsed = 0f;
            
            while (elapsed < scaleAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / scaleAnimationDuration;
                float curveValue = scaleCurve.Evaluate(progress);
                
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                yield return null;
            }
            
            rectTransform.localScale = targetScale;
            isAnimating = false;
        }
        
        private System.Collections.IEnumerator AnimateColor(Color targetColor)
        {
            if (backgroundImage == null) yield break;
            
            Color startColor = backgroundImage.color;
            float elapsed = 0f;
            
            while (elapsed < colorAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / colorAnimationDuration;
                float curveValue = colorCurve.Evaluate(progress);
                
                backgroundImage.color = Color.Lerp(startColor, targetColor, curveValue);
                yield return null;
            }
            
            backgroundImage.color = targetColor;
        }
        
        
        private System.Collections.IEnumerator SuccessAnimation()
        {
            // Быстрая анимация успеха
            Vector3 originalScale = rectTransform.localScale;
            Color originalColor = backgroundImage != null ? backgroundImage.color : Color.white;
            
            // Увеличиваем и подсвечиваем зеленым
            rectTransform.localScale = originalScale * 1.2f;
            if (backgroundImage != null)
            {
                backgroundImage.color = validDropColor;
            }
            
            yield return new WaitForSeconds(0.2f);
            
            // Возвращаем к нормальному состоянию
            rectTransform.localScale = originalScale;
            if (backgroundImage != null)
            {
                backgroundImage.color = originalColor;
            }
            
            HideFeedback();
        }
    }
}

