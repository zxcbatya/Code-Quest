using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }
        
        [Header("Effect Prefabs")]
        [SerializeField] private ParticleSystem moveEffect;
        [SerializeField] private ParticleSystem turnEffect;
        [SerializeField] private ParticleSystem jumpEffect;
        [SerializeField] private ParticleSystem interactEffect;
        [SerializeField] private ParticleSystem goalReachedEffect;
        [SerializeField] private ParticleSystem levelCompleteEffect;
        
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
        
        public void PlayMoveEffect(Vector3 position)
        {
            if (moveEffect != null)
            {
                ParticleSystem effect = Instantiate(moveEffect, position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
        
        public void PlayTurnEffect(Vector3 position)
        {
            if (turnEffect != null)
            {
                ParticleSystem effect = Instantiate(turnEffect, position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
        
        public void PlayJumpEffect(Vector3 position)
        {
            if (jumpEffect != null)
            {
                ParticleSystem effect = Instantiate(jumpEffect, position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
        
        public void PlayInteractEffect(Vector3 position)
        {
            if (interactEffect != null)
            {
                ParticleSystem effect = Instantiate(interactEffect, position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
        
        public void PlayGoalReachedEffect(Vector3 position)
        {
            if (goalReachedEffect != null)
            {
                ParticleSystem effect = Instantiate(goalReachedEffect, position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
        
        public void PlayLevelCompleteEffect(Vector3 position)
        {
            if (levelCompleteEffect != null)
            {
                ParticleSystem effect = Instantiate(levelCompleteEffect, position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
    }
}