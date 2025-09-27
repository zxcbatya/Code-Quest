using RobotCoder.Core;
using RobotCoder.UI;
using TMPro;
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
        
        protected RectTransform RectTransform;
        protected CanvasGroup CanvasGroup;
        
        public bool IsInWorkspace => isInWorkspace;
        public int ExecutionOrder => executionOrder;

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            CanvasGroup = GetComponent<CanvasGroup>();
            InitializeBlock();
        }

        public virtual void InitializeBlock()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = blockColor;
            }
                
            if (iconImage && blockIcon)
                iconImage.sprite = blockIcon;
                
            if (commandText)
            {
                commandText.text = GetLocalizedCommandName();
                commandText.color = Color.white;
                commandText.overflowMode = TextOverflowModes.Overflow;
                commandText.alignment = TextAlignmentOptions.Center;
            }
        }

        public virtual string GetLocalizedCommandName()
        {
            string key = commandType.ToString().ToUpper();
            if (LocalizationManager.Instance != null)
            {
                return LocalizationManager.Instance.GetText(key);
            }
            return GetDefaultCommandName();
        }

        private string GetDefaultCommandName()
        {
            switch (commandType)
            { 
                case CommandType.MoveForward: return "Вперед";
                case CommandType.TurnLeft: return "Налево";
                case CommandType.TurnRight: return "Направо";
                case CommandType.Jump: return "Прыжок";
                case CommandType.Interact: return "Действие";
                case CommandType.Repeat: return "Повтор";
                case CommandType.If: return "Если";
                case CommandType.Else: return "Иначе";
                default: return commandType.ToString();
            }
        }

        public abstract bool Execute(RobotController robot);
        
        public virtual void SetInWorkspace(bool inWorkspace, int order = 0)
        {
            isInWorkspace = inWorkspace;
            executionOrder = order;
            
            backgroundImage.color = inWorkspace ? blockColor * 1.2f : blockColor;
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
            backgroundImage.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            backgroundImage.color = originalColor;
        }
        
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}