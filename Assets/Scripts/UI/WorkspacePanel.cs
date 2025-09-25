using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Core;

namespace RobotCoder.UI
{
    public class WorkspacePanel : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentArea;
        [SerializeField] private DropZone mainDropZone;
        [SerializeField] private Button clearAllButton;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button redoButton;
        [SerializeField] private TextMeshProUGUI commandCountText;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color emptyWorkspaceColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        [SerializeField] private Color filledWorkspaceColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
        [SerializeField] private string dropSoundName = "drop_success";
        [SerializeField] private string clearSoundName = "clear_workspace";

        [SerializeField] private Transform paletteContent;

        private readonly List<CommandBlock> _undoStack = new List<CommandBlock>();
        private readonly List<CommandBlock> _redoStack = new List<CommandBlock>();
        private readonly int _maxUndoSteps = 10;

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
            mainDropZone.OnBlockDropped += OnBlockDropped;
            UpdateInstructionText();
            UpdateCommandCount();
            scrollRect.verticalNormalizedPosition = 1f;
        }
        
        private void OnBlockDropped(CommandBlock block, int slotIndex)
        {
            OnBlockAdded(block);
        }

        private void SetupEventListeners()
        {
            clearAllButton.onClick.AddListener(ClearAllBlocks);
            undoButton.onClick.AddListener(UndoLastAction);
            redoButton.onClick.AddListener(RedoLastAction);
        }

        public void OnBlockAdded(CommandBlock block)
        {
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
            int commandCount = mainDropZone.BlockCount;
            commandCountText.SetText(commandCount.ToString());
            OnCommandCountChanged?.Invoke(commandCount);
        }

        private void UpdateWorkspaceVisual()
        {
            bool hasBlocks = mainDropZone.BlockCount > 0;
            backgroundImage.color = hasBlocks ? filledWorkspaceColor : emptyWorkspaceColor;
            UpdateInstructionText();
        }

        private void UpdateInstructionText()
        {
            bool hasBlocks = mainDropZone.BlockCount > 0;
            
            if (hasBlocks)
            {
                string runText = LocalizationManager.Instance?.GetText("PRESS_START_TO_RUN") ?? "Нажмите СТАРТ для запуска";
                instructionText.text = runText;
                instructionText.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);
            }
            else
            {
                string dragText = LocalizationManager.Instance?.GetText("DRAG_BLOCKS_HERE") ?? "Перетащите блоки сюда";
                instructionText.text = dragText;
                instructionText.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }
        }

        private void ClearAllBlocks()
        {
            var currentBlocks = new List<CommandBlock>(mainDropZone.Blocks);
            _undoStack.AddRange(currentBlocks);
            
            if (_undoStack.Count > _maxUndoSteps)
            {
                int excess = _undoStack.Count - _maxUndoSteps;
                for (int i = 0; i < excess; i++)
                {
                    Destroy(_undoStack[i].gameObject);
                }
                _undoStack.RemoveRange(0, excess);
            }
            
            foreach (var block in currentBlocks)
            {
                ReturnBlockToPalette(block);
            }
            _redoStack.Clear();
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            OnWorkspaceCleared?.Invoke();
            AudioManager.Instance?.PlaySound(clearSoundName);
        }

        private void AddToUndoStack(CommandBlock block)
        {
            _undoStack.Add(block);
            
            if (_undoStack.Count > _maxUndoSteps)
            {
                Destroy(_undoStack[0].gameObject);
                _undoStack.RemoveAt(0);
            }
            
            _redoStack.Clear();
        }

        private void UndoLastAction()
        {
            if (_undoStack.Count == 0) return;
            
            var lastBlock = _undoStack[_undoStack.Count - 1];
            _undoStack.RemoveAt(_undoStack.Count - 1);
            
            bool blockExists = lastBlock.transform.parent == mainDropZone.transform;
            
            if (blockExists)
            {
                _redoStack.Add(lastBlock);
                ReturnBlockToPalette(lastBlock);
            }
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            AudioManager.Instance?.PlaySound("undo");
        }

        private void RedoLastAction()
        {
            if (_redoStack.Count == 0) return;
            
            var lastBlock = _redoStack[_redoStack.Count - 1];
            _redoStack.RemoveAt(_redoStack.Count - 1);
            
            mainDropZone.AcceptBlock(lastBlock);
            _undoStack.Add(lastBlock);
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            AudioManager.Instance?.PlaySound("redo");
        }

        private void UpdateUndoRedoButtons()
        {
            undoButton.interactable = _undoStack.Count > 0;
            redoButton.interactable = _redoStack.Count > 0;
        }

        public void SetInteractable(bool interactable)
        {
            clearAllButton.interactable = interactable;
            undoButton.interactable = interactable && _undoStack.Count > 0;
            redoButton.interactable = interactable && _redoStack.Count > 0;
        }

        public void HighlightWorkspace(bool highlight)
        {
            Color targetColor = highlight ? Color.yellow : (mainDropZone.BlockCount > 0 ? filledWorkspaceColor : emptyWorkspaceColor);
            backgroundImage.color = targetColor;
        }

        public CommandBlock[] GetAllBlocks()
        {
            return mainDropZone.GetOrderedBlocks();
        }

        public bool HasBlocks()
        {
            return mainDropZone.BlockCount > 0;
        }

        public void ScrollToTop()
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        public void ScrollToBottom()
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void OnDestroy()
        {
            mainDropZone.OnBlockDropped -= OnBlockDropped;
        }

        private void ReturnBlockToPalette(CommandBlock block)
        {
            block.transform.SetParent(paletteContent);
            
            var rect = block.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(150, 50);
            
            var canvasGroup = block.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            
            block.SetInWorkspace(false, -1);
            
            OnBlockRemoved(block);
        }
    }
}