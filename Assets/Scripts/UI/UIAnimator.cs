using UnityEngine;

namespace UI
{
    public class UIAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float animationSpeed = 2f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Vector3 originalScale;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            originalScale = rectTransform.localScale;
        }

        public void ShowPanel()
        {
            gameObject.SetActive(true);
            StartCoroutine(AnimatePanel(true));
        }

        public void HidePanel()
        {
            StartCoroutine(AnimatePanel(false));
        }

        private System.Collections.IEnumerator AnimatePanel(bool show)
        {
            float startTime = Time.time;
            float duration = 1f / animationSpeed;

            Vector3 startScale = show ? Vector3.zero : originalScale;
            Vector3 endScale = show ? originalScale : Vector3.zero;
            
            float startAlpha = show ? 0f : 1f;
            float endAlpha = show ? 1f : 0f;

            while (Time.time - startTime < duration)
            {
                float progress = (Time.time - startTime) / duration;
                
                float scaleProgress = scaleCurve.Evaluate(progress);
                float fadeProgress = fadeCurve.Evaluate(progress);

                rectTransform.localScale = Vector3.Lerp(startScale, endScale, scaleProgress);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, fadeProgress);

                yield return null;
            }

            rectTransform.localScale = endScale;
            canvasGroup.alpha = endAlpha;

            if (!show)
            {
                gameObject.SetActive(false);
            }
        }

        public void AnimateButton()
        {
            StartCoroutine(ButtonPressAnimation());
        }

        private System.Collections.IEnumerator ButtonPressAnimation()
        {
            Vector3 pressedScale = originalScale * 0.95f;
            float pressTime = 0.1f;

            // Нажатие
            float startTime = Time.time;
            while (Time.time - startTime < pressTime)
            {
                float progress = (Time.time - startTime) / pressTime;
                rectTransform.localScale = Vector3.Lerp(originalScale, pressedScale, progress);
                yield return null;
            }

            // Возврат
            startTime = Time.time;
            while (Time.time - startTime < pressTime)
            {
                float progress = (Time.time - startTime) / pressTime;
                rectTransform.localScale = Vector3.Lerp(pressedScale, originalScale, progress);
                yield return null;
            }

            rectTransform.localScale = originalScale;
        }
    }
}