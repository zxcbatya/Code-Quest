using UnityEngine;
using UnityEngine.EventSystems;
using Core;
using UnityEngine.UI;

namespace UI
{
    public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Drag Settings")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform dragRectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private CommandBlock commandBlock;
        private GameObject placeholder;
        private Transform originalParent;
        private int siblingIndex;
        
        private void Awake()
        {
            commandBlock = GetComponent<CommandBlock>();
            dragRectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Сохраняем оригинальные значения
            originalParent = transform.parent;
            siblingIndex = transform.GetSiblingIndex();
            
            // Создаем placeholder
            CreatePlaceholder();
            
            // Настраиваем визуальные эффекты перетаскивания
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0.6f;
                canvasGroup.blocksRaycasts = false;
            }
            
            // Проигрываем звук
            AudioManager.Instance?.PlaySound("drag_start");
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (dragRectTransform == null) return;
            
            // Перемещаем блок
            dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            // Восстанавливаем визуальные эффекты
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
            
            // Удаляем placeholder
            if (placeholder != null)
            {
                Destroy(placeholder);
            }
            
            // Проверяем, куда был сброшен блок
            DropZone dropZone = GetDropZoneUnderPointer(eventData);
            if (dropZone != null)
            {
                // Блок сброшен в зону сброса
                dropZone.ReceiveBlock(commandBlock);
                AudioManager.Instance?.PlaySound("drop_success");
            }
            else
            {
                // Блок сброшен вне зоны сброса - возвращаем на место
                transform.SetParent(originalParent);
                transform.SetSiblingIndex(siblingIndex);
                AudioManager.Instance?.PlaySound("drop_fail");
            }
        }
        
        private void CreatePlaceholder()
        {
            placeholder = new GameObject("Placeholder");
            placeholder.transform.SetParent(originalParent);
            placeholder.transform.SetSiblingIndex(siblingIndex);
            
            RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
            placeholderRect.sizeDelta = dragRectTransform.sizeDelta;
            placeholderRect.anchoredPosition = dragRectTransform.anchoredPosition;
            placeholderRect.localScale = Vector3.one;
            
            Image placeholderImage = placeholder.AddComponent<Image>();
            placeholderImage.color = new Color(1, 1, 1, 0.3f);
        }
        
        private DropZone GetDropZoneUnderPointer(PointerEventData eventData)
        {
            // Ищем DropZone под указателем
            GameObject pointerObj = eventData.pointerCurrentRaycast.gameObject;
            if (pointerObj != null)
            {
                DropZone dropZone = pointerObj.GetComponentInParent<DropZone>();
                return dropZone;
            }
            
            return null;
        }
    }
}