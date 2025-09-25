using UnityEngine;
using System.Collections;

namespace Core
{
    public class LevelTimer : MonoBehaviour
    {
        public static LevelTimer Instance { get; private set; }
        
        [Header("Timer Settings")]
        [SerializeField] private float timeLimit = 300f; // 5 minutes
        [SerializeField] private bool countUp = true;
        
        private float currentTime = 0f;
        private bool isRunning = false;
        
        public System.Action<float> OnTimeChanged;
        public System.Action OnTimeLimitReached;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            if (isRunning)
            {
                if (countUp)
                {
                    currentTime += Time.deltaTime;
                }
                else
                {
                    currentTime -= Time.deltaTime;
                    if (currentTime <= 0f)
                    {
                        currentTime = 0f;
                        StopTimer();
                        OnTimeLimitReached?.Invoke();
                    }
                }
                
                OnTimeChanged?.Invoke(currentTime);
            }
        }
        
        public void StartTimer()
        {
            isRunning = true;
        }
        
        public void StopTimer()
        {
            isRunning = false;
        }
        
        public void ResetTimer()
        {
            currentTime = countUp ? 0f : timeLimit;
            isRunning = false;
        }
        
        public void SetTimeLimit(float limit)
        {
            timeLimit = limit;
            if (!countUp)
            {
                currentTime = timeLimit;
            }
        }
        
        public float GetCurrentTime()
        {
            return currentTime;
        }
        
        public bool IsRunning()
        {
            return isRunning;
        }
        
        public float GetTimeLimit()
        {
            return timeLimit;
        }
    }
}