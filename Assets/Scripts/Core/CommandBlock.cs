using RobotCoder.Core;
using RobotCoder.UI;
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
        
        [Header("Состояние")]
        [SerializeField] protected bool isInWorkspace = false;
        [SerializeField] protected int executionOrder = 0;
        
        protected RectTransform rectTransform;
        protected CanvasGroup canvasGroup;
        
        public bool IsInWorkspace => isInWorkspace;
        public int ExecutionOrder => executionOrder;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
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
        
        public virtual void SetInWorkspace(bool inWorkspace, int order = 0)
        {
            isInWorkspace = inWorkspace;
            executionOrder = order;
            
            if (backgroundImage != null)
            {
                backgroundImage.color = inWorkspace ? blockColor * 1.2f : blockColor;
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
            if (backgroundImage == null) yield break;
            
            Color originalColor = backgroundImage.color;
            backgroundImage.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            backgroundImage.color = originalColor;
        }
    }
}