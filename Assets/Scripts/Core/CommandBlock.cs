
using RobotCoder.Core;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public enum CommandType
    {
        MoveForward,
        TurnLeft,
        TurnRight,
        Jump,
        Interact,
        Repeat,
        If,
        Else
    }

    [System.Serializable]
    public abstract class CommandBlock : MonoBehaviour
    {
        [Header("Базовые настройки блока")]
        public CommandType commandType;
        public string commandName;
        public Color blockColor = Color.blue;
        public Sprite blockIcon;
        
        [Header("UI Компоненты")]
        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected Image iconImage;
        [SerializeField] protected TextMeshProUGUI commandText;
        [SerializeField] protected Button blockButton;
        
        [Header("Drag & Drop")]
        [SerializeField] protected bool isDragging = false;
        [SerializeField] protected bool isInWorkspace = false;
        [SerializeField] protected int executionOrder = 0;
        
        protected RectTransform rectTransform;
        protected Canvas parentCanvas;
        protected CanvasGroup canvasGroup;
        
        public bool IsDragging => isDragging;
        public bool IsInWorkspace => isInWorkspace;
        public int ExecutionOrder => executionOrder;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                
            parentCanvas = GetComponentInParent<Canvas>();
            InitializeBlock();
        }

        public virtual void InitializeBlock()
        {
            if (backgroundImage != null)
                backgroundImage.color = blockColor;
                
            if (iconImage != null && blockIcon != null)
                iconImage.sprite = blockIcon;
                
            if (commandText != null)
                commandText.text = GetLocalizedCommandName();
        }

        public virtual string GetLocalizedCommandName()
        {
            if (LocalizationManager.Instance != null)
            {
                return LocalizationManager.Instance.GetText(commandType.ToString().ToUpper());
            }
            return commandName;
        }

        public abstract bool Execute(RobotController robot);
        
        public virtual void StartDrag()
        {
            isDragging = true;
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;
            
            transform.SetAsLastSibling();
        }
        
        public virtual void EndDrag()
        {
            isDragging = false;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        public virtual void SetInWorkspace(bool inWorkspace, int order = 0)
        {
            isInWorkspace = inWorkspace;
            executionOrder = order;
            
            // Визуальная обратная связь
            if (backgroundImage != null)
            {
                backgroundImage.color = inWorkspace ? 
                    blockColor * 1.2f : blockColor;
            }
        }
        
        public virtual CommandBlock Clone()
        {
            GameObject cloneObj = Instantiate(gameObject);
            CommandBlock cloneBlock = cloneObj.GetComponent<CommandBlock>();
            cloneBlock.isInWorkspace = false;
            cloneBlock.executionOrder = 0;
            return cloneBlock;
        }
        
        public virtual void HighlightExecution()
        {
            StartCoroutine(ExecutionHighlight());
        }
        
        protected virtual System.Collections.IEnumerator ExecutionHighlight()
        {
            Color originalColor = backgroundImage.color;
            Color highlightColor = Color.yellow;
            
            // Подсветка при выполнении
            backgroundImage.color = highlightColor;
            yield return new WaitForSeconds(0.5f);
            backgroundImage.color = originalColor;
        }
    }
}