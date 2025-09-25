using UnityEngine;

namespace Core
{
    public class YandexGameManager : MonoBehaviour
    {
        public static YandexGameManager Instance { get; private set; }
        
        [Header("Yandex Games Settings")]
        [SerializeField] private bool enableYandexFeatures = true;
        [SerializeField] private bool enableLeaderboards = true;
        [SerializeField] private bool enableAdvertisements = true;
        [SerializeField] private bool enableInAppPurchases = true;
        
        private bool isInitialized = false;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeYandexGames();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeYandexGames()
        {
            if (!enableYandexFeatures) return;
            
            // In a real implementation, you would initialize the Yandex Games SDK here
            // For now, we'll just simulate initialization
            isInitialized = true;
            Debug.Log("Yandex Games SDK инициализирован");
        }
        
        public void ShowAdvertisement()
        {
            if (!enableYandexFeatures || !enableAdvertisements) return;
            if (!isInitialized) return;
            
            // In a real implementation, you would show an advertisement here
            Debug.Log("Показ рекламы");
        }
        
        public void SubmitScore(string leaderboardName, int score)
        {
            if (!enableYandexFeatures || !enableLeaderboards) return;
            if (!isInitialized) return;
            
            // In a real implementation, you would submit the score to Yandex leaderboard
            Debug.Log($"Счет {score} отправлен в таблицу лидеров {leaderboardName}");
        }
        
        public void PurchaseItem(string productId)
        {
            if (!enableYandexFeatures || !enableInAppPurchases) return;
            if (!isInitialized) return;
            
            // In a real implementation, you would initiate an in-app purchase
            Debug.Log($"Покупка товара {productId}");
        }
        
        public void ShowLeaderboard(string leaderboardName)
        {
            if (!enableYandexFeatures || !enableLeaderboards) return;
            if (!isInitialized) return;
            
            // In a real implementation, you would show the leaderboard
            Debug.Log($"Показ таблицы лидеров {leaderboardName}");
        }
        
        public bool IsYandexGamesEnabled()
        {
            return enableYandexFeatures && isInitialized;
        }
        
        public bool AreLeaderboardsEnabled()
        {
            return enableYandexFeatures && enableLeaderboards && isInitialized;
        }
        
        public bool AreAdvertisementsEnabled()
        {
            return enableYandexFeatures && enableAdvertisements && isInitialized;
        }
        
        public bool AreInAppPurchasesEnabled()
        {
            return enableYandexFeatures && enableInAppPurchases && isInitialized;
        }
    }
}