using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Core
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }
        
        [System.Serializable]
        public class TutorialStep
        {
            public string title;
            public string description;
            public bool waitForInteraction = true;
            public float autoAdvanceTime = 0f;
        }
        
        [Header("Tutorial Settings")]
        [SerializeField] private TutorialStep[] tutorialSteps;
        [SerializeField] private int currentStep = 0;
        
        [Header("UI Components")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private TextMeshProUGUI stepText;
        
        private bool tutorialActive = false;
        private bool waitingForInteraction = false;
        
        public System.Action OnTutorialStarted;
        public System.Action OnTutorialCompleted;
        public System.Action OnTutorialSkipped;
        
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
            SetupEventListeners();
            
            // Start tutorial on first play
            if (!PlayerPrefs.HasKey("TutorialCompleted"))
            {
                StartTutorial();
            }
        }
        
        private void SetupEventListeners()
        {
            if (nextButton != null)
                nextButton.onClick.AddListener(OnNextButtonClicked);
                
            if (skipButton != null)
                skipButton.onClick.AddListener(OnSkipButtonClicked);
        }
        
        public void StartTutorial()
        {
            if (tutorialSteps.Length == 0) return;
            
            tutorialActive = true;
            currentStep = 0;
            OnTutorialStarted?.Invoke();
            ShowTutorialStep(currentStep);
        }
        
        private void ShowTutorialStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= tutorialSteps.Length) return;
            
            TutorialStep step = tutorialSteps[stepIndex];
            
            if (titleText != null)
                titleText.text = step.title;
                
            if (descriptionText != null)
                descriptionText.text = step.description;
                
            if (stepText != null)
                stepText.text = $"Шаг {stepIndex + 1} из {tutorialSteps.Length}";
                
            if (tutorialPanel != null)
                tutorialPanel.SetActive(true);
                
            waitingForInteraction = step.waitForInteraction;
            
            if (!waitingForInteraction && step.autoAdvanceTime > 0)
            {
                StartCoroutine(AutoAdvance(step.autoAdvanceTime));
            }
            
            // Update button visibility
            if (nextButton != null)
                nextButton.gameObject.SetActive(waitingForInteraction);
        }
        
        private IEnumerator AutoAdvance(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (tutorialActive && !waitingForInteraction)
            {
                OnNextButtonClicked();
            }
        }
        
        private void OnNextButtonClicked()
        {
            currentStep++;
            
            if (currentStep >= tutorialSteps.Length)
            {
                CompleteTutorial();
            }
            else
            {
                ShowTutorialStep(currentStep);
            }
        }
        
        private void OnSkipButtonClicked()
        {
            SkipTutorial();
        }
        
        private void CompleteTutorial()
        {
            tutorialActive = false;
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.Save();
            
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
                
            OnTutorialCompleted?.Invoke();
        }
        
        private void SkipTutorial()
        {
            tutorialActive = false;
            
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
                
            OnTutorialSkipped?.Invoke();
        }
        
        public bool IsTutorialActive()
        {
            return tutorialActive;
        }
        
        public int GetCurrentStep()
        {
            return currentStep;
        }
    }
}