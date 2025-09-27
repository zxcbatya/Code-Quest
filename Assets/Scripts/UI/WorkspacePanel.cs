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
            if (mainDropZone != null)
            {
                mainDropZone.OnBlockDropped += OnBlockDropped;
            }
            UpdateInstructionText();
            UpdateCommandCount();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
        
        private void OnBlockDropped(CommandBlock block, int slotIndex)
        {
            OnBlockAdded(block);
        }

        private void SetupEventListeners()
        {
            // Очищаем существующие слушатели
            ClearEventListeners();
            
            if (clearAllButton != null)
                clearAllButton.onClick.AddListener(ClearAllBlocks);
            if (undoButton != null)
                undoButton.onClick.AddListener(UndoLastAction);
            if (redoButton != null)
                redoButton.onClick.AddListener(RedoLastAction);
        }
        
        private void ClearEventListeners()
        {
            if (clearAllButton != null)
                clearAllButton.onClick.RemoveAllListeners();
            if (undoButton != null)
                undoButton.onClick.RemoveAllListeners();
            if (redoButton != null)
                redoButton.onClick.RemoveAllListeners();
        }

        public void OnBlockAdded(CommandBlock block)
        {
            AddToUndoStack(block);
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            // Проверяем наличие звука перед его воспроизведением
            if (AudioManager.Instance != null && AudioManager.Instance.HasSound(dropSoundName))
            {
                AudioManager.Instance.PlaySound(dropSoundName);
            }
        }

        public void OnBlockRemoved(CommandBlock block)
        {
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
        }

        private void UpdateCommandCount()
        {
            int commandCount = mainDropZone != null ? mainDropZone.BlockCount : 0;
            if (commandCountText != null)
            {
                commandCountText.SetText(commandCount.ToString());
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
            bool hasBlocks = mainDropZone != null && mainDropZone.BlockCount > 0;
            
            if (instructionText != null)
            {
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
        }

        private void ClearAllBlocks()
        {
            if (mainDropZone == null) return;
            
            var currentBlocks = new List<CommandBlock>(mainDropZone.Blocks);
            _undoStack.AddRange(currentBlocks);
            
            if (_undoStack.Count > _maxUndoSteps)
            {
                int excess = _undoStack.Count - _maxUndoSteps;
                for (int i = 0; i < excess; i++)
                {
                    if (_undoStack[i] != null)
                    {
                        Destroy(_undoStack[i].gameObject);
                    }
                }
                _undoStack.RemoveRange(0, excess);
            }
            
            foreach (var block in currentBlocks)
            {
                if (block != null)
                {
                    ReturnBlockToPalette(block);
                }
            }
            _redoStack.Clear();
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            OnWorkspaceCleared?.Invoke();
            // Проверяем наличие звука перед его воспроизведением
            if (AudioManager.Instance != null && AudioManager.Instance.HasSound(clearSoundName))
            {
                AudioManager.Instance.PlaySound(clearSoundName);
            }
        }

        private void AddToUndoStack(CommandBlock block)
        {
            if (block == null) return;
            
            _undoStack.Add(block);
            
            if (_undoStack.Count > _maxUndoSteps)
            {
                if (_undoStack[0] != null)
                {
                    Destroy(_undoStack[0].gameObject);
                }
                _undoStack.RemoveAt(0);
            }
            
            _redoStack.Clear();
        }

        private void UndoLastAction()
        {
            if (_undoStack.Count == 0) return;
            
            var lastBlock = _undoStack[_undoStack.Count - 1];
            _undoStack.RemoveAt(_undoStack.Count - 1);
            
            bool blockExists = lastBlock != null && lastBlock.transform.parent == mainDropZone?.transform;
            
            if (blockExists)
            {
                _redoStack.Add(lastBlock);
                ReturnBlockToPalette(lastBlock);
            }
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            // Проверяем наличие звука перед его воспроизведением
            if (AudioManager.Instance != null && AudioManager.Instance.HasSound("undo"))
            {
                AudioManager.Instance.PlaySound("undo");
            }
        }

        private void RedoLastAction()
        {
            if (_redoStack.Count == 0) return;
            
            var lastBlock = _redoStack[_redoStack.Count - 1];
            _redoStack.RemoveAt(_redoStack.Count - 1);
            
            if (lastBlock != null && mainDropZone != null)
            {
                mainDropZone.AcceptBlock(lastBlock);
                _undoStack.Add(lastBlock);
            }
            
            UpdateCommandCount();
            UpdateWorkspaceVisual();
            UpdateUndoRedoButtons();
            
            // Проверяем наличие звука перед его воспроизведением
            if (AudioManager.Instance != null && AudioManager.Instance.HasSound("redo"))
            {
                AudioManager.Instance.PlaySound("redo");
            }
        }

        private void UpdateUndoRedoButtons()
        {
            if (undoButton != null)
                undoButton.interactable = _undoStack.Count > 0;
            if (redoButton != null)
                redoButton.interactable = _redoStack.Count > 0;
        }

        public void SetInteractable(bool interactable)
        {
            if (clearAllButton != null)
                clearAllButton.interactable = interactable;
            if (undoButton != null)
                undoButton.interactable = interactable && _undoStack.Count > 0;
            if (redoButton != null)
                redoButton.interactable = interactable && _redoStack.Count > 0;
        }

        public void HighlightWorkspace(bool highlight)
        {
            if (backgroundImage == null) return;
            
            Color targetColor = highlight ? Color.yellow : (mainDropZone != null && mainDropZone.BlockCount > 0 ? filledWorkspaceColor : emptyWorkspaceColor);
            backgroundImage.color = targetColor;
        }

        public CommandBlock[] GetAllBlocks()
        {
            return mainDropZone != null ? mainDropZone.GetOrderedBlocks() : new CommandBlock[0];
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

        private void OnDestroy()
        {
            // Очищаем слушатели событий
            ClearEventListeners();
            
            if (mainDropZone != null)
            {
                mainDropZone.OnBlockDropped -= OnBlockDropped;
            }
            
            // Очищаем стеки
            foreach (var block in _undoStack)
            {
                if (block != null)
                {
                    Destroy(block.gameObject);
                }
            }
            _undoStack.Clear();
            
            foreach (var block in _redoStack)
            {
                if (block != null)
                {
                    Destroy(block.gameObject);
                }
            }
            _redoStack.Clear();
            
            OnCommandCountChanged = null;
            OnWorkspaceCleared = null;
        }
        
        private void OnDisable()
        {
            // Очищаем слушатели событий при отключении
            ClearEventListeners();
        }

        private void ReturnBlockToPalette(CommandBlock block)
        {
            if (block == null || paletteContent == null) return;
            
            block.transform.SetParent(paletteContent);
            
            var rect = block.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.sizeDelta = new Vector2(150, 50);
            }
            
            var canvasGroup = block.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
            
            block.SetInWorkspace(false, -1);
            
            OnBlockRemoved(block);
        }
    }
}