using UnityEngine;
using UnityEngine.EventSystems;
using Core;
using UnityEngine.UI;

namespace UI
{
    public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Visual Feedback")]
        [SerializeField] private Color highlightColor = Color.green;
        [SerializeField] private Color normalColor = Color.white;
        
        private Image backgroundImage;
        public int slotIndex = -1;
        
        // Событие сброса блока
        public System.Action<CommandBlock, int> OnBlockDropped;
        
        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            if (backgroundImage != null)
            {
                normalColor = backgroundImage.color;
            }
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            // Получаем перетаскиваемый блок
            DragDropHandler dragHandler = eventData.pointerDrag?.GetComponent<DragDropHandler>();
            if (dragHandler != null)
            {
                CommandBlock commandBlock = dragHandler.GetComponent<CommandBlock>();
                if (commandBlock != null)
                {
                    ReceiveBlock(commandBlock);
                }
            }
            
            // Восстанавливаем нормальный цвет
            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
        }
        
        public void ReceiveBlock(CommandBlock commandBlock)
        {
            if (commandBlock == null) return;
            
            // Создаем копию блока, если он из палитры
            bool isFromPalette = commandBlock.transform.parent.CompareTag("BlockPalette");
            CommandBlock blockToPlace = commandBlock;
            
            if (isFromPalette)
            {
                // Создаем копию блока
                GameObject blockCopy = Instantiate(commandBlock.gameObject, transform);
                blockToPlace = blockCopy.GetComponent<CommandBlock>();
                
                // Настраиваем копию
                RectTransform rectTransform = blockToPlace.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.localScale = Vector3.one;
                }
                
                // Добавляем DragDropHandler для новой копии
                if (blockToPlace.GetComponent<DragDropHandler>() == null)
                {
                    blockToPlace.gameObject.AddComponent<DragDropHandler>();
                }
            }
            else
            {
                // Перемещаем существующий блок
                commandBlock.transform.SetParent(transform);
                RectTransform rectTransform = commandBlock.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.localScale = Vector3.one;
                }
            }
            
            // Вызываем событие
            OnBlockDropped?.Invoke(blockToPlace, slotIndex);
        }

        // --- API expected by WorkspacePanel ---
        private readonly System.Collections.Generic.List<CommandBlock> _blocks = new System.Collections.Generic.List<CommandBlock>();

        public int BlockCount => _blocks.Count;

        public System.Collections.Generic.IReadOnlyList<CommandBlock> Blocks => _blocks;

        public void ClearAllBlocks()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                var block = child.GetComponent<CommandBlock>();
                if (block != null)
                {
                    _blocks.Remove(block);
                }
                Destroy(child.gameObject);
            }
        }

        public void RemoveBlock(CommandBlock block)
        {
            if (block == null) return;
            _blocks.Remove(block);
            if (block.transform.parent == transform)
            {
                Destroy(block.gameObject);
            }
        }

        public void AcceptBlock(CommandBlock block)
        {
            if (block == null) return;
            block.transform.SetParent(transform);
            var rect = block.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }
            if (!_blocks.Contains(block))
                _blocks.Add(block);
        }

        public CommandBlock[] GetOrderedBlocks()
        {
            return _blocks.ToArray();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Проверяем, есть ли перетаскиваемый блок
            if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<CommandBlock>() != null)
            {
                // Подсвечиваем зону сброса
                if (backgroundImage != null)
                {
                    backgroundImage.color = highlightColor;
                }
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            // Восстанавливаем нормальный цвет
            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
        }
    }
}