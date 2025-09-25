using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    [System.Serializable]
    public class ShopItem
    {
        public string id;
        public string name;
        public string description;
        public int coinCost;
        public int starCost;
        public bool isPurchased;
        public bool isAvailable;
    }
    
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }
        
        [Header("Shop Items")]
        [SerializeField] private ShopItem[] shopItems;
        
        private Dictionary<string, ShopItem> itemDictionary;
        
        public System.Action<string> OnItemPurchased;
        public System.Action OnShopUpdated;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeShop();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeShop()
        {
            itemDictionary = new Dictionary<string, ShopItem>();
            
            foreach (var item in shopItems)
            {
                itemDictionary[item.id] = item;
                LoadItemPurchaseStatus(item);
            }
        }
        
        private void LoadItemPurchaseStatus(ShopItem item)
        {
            item.isPurchased = PlayerPrefs.GetInt($"ShopItem_{item.id}_Purchased", 0) == 1;
        }
        
        public bool PurchaseItem(string itemId)
        {
            if (!itemDictionary.TryGetValue(itemId, out ShopItem item))
                return false;
                
            if (item.isPurchased)
                return false;
                
            if (!item.isAvailable)
                return false;
                
            bool canAfford = true;
            
            // Check if player can afford with coins
            if (item.coinCost > 0)
            {
                if (CurrencyManager.Instance.GetCoins() < item.coinCost)
                {
                    canAfford = false;
                }
            }
            
            // Check if player can afford with stars
            if (item.starCost > 0)
            {
                if (CurrencyManager.Instance.GetStars() < item.starCost)
                {
                    canAfford = false;
                }
            }
            
            if (!canAfford)
                return false;
                
            // Spend currencies
            if (item.coinCost > 0)
            {
                CurrencyManager.Instance.SpendCoins(item.coinCost);
            }
            
            if (item.starCost > 0)
            {
                CurrencyManager.Instance.SpendStars(item.starCost);
            }
            
            // Mark as purchased
            item.isPurchased = true;
            PlayerPrefs.SetInt($"ShopItem_{itemId}_Purchased", 1);
            PlayerPrefs.Save();
            
            OnItemPurchased?.Invoke(itemId);
            OnShopUpdated?.Invoke();
            
            Debug.Log($"Предмет {item.name} куплен!");
            return true;
        }
        
        public bool IsItemPurchased(string itemId)
        {
            if (itemDictionary.TryGetValue(itemId, out ShopItem item))
            {
                return item.isPurchased;
            }
            return false;
        }
        
        public ShopItem[] GetAllItems()
        {
            return shopItems;
        }
        
        public ShopItem GetItem(string itemId)
        {
            if (itemDictionary.TryGetValue(itemId, out ShopItem item))
            {
                return item;
            }
            return null;
        }
    }
}