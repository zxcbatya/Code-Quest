using UnityEngine;
using System.Collections;

namespace Core
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }
        
        [Header("Screen Effects")]
        [SerializeField] private Animator screenAnimator;
        
        [Header("UI Effects")]
        [SerializeField] private Animator uiAnimator;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void PlayScreenShake(float intensity = 1f, float duration = 0.5f)
        {
            if (screenAnimator != null)
            {
                screenAnimator.SetFloat("ShakeIntensity", intensity);
                screenAnimator.SetTrigger("Shake");
                
                // Сбрасываем интенсивность через некоторое время
                StartCoroutine(ResetShakeIntensity(duration));
            }
        }
        
        private IEnumerator ResetShakeIntensity(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (screenAnimator != null)
            {
                screenAnimator.SetFloat("ShakeIntensity", 0f);
            }
        }
        
        public void PlayScreenFlash(Color color, float duration = 0.2f)
        {
            if (screenAnimator != null)
            {
                screenAnimator.SetTrigger("Flash");
            }
        }
        
        public void PlayUIPulse(string triggerName = "Pulse")
        {
            if (uiAnimator != null)
            {
                uiAnimator.SetTrigger(triggerName);
            }
        }
        
        public void PlayTransitionEffect(string transitionName)
        {
            if (screenAnimator != null)
            {
                screenAnimator.SetTrigger(transitionName);
            }
        }
    }
}