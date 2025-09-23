using UnityEngine;
using System.Collections;

namespace Core
{
    public class YandexGameManager : MonoBehaviour
    {
        public static YandexGameManager Instance { get; private set; }
        
        [Header("Yandex Settings")]
        [SerializeField] private bool enableYandexIntegration = true;
        [SerializeField] private string leaderboardName = "level_progress";
        
        private bool isYandexAvailable = false;
        private bool isPlayerAuthorized = false;
        
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
        
        private void Start()
        {
            if (enableYandexIntegration)
            {
                InitializeYandexIntegration();
            }
        }
        
        private void InitializeYandexIntegration()
        {
            // Проверяем доступность Yandex Games API
            #if UNITY_WEBGL
            isYandexAvailable = CheckYandexAvailability();
            #endif
            
            if (isYandexAvailable)
            {
                // Авторизуем игрока
                AuthorizePlayer();
            }
        }
        
        private bool CheckYandexAvailability()
        {
            // В WebGL проверяем наличие Yandex API
            #if UNITY_WEBGL
            try
            {
                // Проверяем, доступен ли объект ysdk
                // Это псевдокод, реальная реализация зависит от Yandex Games SDK
                return Application.platform == RuntimePlatform.WebGLPlayer;
            }
            catch
            {
                return false;
            }
            #else
            return false;
            #endif
        }
        
        private void AuthorizePlayer()
        {
            if (!isYandexAvailable) return;
            
            // В реальной реализации здесь будет код авторизации через Yandex Games SDK
            // ysdk.auth.openAuthDialog().then(...)
            
            isPlayerAuthorized = true;
        }
        
        public void SaveToLeaderboard(int score)
        {
            if (!isYandexAvailable || !isPlayerAuthorized) return;
            
            // В реальной реализации здесь будет код сохранения результата
            // ysdk.leaderboards.setLeaderboardScore(leaderboardName, score);
            
            Debug.Log($"Saving score {score} to leaderboard {leaderboardName}");
        }
        
        public void ShowLeaderboard()
        {
            if (!isYandexAvailable || !isPlayerAuthorized) return;
            
            // В реальной реализации здесь будет код отображения таблицы лидеров
            // ysdk.leaderboards.getLeaderboardEntries(leaderboardName);
            
            Debug.Log($"Showing leaderboard {leaderboardName}");
        }
        
        public void ShowFullscreenAd()
        {
            if (!isYandexAvailable || !isPlayerAuthorized) return;
            
            // В реальной реализации здесь будет код показа рекламы
            // ysdk.adv.showFullscreenAdv();
            
            Debug.Log("Showing fullscreen ad");
        }
        
        public void ShowRewardedAd(System.Action<bool> onComplete)
        {
            if (!isYandexAvailable || !isPlayerAuthorized) 
            {
                onComplete?.Invoke(false);
                return;
            }
            
            // В реальной реализации здесь будет код показа вознаграждаемой рекламы
            // ysdk.adv.showRewardedVideo().then(() => onComplete?.Invoke(true));
            
            Debug.Log("Showing rewarded ad");
            onComplete?.Invoke(true); // Для тестирования
        }
        
        public bool IsYandexAvailable()
        {
            return isYandexAvailable;
        }
        
        public bool IsPlayerAuthorized()
        {
            return isPlayerAuthorized;
        }
    }
}