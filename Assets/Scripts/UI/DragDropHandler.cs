using Core;
using RobotCoder.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
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
            _isDragging = true;
            _originalParent = transform.parent;
            _originalPosition = transform.position;

            CreateDragPreview();

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector3 worldPoint);
            _dragPreview.transform.position = worldPoint;

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

            DestroyImmediate(dragDropHandler);

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
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform,
                eventData.position, canvas.worldCamera, out Vector3 worldPoint);
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
            GameObject newBlock = Instantiate(gameObject, dropZone.transform);
            var newCommandBlock = newBlock.GetComponent<CommandBlock>();

            var dragDropHandler = newBlock.GetComponent<DragDropHandler>();
            Destroy(dragDropHandler);

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
            if (!IsFromPalette())
            {
                transform.SetParent(_originalParent);
                transform.position = _originalPosition;

                var originalCanvasGroup = GetComponent<CanvasGroup>();

                originalCanvasGroup.alpha = 1f;
                originalCanvasGroup.blocksRaycasts = true;
            }
        }

        private void CleanupDragState()
        {
            Destroy(_dragPreview);
            _dragPreview = null;

            if (!IsFromPalette())
            {
                var canvasGroup = GetComponent<CanvasGroup>();

                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
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

        public bool IsFromPalette()
        {
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