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
        private bool _isFromPalette;
        private bool _isDragging;

        private void Awake()
        {
            _commandBlock = GetComponent<CommandBlock>();
            _isFromPalette = transform.parent.GetComponent<BlockPalette>() != null;
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
            
            var originalCanvasGroup = GetComponent<CanvasGroup>();
            originalCanvasGroup.alpha = _isFromPalette ? 0.1f : 0f;
            originalCanvasGroup.blocksRaycasts = false;
        }

        private void CreateDragPreview()
        {
            _dragPreview = Instantiate(gameObject, canvas.transform);
            _dragPreview.name = "DragPreview";
            DestroyImmediate(_dragPreview.GetComponent<DragDropHandler>());
            
            var rect = _dragPreview.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(120, 60);
            
            var canvasGroup = _dragPreview.GetComponent<CanvasGroup>();
            canvasGroup.alpha = dragAlpha;
            canvasGroup.blocksRaycasts = false;
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
                if (_isFromPalette)
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
            GameObject newBlock = Instantiate(gameObject, dropZone.transform);
            var newCommandBlock = newBlock.GetComponent<CommandBlock>();
            
            SetupBlockForWorkspace(newBlock);
            newCommandBlock.SetInWorkspace(true, dropZone.BlockCount);
            dropZone.OnBlockDropped?.Invoke(newCommandBlock, dropZone.slotIndex);
        }

        private void MoveBlockToDropZone(DropZone dropZone)
        {
            transform.SetParent(dropZone.transform);
            SetupBlockForWorkspace(gameObject);
            _commandBlock.SetInWorkspace(true, dropZone.BlockCount);
            dropZone.OnBlockDropped?.Invoke(_commandBlock, dropZone.slotIndex);
        }

        private void SetupBlockForWorkspace(GameObject blockObj)
        {
            var rect = blockObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(120, 60);
            
            var canvasGroup = blockObj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        private void RestoreOriginalState()
        {
            transform.SetParent(_originalParent);
            transform.position = _originalPosition;
            
            var originalCanvasGroup = GetComponent<CanvasGroup>();
            originalCanvasGroup.alpha = 1f;
            originalCanvasGroup.blocksRaycasts = true;
        }

        private void CleanupDragState()
        {
            if (_dragPreview != null)
            {
                Destroy(_dragPreview);
                _dragPreview = null;
            }
            
            if (_isFromPalette)
            {
                GetComponent<CanvasGroup>().alpha = 1f;
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
    }
}