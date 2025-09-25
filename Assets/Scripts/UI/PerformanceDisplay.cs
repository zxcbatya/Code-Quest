using UnityEngine;
using TMPro;
using Core;

namespace RobotCoder.UI
{
    public class PerformanceDisplay : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI avgFPSText;
        [SerializeField] private TextMeshProUGUI minFPSText;
        [SerializeField] private TextMeshProUGUI maxFPSText;
        
        [Header("Display Settings")]
        [SerializeField] private string fpsPrefix = "FPS: ";
        [SerializeField] private string avgFPSPrefix = "Средний FPS: ";
        [SerializeField] private string minFPSPrefix = "Мин. FPS: ";
        [SerializeField] private string maxFPSPrefix = "Макс. FPS: ";
        [SerializeField] private bool showDetailedMetrics = false;
        
        private void Start()
        {
            InitializePerformanceDisplay();
        }
        
        private void InitializePerformanceDisplay()
        {
            if (PerformanceManager.Instance != null)
            {
                PerformanceManager.Instance.OnFPSUpdated += OnFPSUpdated;
                
                // Hide detailed metrics if not enabled
                if (avgFPSText != null)
                    avgFPSText.gameObject.SetActive(showDetailedMetrics);
                    
                if (minFPSText != null)
                    minFPSText.gameObject.SetActive(showDetailedMetrics);
                    
                if (maxFPSText != null)
                    maxFPSText.gameObject.SetActive(showDetailedMetrics);
            }
        }
        
        private void OnFPSUpdated(float fps)
        {
            UpdatePerformanceDisplay();
        }
        
        private void UpdatePerformanceDisplay()
        {
            if (PerformanceManager.Instance == null) return;
            
            if (fpsText != null)
                fpsText.text = fpsPrefix + Mathf.RoundToInt(PerformanceManager.Instance.GetCurrentFPS()).ToString();
                
            if (showDetailedMetrics)
            {
                if (avgFPSText != null)
                    avgFPSText.text = avgFPSPrefix + Mathf.RoundToInt(PerformanceManager.Instance.GetAverageFPS()).ToString();
                    
                if (minFPSText != null)
                    minFPSText.text = minFPSPrefix + Mathf.RoundToInt(PerformanceManager.Instance.GetMinFPS()).ToString();
                    
                if (maxFPSText != null)
                    maxFPSText.text = maxFPSPrefix + Mathf.RoundToInt(PerformanceManager.Instance.GetMaxFPS()).ToString();
            }
        }
        
        private void OnDestroy()
        {
            if (PerformanceManager.Instance != null)
            {
                PerformanceManager.Instance.OnFPSUpdated -= OnFPSUpdated;
            }
        }
    }
}