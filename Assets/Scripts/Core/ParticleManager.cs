using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }
        
        [Header("Particle Systems")]
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
                DontDestroyOnLoad(gameObject);
                InitializeParticleSystems();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
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
                    // Создаем копию системы частиц
                    ParticleSystem instance = Instantiate(particleSystem, position, Quaternion.identity);
                    instance.Play();
                    
                    // Удаляем объект после завершения воспроизведения
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