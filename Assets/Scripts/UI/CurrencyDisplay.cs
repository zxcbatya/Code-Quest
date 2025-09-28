using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CurrencyDisplay : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private TextMeshProUGUI coinsText;

        [SerializeField] private TextMeshProUGUI starsText;

        [Header("Display Settings")] [SerializeField]
        private string coinsPrefix = "Монеты: ";

        [SerializeField] private string starsPrefix = "Звезды: ";

        private void Start()
        {
            InitializeCurrencyDisplay();
        }

        private void InitializeCurrencyDisplay()
        {
            CurrencyManager.Instance.OnCoinsChanged += OnCoinsChanged;
            CurrencyManager.Instance.OnStarsChanged += OnStarsChanged;

            UpdateCoinsDisplay(CurrencyManager.Instance.GetCoins());
            UpdateStarsDisplay(CurrencyManager.Instance.GetStars());
        }

        private void OnCoinsChanged(int newAmount)
        {
            UpdateCoinsDisplay(newAmount);
        }

        private void OnStarsChanged(int newAmount)
        {
            UpdateStarsDisplay(newAmount);
        }

        private void UpdateCoinsDisplay(int amount)
        {
            coinsText.text = coinsPrefix + amount.ToString();
        }

        private void UpdateStarsDisplay(int amount)
        {
            starsText.text = starsPrefix + amount.ToString();
        }

        private void OnDestroy()
        {
            CurrencyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
            CurrencyManager.Instance.OnStarsChanged -= OnStarsChanged;
        }
    }
}