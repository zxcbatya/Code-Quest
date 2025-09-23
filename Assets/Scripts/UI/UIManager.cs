using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("UI Panels")]
        [SerializeField] private List<GameObject> uiPanels = new List<GameObject>();
        
        [Header("UI Settings")]
        [SerializeField] private bool autoHidePanels = true;
        [SerializeField] private float panelHideDelay = 5f;
        
        private Dictionary<string, GameObject> panelDict = new Dictionary<string, GameObject>();
        private Dictionary<GameObject, float> panelHideTimers = new Dictionary<GameObject, float>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePanels();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Инициализируем UI при запуске
            InitializeUI();
        }
        
        private void InitializePanels()
        {
            // Инициализируем словарь панелей
            foreach (GameObject panel in uiPanels)
            {
                if (panel != null)
                {
                    string panelName = panel.name.ToLower();
                    if (!panelDict.ContainsKey(panelName))
                    {
                        panelDict.Add(panelName, panel);
                    }
                }
            }
        }
        
        private void InitializeUI()
        {
            // Инициализируем UI элементы
            Debug.Log("UI initialized");
        }
        
        private void Update()
        {
            // Обрабатываем таймеры скрытия панелей
            if (autoHidePanels)
            {
                UpdatePanelHideTimers();
            }
        }
        
        private void UpdatePanelHideTimers()
        {
            List<GameObject> panelsToHide = new List<GameObject>();
            
            foreach (var kvp in panelHideTimers)
            {
                if (Time.time >= kvp.Value)
                {
                    panelsToHide.Add(kvp.Key);
                }
            }
            
            foreach (GameObject panel in panelsToHide)
            {
                HidePanel(panel);
                panelHideTimers.Remove(panel);
            }
        }
        
        public void ShowPanel(string panelName)
        {
            if (panelDict.ContainsKey(panelName.ToLower()))
            {
                GameObject panel = panelDict[panelName.ToLower()];
                ShowPanel(panel);
            }
        }
        
        public void ShowPanel(GameObject panel)
        {
            if (panel != null)
            {
                panel.SetActive(true);
                
                // Отменяем таймер скрытия, если он есть
                if (panelHideTimers.ContainsKey(panel))
                {
                    panelHideTimers.Remove(panel);
                }
            }
        }
        
        public void HidePanel(string panelName)
        {
            if (panelDict.ContainsKey(panelName.ToLower()))
            {
                GameObject panel = panelDict[panelName.ToLower()];
                HidePanel(panel);
            }
        }
        
        public void HidePanel(GameObject panel)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
        
        public void HidePanelAfterDelay(string panelName, float delay)
        {
            if (panelDict.ContainsKey(panelName.ToLower()))
            {
                GameObject panel = panelDict[panelName.ToLower()];
                HidePanelAfterDelay(panel, delay);
            }
        }
        
        public void HidePanelAfterDelay(GameObject panel, float delay)
        {
            if (panel != null)
            {
                if (panelHideTimers.ContainsKey(panel))
                {
                    panelHideTimers[panel] = Time.time + delay;
                }
                else
                {
                    panelHideTimers.Add(panel, Time.time + delay);
                }
            }
        }
        
        public void TogglePanel(string panelName)
        {
            if (panelDict.ContainsKey(panelName.ToLower()))
            {
                GameObject panel = panelDict[panelName.ToLower()];
                TogglePanel(panel);
            }
        }
        
        public void TogglePanel(GameObject panel)
        {
            if (panel != null)
            {
                if (panel.activeSelf)
                {
                    HidePanel(panel);
                }
                else
                {
                    ShowPanel(panel);
                }
            }
        }
        
        public bool IsPanelVisible(string panelName)
        {
            if (panelDict.ContainsKey(panelName.ToLower()))
            {
                GameObject panel = panelDict[panelName.ToLower()];
                return panel != null && panel.activeSelf;
            }
            return false;
        }
        
        public bool IsPanelVisible(GameObject panel)
        {
            return panel != null && panel.activeSelf;
        }
        
        public void AddPanel(GameObject panel)
        {
            if (panel != null && !uiPanels.Contains(panel))
            {
                uiPanels.Add(panel);
                string panelName = panel.name.ToLower();
                if (!panelDict.ContainsKey(panelName))
                {
                    panelDict.Add(panelName, panel);
                }
            }
        }
        
        public void RemovePanel(GameObject panel)
        {
            if (panel != null && uiPanels.Contains(panel))
            {
                uiPanels.Remove(panel);
                string panelName = panel.name.ToLower();
                if (panelDict.ContainsKey(panelName))
                {
                    panelDict.Remove(panelName);
                }
                
                // Удаляем таймер, если он есть
                if (panelHideTimers.ContainsKey(panel))
                {
                    panelHideTimers.Remove(panel);
                }
            }
        }
        
        public void ShowAllPanels()
        {
            foreach (GameObject panel in uiPanels)
            {
                ShowPanel(panel);
            }
        }
        
        public void HideAllPanels()
        {
            foreach (GameObject panel in uiPanels)
            {
                HidePanel(panel);
            }
            
            // Очищаем все таймеры
            panelHideTimers.Clear();
        }
        
        public List<GameObject> GetAllPanels()
        {
            return new List<GameObject>(uiPanels);
        }
        
        public GameObject GetPanel(string panelName)
        {
            if (panelDict.ContainsKey(panelName.ToLower()))
            {
                return panelDict[panelName.ToLower()];
            }
            return null;
        }
    }
}