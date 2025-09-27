using UnityEngine;

namespace UI
{
    public class UIAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float animationSpeed = 2f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector3 _originalScale;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            _originalScale = _rectTransform.localScale;
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

            Vector3 startScale = show ? Vector3.zero : _originalScale;
            Vector3 endScale = show ? _originalScale : Vector3.zero;
            
            float startAlpha = show ? 0f : 1f;
            float endAlpha = show ? 1f : 0f;

            while (Time.time - startTime < duration)
            {
                float progress = (Time.time - startTime) / duration;
                
                float scaleProgress = scaleCurve.Evaluate(progress);
                float fadeProgress = fadeCurve.Evaluate(progress);

                _rectTransform.localScale = Vector3.Lerp(startScale, endScale, scaleProgress);
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, fadeProgress);

                yield return null;
            }

            _rectTransform.localScale = endScale;
            _canvasGroup.alpha = endAlpha;

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
            Vector3 pressedScale = _originalScale * 0.95f;
            float pressTime = 0.1f;

            // Нажатие
            float startTime = Time.time;
            while (Time.time - startTime < pressTime)
            {
                float progress = (Time.time - startTime) / pressTime;
                _rectTransform.localScale = Vector3.Lerp(_originalScale, pressedScale, progress);
                yield return null;
            }

            // Возврат
            startTime = Time.time;
            while (Time.time - startTime < pressTime)
            {
                float progress = (Time.time - startTime) / pressTime;
                _rectTransform.localScale = Vector3.Lerp(pressedScale, _originalScale, progress);
                yield return null;
            }

            _rectTransform.localScale = _originalScale;
        }
    }
}