
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Drag Settings")]
        [SerializeField] private LayerMask dropZoneLayer = -1;
        
        private CommandBlock commandBlock;
        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 originalPosition;
        private Transform originalParent;
        private bool isFromPalette = true;
        
        public static DragDropHandler CurrentDragging { get; private set; }
        public CommandBlock CommandBlock => commandBlock;

        private void Awake()
        {
            commandBlock = GetComponent<CommandBlock>();
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CurrentDragging != null) return;
            
            CurrentDragging = this;
            originalPosition = rectTransform.anchoredPosition;
            originalParent = transform.parent;
            
            if (isFromPalette)
            {
                CreateDragCopy();
            }
            
            commandBlock.StartDrag();
            AudioManager.Instance?.PlaySound("drag_start");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (CurrentDragging != this) return;
            
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out position
            );
            
            rectTransform.anchoredPosition = position;
            
            CheckDropZones(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (CurrentDragging != this) return;
            
            CurrentDragging = null;
            commandBlock.EndDrag();
            
            DropZone dropZone = FindDropZone(eventData.position);
            
            if (dropZone != null && dropZone.CanAcceptBlock(commandBlock))
            {
                dropZone.AcceptBlock(commandBlock);
                AudioManager.Instance?.PlaySound("drop_success");
            }
            else
            {
                HandleFailedDrop();
                AudioManager.Instance?.PlaySound("drop_fail");
            }
        }

        private void CreateDragCopy()
        {
            GameObject copyObj = Instantiate(gameObject, transform.parent);
            CommandBlock copyBlock = copyObj.GetComponent<CommandBlock>();
            DragDropHandler copyHandler = copyObj.GetComponent<DragDropHandler>();
            
            copyHandler.isFromPalette = false;
            copyHandler.originalPosition = originalPosition;
            copyHandler.originalParent = originalParent;
            
            CurrentDragging = copyHandler;
            copyHandler.commandBlock = copyBlock;
            copyHandler.rectTransform = copyObj.GetComponent<RectTransform>();
            
            commandBlock.EndDrag();
        }

        private void CheckDropZones(Vector2 screenPosition)
        {
            DropZone[] allDropZones = FindObjectsOfType<DropZone>();
            
            foreach (var dropZone in allDropZones)
            {
                bool isOver = RectTransformUtility.RectangleContainsScreenPoint(
                    dropZone.GetComponent<RectTransform>(),
                    screenPosition,
                    canvas.worldCamera
                );
                
                dropZone.SetHighlight(isOver && dropZone.CanAcceptBlock(commandBlock));
            }
        }

        private DropZone FindDropZone(Vector2 screenPosition)
        {
            DropZone[] allDropZones = FindObjectsOfType<DropZone>();
            
            foreach (var dropZone in allDropZones)
            {
                bool isOver = RectTransformUtility.RectangleContainsScreenPoint(
                    dropZone.GetComponent<RectTransform>(),
                    screenPosition,
                    canvas.worldCamera
                );
                
                if (isOver)
                {
                    return dropZone;
                }
            }
            
            return null;
        }

        private void HandleFailedDrop()
        {
            if (isFromPalette)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(ReturnToOriginalPosition());
            }
        }

        private System.Collections.IEnumerator ReturnToOriginalPosition()
        {
            Vector2 startPos = rectTransform.anchoredPosition;
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, progress);
                yield return null;
            }
            
            rectTransform.anchoredPosition = originalPosition;
            transform.SetParent(originalParent);
        }

        public void SetFromPalette(bool fromPalette)
        {
            isFromPalette = fromPalette;
        }
    }
}