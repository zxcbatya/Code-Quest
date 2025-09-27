using UnityEngine;
using UnityEngine.EventSystems;
using Core;

namespace RobotCoder.UI
{
    public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private float dragAlpha = 0.8f;
        
        private CommandBlock _commandBlock;
        private GameObject _dragPreview;
        private Transform _originalParent;
        private Vector3 _originalPosition;
        private bool _isDragging;

        private void Awake()
        {
            _commandBlock = GetComponent<CommandBlock>();
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragging || canvas == null) return;
            
            _isDragging = true;
            _originalParent = transform.parent;
            _originalPosition = transform.position;
            
            CreateDragPreview();
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector3 worldPoint);
            _dragPreview.transform.position = worldPoint;
            
            // Для блоков из палитры не меняем прозрачность, они должны оставаться видимыми
            if (!IsFromPalette())
            {
                var originalCanvasGroup = GetComponent<CanvasGroup>();
                if (originalCanvasGroup != null)
                {
                    originalCanvasGroup.alpha = 0f;
                    originalCanvasGroup.blocksRaycasts = false;
                }
            }
        }

        private void CreateDragPreview()
        {
            _dragPreview = Instantiate(gameObject, canvas.transform);
            _dragPreview.name = "DragPreview";
            
            var dragDropHandler = _dragPreview.GetComponent<DragDropHandler>();
            if (dragDropHandler != null)
            {
                DestroyImmediate(dragDropHandler);
            }
            
            var rect = _dragPreview.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.localScale = Vector3.one;
                rect.sizeDelta = new Vector2(120, 60);
            }
            
            var canvasGroup = _dragPreview.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = dragAlpha;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || canvas == null || _dragPreview == null) return;
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector3 worldPoint);
            _dragPreview.transform.position = worldPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            _isDragging = false;
            
            var dropZone = GetDropZoneUnderPointer(eventData);
            if (dropZone != null)
            {
                if (IsFromPalette())
                {
                    CreateBlockInWorkspace(dropZone);
                }
                else
                {
                    MoveBlockToDropZone(dropZone);
                }
            }
            else
            {
                RestoreOriginalState();
            }
            
            CleanupDragState();
        }

        private void CreateBlockInWorkspace(DropZone dropZone)
        {
            // Create a new block instead of moving the original
            GameObject newBlock = Instantiate(gameObject, dropZone.transform);
            var newCommandBlock = newBlock.GetComponent<CommandBlock>();
            
            // Remove the DragDropHandler from the new block since it's now in the workspace
            var dragDropHandler = newBlock.GetComponent<DragDropHandler>();
            if (dragDropHandler != null)
            {
                Destroy(dragDropHandler);
            }
            
            SetupBlockForWorkspace(newBlock);
            if (newCommandBlock != null)
            {
                try
                {
                    newCommandBlock.SetInWorkspace(true, dropZone.BlockCount);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error setting command block in workspace: {e.Message}");
                }
            }
            dropZone.OnBlockDropped?.Invoke(newCommandBlock, dropZone.slotIndex);
            
            // For palette blocks, we don't need to restore anything since the original stays in place
        }

        private void MoveBlockToDropZone(DropZone dropZone)
        {
            transform.SetParent(dropZone.transform);
            SetupBlockForWorkspace(gameObject);
            if (_commandBlock != null)
            {
                _commandBlock.SetInWorkspace(true, dropZone.BlockCount);
            }
            dropZone.OnBlockDropped?.Invoke(_commandBlock, dropZone.slotIndex);
        }

        
        private void SetupBlockForWorkspace(GameObject blockObj)
        {
            if (blockObj == null) return;
            
            var rect = blockObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.sizeDelta = new Vector2(120, 60);
            }
            
            var canvasGroup = blockObj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void RestoreOriginalState()
        {
            // Only restore non-palette blocks
            if (!IsFromPalette())
            {
                transform.SetParent(_originalParent);
                transform.position = _originalPosition;
                
                var originalCanvasGroup = GetComponent<CanvasGroup>();
                if (originalCanvasGroup != null)
                {
                    originalCanvasGroup.alpha = 1f;
                    originalCanvasGroup.blocksRaycasts = true;
                }
            }
        }

        private void CleanupDragState()
        {
            if (_dragPreview != null)
            {
                Destroy(_dragPreview);
                _dragPreview = null;
            }
            
            // For palette blocks, we don't need to cleanup since they stay in place
            if (!IsFromPalette())
            {
                var canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        private DropZone GetDropZoneUnderPointer(PointerEventData eventData)
        {
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                var dropZone = result.gameObject.GetComponent<DropZone>();
                if (dropZone != null) return dropZone;
            }
            return null;
        }
        
        // Public method to check if this block is from the palette
        public bool IsFromPalette()
        {
            // Check if the parent or any ancestor has a BlockPalette component
            Transform currentParent = transform.parent;
            while (currentParent != null)
            {
                if (currentParent.GetComponent<BlockPalette>() != null)
                {
                    return true;
                }
                currentParent = currentParent.parent;
            }
            return false;
        }
    }
}