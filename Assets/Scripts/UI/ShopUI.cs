using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class ShopUI : MonoBehaviour
    {
        [System.Serializable]
        public class ShopItemUI
        {
            public string itemId;
            public Button purchaseButton;
            public TextMeshProUGUI itemNameText;
            public TextMeshProUGUI itemDescriptionText;
            public TextMeshProUGUI coinCostText;
            public TextMeshProUGUI starCostText;
            public GameObject purchasedIndicator;
        }
        
        [Header("UI Components")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private ShopItemUI[] shopItemUIs;
        [SerializeField] private Button closeButton;
        
        [Header("References")]
        [SerializeField] private CurrencyDisplay currencyDisplay;
        
        private void Start()
        {
            InitializeShopUI();
        }
        
        private void InitializeShopUI()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(HideShop);
                
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnShopUpdated += UpdateShopUI;
                UpdateShopUI();
            }
            
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCoinsChanged += OnCurrencyChanged;
                CurrencyManager.Instance.OnStarsChanged += OnCurrencyChanged;
            }
            
            SetupItemButtons();
        }
        
        private void SetupItemButtons()
        {
            foreach (var itemUI in shopItemUIs)
            {
                if (itemUI.purchaseButton != null)
                {
                    string itemId = itemUI.itemId;
                    itemUI.purchaseButton.onClick.AddListener(() => OnPurchaseButtonClicked(itemId));
                }
            }
        }
        
        private void OnPurchaseButtonClicked(string itemId)
        {
            if (ShopManager.Instance != null)
            {
                bool purchased = ShopManager.Instance.PurchaseItem(itemId);
                if (purchased)
                {
                    UpdateShopUI();
                }
            }
        }
        
        private void OnCurrencyChanged(int newAmount)
        {
            UpdateShopUI();
        }
        
        private void UpdateShopUI()
        {
            if (ShopManager.Instance == null) return;
            
            foreach (var itemUI in shopItemUIs)
            {
                ShopItem item = ShopManager.Instance.GetItem(itemUI.itemId);
                if (item == null) continue;
                
                // Update item name and description
                if (itemUI.itemNameText != null)
                    itemUI.itemNameText.text = item.name;
                    
                if (itemUI.itemDescriptionText != null)
                    itemUI.itemDescriptionText.text = item.description;
                
                // Update cost display
                if (itemUI.coinCostText != null)
                    itemUI.coinCostText.text = item.coinCost > 0 ? item.coinCost.ToString() : "";
                    
                if (itemUI.starCostText != null)
                    itemUI.starCostText.text = item.starCost > 0 ? item.starCost.ToString() : "";
                
                // Update purchase button state
                if (itemUI.purchaseButton != null)
                {
                    bool canAfford = CanAffordItem(item);
                    bool isPurchased = item.isPurchased;
                    
                    itemUI.purchaseButton.interactable = !isPurchased && canAfford;
                    itemUI.purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                        isPurchased ? "Куплено" : "Купить";
                }
                
                // Update purchased indicator
                if (itemUI.purchasedIndicator != null)
                {
                    itemUI.purchasedIndicator.SetActive(item.isPurchased);
                }
            }
        }
        
        private bool CanAffordItem(ShopItem item)
        {
            if (CurrencyManager.Instance == null) return false;
            
            bool canAffordCoins = item.coinCost <= 0 || 
                                CurrencyManager.Instance.GetCoins() >= item.coinCost;
                                
            bool canAffordStars = item.starCost <= 0 || 
                                CurrencyManager.Instance.GetStars() >= item.starCost;
                                
            return canAffordCoins && canAffordStars;
        }
        
        public void ShowShop()
        {
            if (shopPanel != null)
                shopPanel.SetActive(true);
                
            UpdateShopUI();
        }
        
        public void HideShop()
        {
            if (shopPanel != null)
                shopPanel.SetActive(false);
        }
        
        private void OnDestroy()
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnShopUpdated -= UpdateShopUI;
            }
            
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCoinsChanged -= OnCurrencyChanged;
                CurrencyManager.Instance.OnStarsChanged -= OnCurrencyChanged;
            }
        }
    }
}