using UnityEngine;
using TMPro;
using System.Collections;

namespace UI
{
    public class HintManager : MonoBehaviour
    {
        public static HintManager Instance { get; private set; }
        
        [Header("UI References")]
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private float displayDuration = 3f;
        
        private Coroutine hideCoroutine;
        
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
        
        private void Start()
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
        }
        
        public void ShowHint(string message, float duration = -1f)
        {
            if (hintPanel == null || hintText == null) return;
            
            // Отменяем предыдущий корутин, если он есть
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            
            // Показываем подсказку
            hintText.text = message;
            hintPanel.SetActive(true);
            
            // Запускаем корутин для скрытия подсказки
            float hideDuration = duration > 0 ? duration : displayDuration;
            hideCoroutine = StartCoroutine(HideHintAfterDelay(hideDuration));
        }
        
        private IEnumerator HideHintAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
            
            hideCoroutine = null;
        }
        
        public void HideHint()
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
            
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }
        }
        
        public void ShowLocalizedHint(string key, float duration = -1f)
        {
            string message = LocalizationManager.Instance?.GetText(key) ?? key;
            ShowHint(message, duration);
        }
    }
}