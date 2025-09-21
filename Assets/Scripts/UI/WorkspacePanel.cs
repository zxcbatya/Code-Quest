using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Core;
using UI;

namespace RobotCoder.UI
{
    public class WorkspacePanel : MonoBehaviour
    {
        [Header("Основные компоненты")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentArea;
        [SerializeField] private DropZone mainDropZone;
        [SerializeField] private Button clearAllButton;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button redoButton;

        [Header("Информация")]
        [SerializeField] private TextMeshProUGUI commandCountText;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Image backgroundImage;

        [Header("Визуальные настройки")]
        [SerializeField] private Color emptyWorkspaceColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        [SerializeField] private Color filledWorkspaceColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
        [SerializeField] private float gridLineSpacing = 50f;
        [SerializeField] private bool showGridLines = true;

        [Header("Звуки")]
        [SerializeField] private string dropSoundName = "drop_success";
        [SerializeField] private string clearSoundName = "clear_workspace";

        private List<CommandBlock> undoStack = new List<CommandBlock>();
        private List<CommandBlock> redoStack = new List<CommandBlock>();
        private int maxUndoSteps = 10;

        public System.Action<int> OnCommandCountChanged;
        public System.Action OnWorkspaceCleared;

        private void Start()
        {
            InitializeWorkspace();
            SetupEventListeners();
            UpdateWorkspaceVisual();
        }

        private void InitializeWorkspace()
        {
            if (mainDropZone == null)
            {
                mainDropZone = GetComponentInChildren<DropZone>();
            }

            UpdateInstructionText();
            UpdateCommandCount();
            
            // Настраиваем прокрутку
            if (scrollRect != null)
            {
                scrollRect.verticalScrollbar.value = 1f; // Прокрутка вверх
            }
        }

        private void SetupEventListeners()
        {
            if (clearAllButton) clearAllButton.onClick.AddListener(ClearAllBlocks);
            if (undoButton) undoButton.onClick.AddListener(UndoLastAction);
            if (redoButton) redoButton.onClick.AddListener(RedoLastAction);

            // Подписываемся на события drop zone
            if (mainDropZone != null)
            {
                // Эти события нужно будет добавить в DropZone класс
            }
        }

        public void OnBlockAdded(CommandBlock block)
        {
            // Добавляем в стек отмены
            AddToUndoStack(block);
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            AudioManager.Instance?.PlaySound(dropSoundName);
        }

        public void OnBlockRemoved(CommandBlock block)
        {
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
        }

        private void UpdateCommandCount()
        {
            int commandCount = 0;
            
            if (mainDropZone != null)
            {
                commandCount = mainDropZone.BlockCount;
            }
            
            if (commandCountText != null)
            {
                commandCountText.text = commandCount.ToString();
            }
            
            OnCommandCountChanged?.Invoke(commandCount);
        }

        private void UpdateWorkspaceVisual()
        {
            bool hasBlocks = mainDropZone != null && mainDropZone.BlockCount > 0;
            
            if (backgroundImage != null)
            {
                backgroundImage.color = hasBlocks ? filledWorkspaceColor : emptyWorkspaceColor;
            }
            
            UpdateInstructionText();
        }

        private void UpdateInstructionText()
        {
            if (instructionText == null) return;
            
            bool hasBlocks = mainDropZone != null && mainDropZone.BlockCount > 0;
            
            if (hasBlocks)
            {
                string runText = LocalizationManager.Instance?.GetText("PRESS_START_TO_RUN") 
                    ?? "Нажмите СТАРТ для запуска";
                instructionText.text = runText;
                instructionText.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);
            }
            else
            {
                string dragText = LocalizationManager.Instance?.GetText("DRAG_BLOCKS_HERE") 
                    ?? "Перетащите блоки сюда";
                instructionText.text = dragText;
                instructionText.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }
        }

        private void ClearAllBlocks()
        {
            if (mainDropZone != null)
            {
                // Сохраняем состояние для отмены
                var currentBlocks = new List<CommandBlock>(mainDropZone.Blocks);
                if (currentBlocks.Count > 0)
                {
                    undoStack.AddRange(currentBlocks);
                    if (undoStack.Count > maxUndoSteps)
                    {
                        int excess = undoStack.Count - maxUndoSteps;
                        undoStack.RemoveRange(0, excess);
                    }
                }
                
                mainDropZone.ClearAllBlocks();
                redoStack.Clear();
            }
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            OnWorkspaceCleared?.Invoke();
            AudioManager.Instance?.PlaySound(clearSoundName);
        }

        private void AddToUndoStack(CommandBlock block)
        {
            undoStack.Add(block);
            
            if (undoStack.Count > maxUndoSteps)
            {
                undoStack.RemoveAt(0);
            }
            
            redoStack.Clear(); // Очищаем redo при новом действии
        }

        private void UndoLastAction()
        {
            if (undoStack.Count == 0 || mainDropZone == null) return;
            
            var lastBlock = undoStack[undoStack.Count - 1];
            undoStack.RemoveAt(undoStack.Count - 1);
            
            if (mainDropZone.Blocks.Contains(lastBlock))
            {
                redoStack.Add(lastBlock);
                mainDropZone.RemoveBlock(lastBlock);
            }
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            AudioManager.Instance?.PlaySound("undo");
        }

        private void RedoLastAction()
        {
            if (redoStack.Count == 0 || mainDropZone == null) return;
            
            var lastBlock = redoStack[redoStack.Count - 1];
            redoStack.RemoveAt(redoStack.Count - 1);
            
            mainDropZone.AcceptBlock(lastBlock);
            undoStack.Add(lastBlock);
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            AudioManager.Instance?.PlaySound("redo");
        }

        private void UpdateUndoRedoButtons()
        {
            if (undoButton) undoButton.interactable = undoStack.Count > 0;
            if (redoButton) redoButton.interactable = redoStack.Count > 0;
        }

        public void SetInteractable(bool interactable)
        {
            if (clearAllButton) clearAllButton.interactable = interactable;
            if (undoButton) undoButton.interactable = interactable && undoStack.Count > 0;
            if (redoButton) redoButton.interactable = interactable && redoStack.Count > 0;
        }

        public void HighlightWorkspace(bool highlight)
        {
            if (backgroundImage != null)
            {
                Color targetColor = highlight ? Color.yellow : 
                    (mainDropZone?.BlockCount > 0 ? filledWorkspaceColor : emptyWorkspaceColor);
                backgroundImage.color = targetColor;
            }
        }

        public CommandBlock[] GetAllBlocks()
        {
            return mainDropZone?.GetOrderedBlocks() ?? new CommandBlock[0];
        }

        public bool HasBlocks()
        {
            return mainDropZone != null && mainDropZone.BlockCount > 0;
        }

        public void ScrollToTop()
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        public void ScrollToBottom()
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGridLines || contentArea == null) return;
            
            Gizmos.color = Color.gray;
            
            Vector3 position = contentArea.position;
            Vector2 size = contentArea.sizeDelta;
            
            // Вертикальные линии
            for (float x = position.x - size.x/2; x <= position.x + size.x/2; x += gridLineSpacing)
            {
                Gizmos.DrawLine(
                    new Vector3(x, position.y - size.y/2, 0),
                    new Vector3(x, position.y + size.y/2, 0)
                );
            }
            
            // Горизонтальные линии
            for (float y = position.y - size.y/2; y <= position.y + size.y/2; y += gridLineSpacing)
            {
                Gizmos.DrawLine(
                    new Vector3(position.x - size.x/2, y, 0),
                    new Vector3(position.x + size.x/2, y, 0)
                );
            }
        }
    }
}