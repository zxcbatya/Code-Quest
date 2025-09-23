using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }
        
        [Header("Screen Effects")]
        [SerializeField] private Animator screenAnimator;
        
        [Header("UI Effects")]
        [SerializeField] private Animator uiAnimator;
        
        [Header("Particle Effects (merged from ParticleManager)")]
        [SerializeField] private ParticleSystem successParticles;
        [SerializeField] private ParticleSystem failParticles;
        [SerializeField] private ParticleSystem collectParticles;
        [SerializeField] private ParticleSystem explosionParticles;
        
        private Dictionary<string, ParticleSystem> particleSystems = new Dictionary<string, ParticleSystem>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeParticleSystems();
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
        
        // --- Particle API (from ParticleManager) ---
        private void InitializeParticleSystems()
        {
            if (successParticles != null)
                particleSystems["success"] = successParticles;
            if (failParticles != null)
                particleSystems["fail"] = failParticles;
            if (collectParticles != null)
                particleSystems["collect"] = collectParticles;
            if (explosionParticles != null)
                particleSystems["explosion"] = explosionParticles;
        }
        
        public void PlayParticleEffect(string effectName, Vector3 position)
        {
            if (particleSystems.ContainsKey(effectName))
            {
                ParticleSystem particleSystem = particleSystems[effectName];
                if (particleSystem != null)
                {
                    ParticleSystem instance = Instantiate(particleSystem, position, Quaternion.identity);
                    instance.Play();
                    Destroy(instance.gameObject, instance.main.duration);
                }
            }
        }
        
        public void PlaySuccessEffect(Vector3 position)
        {
            PlayParticleEffect("success", position);
        }
        
        public void PlayFailEffect(Vector3 position)
        {
            PlayParticleEffect("fail", position);
        }
        
        public void PlayCollectEffect(Vector3 position)
        {
            PlayParticleEffect("collect", position);
        }
        
        public void PlayExplosionEffect(Vector3 position)
        {
            PlayParticleEffect("explosion", position);
        }
        
        public void StopAllEffects()
        {
            foreach (var kvp in particleSystems)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Stop();
                }
            }
        }
    }
}