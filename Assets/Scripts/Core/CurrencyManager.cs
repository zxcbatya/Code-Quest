using UnityEngine;

namespace Core
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }
        
        [Header("Currencies")]
        [SerializeField] private int coins = 0;
        [SerializeField] private int stars = 0;
        
        public System.Action<int> OnCoinsChanged;
        public System.Action<int> OnStarsChanged;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadCurrencies();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void LoadCurrencies()
        {
            coins = PlayerPrefs.GetInt("PlayerCoins", 0);
            stars = PlayerPrefs.GetInt("PlayerStars", 0);
        }
        
        public void AddCoins(int amount)
        {
            coins += amount;
            SaveCurrencies();
            OnCoinsChanged?.Invoke(coins);
        }
        
        public void AddStars(int amount)
        {
            stars += amount;
            SaveCurrencies();
            OnStarsChanged?.Invoke(stars);
        }
        
        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                SaveCurrencies();
                OnCoinsChanged?.Invoke(coins);
                return true;
            }
            return false;
        }
        
        public bool SpendStars(int amount)
        {
            if (stars >= amount)
            {
                stars -= amount;
                SaveCurrencies();
                OnStarsChanged?.Invoke(stars);
                return true;
            }
            return false;
        }
        
        public int GetCoins()
        {
            return coins;
        }
        
        public int GetStars()
        {
            return stars;
        }
        
        private void SaveCurrencies()
        {
            PlayerPrefs.SetInt("PlayerCoins", coins);
            PlayerPrefs.SetInt("PlayerStars", stars);
            PlayerPrefs.Save();
        }
        
        public void ResetCurrencies()
        {
            coins = 0;
            stars = 0;
            SaveCurrencies();
            OnCoinsChanged?.Invoke(coins);
            OnStarsChanged?.Invoke(stars);
        }
    }
}