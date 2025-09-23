using UnityEngine;
using UnityEngine.EventSystems;
using Core;
using UnityEngine.UI;

namespace RobotCoder.UI
{
    /// <summary>
    /// Обработчик перетаскивания блоков между палитрой и рабочей областью
    /// Обеспечивает корректное создание копий и управление жизненным циклом объектов
    /// </summary>
    public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Drag Settings")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform dragRectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float dragAlpha = 0.8f;
        [SerializeField] private float dragScale = 1.1f;

        [Header("Visual Feedback")]
        [SerializeField] private DragVisualFeedback visualFeedback;

        // Состояние перетаскивания
        private CommandBlock commandBlock;
        private GameObject dragPreview; // Визуальная копия для перетаскивания
        private Transform originalParent;
        private int siblingIndex;
        private Vector3 originalScale;
        private Vector3 originalPosition;
        private bool isFromPalette = false;
        private bool isDragging = false;

        private void Awake()
        {
            InitializeComponents();
            DetermineSourceLocation();
            SetupVisualFeedback();
        }

        private void InitializeComponents()
        {
            commandBlock = GetComponent<CommandBlock>();
            dragRectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Автоматический поиск Canvas
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
                if (canvas == null)
                {
                    canvas = FindObjectOfType<Canvas>();
                    Debug.Log($"✓ Canvas автоматически найден: {canvas?.name}");
                }
            }

            if (dragRectTransform == null)
            {
                dragRectTransform = gameObject.AddComponent<RectTransform>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void DetermineSourceLocation()
        {
            // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Улучшенное определение блока из палитры
            isFromPalette = false;
            
            // Проверяем родительские объекты
            Transform current = transform.parent;
            while (current != null)
            {
                if (current.GetComponent<BlockPalette>() != null)
                {
                    isFromPalette = true;
                    break;
                }
                current = current.parent;
            }
            
            // Дополнительная проверка по тегу
            if (!isFromPalette && transform.parent != null)
            {
                isFromPalette = transform.parent.CompareTag("BlockPalette") || 
                               gameObject.CompareTag("BlockPalette");
            }
            
            Debug.Log($"Определен источник блока: isFromPalette={isFromPalette}, parent={transform.parent?.name}");
        }

        private void SetupVisualFeedback()
        {
            if (visualFeedback == null)
            {
                visualFeedback = GetComponent<DragVisualFeedback>();
                if (visualFeedback == null)
                {
                    visualFeedback = gameObject.AddComponent<DragVisualFeedback>();
                    Debug.Log("✓ Создан DragVisualFeedback компонент");
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isDragging) return; // Предотвращаем множественные вызовы
            
            isDragging = true;
            StoreOriginalState();
            
            Debug.Log($"Начало перетаскивания: isFromPalette={isFromPalette}, Canvas={canvas?.name}");
            
            if (isFromPalette)
            {
                // Для блоков из палитры создаем визуальную копию
                CreateDragPreview();
                SetupDragPreview(eventData);
                Debug.Log($"Создан dragPreview: {dragPreview?.name}");
            }
            else
            {
                // Для блоков из рабочей области перетаскиваем оригинал
                SetupOriginalDrag(eventData);
            }

            ApplyDragVisuals();
            NotifyDragStart();
        }

        private void StoreOriginalState()
        {
            originalParent = transform.parent;
            siblingIndex = transform.GetSiblingIndex();
            originalScale = transform.localScale;
            originalPosition = transform.position;
        }

        private void CreateDragPreview()
        {
            // Создаем визуальную копию для перетаскивания
            dragPreview = Instantiate(gameObject, canvas.transform);
            dragPreview.name = "DragPreview_" + gameObject.name;
            
            // Удаляем ненужные компоненты с копии
            var previewHandler = dragPreview.GetComponent<DragDropHandler>();
            if (previewHandler != null)
            {
                DestroyImmediate(previewHandler);
            }
            
            // Настраиваем копию
            var previewRect = dragPreview.GetComponent<RectTransform>();
            var previewCanvasGroup = dragPreview.GetComponent<CanvasGroup>();
            
            if (previewCanvasGroup != null)
            {
                previewCanvasGroup.alpha = dragAlpha;
                previewCanvasGroup.blocksRaycasts = false;
            }
            
            // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Правильное позиционирование
            previewRect.anchorMin = new Vector2(0.5f, 0.5f);
            previewRect.anchorMax = new Vector2(0.5f, 0.5f);
            previewRect.pivot = new Vector2(0.5f, 0.5f);
            previewRect.localScale = originalScale * dragScale;
            previewRect.sizeDelta = dragRectTransform.sizeDelta;
            
            // Устанавливаем позицию относительно Canvas
            previewRect.anchoredPosition = Vector2.zero;
        }

        private void SetupDragPreview(PointerEventData eventData)
        {
            if (dragPreview == null) return;
            
            // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Правильное позиционирование относительно курсора
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    canvas.worldCamera,
                    out localPoint))
            {
                var previewRect = dragPreview.GetComponent<RectTransform>();
                previewRect.anchoredPosition = localPoint;
            }
        }

        private void SetupOriginalDrag(PointerEventData eventData)
        {
            if (canvas != null)
            {
                transform.SetParent(canvas.transform, false);
                transform.SetAsLastSibling();

                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvas.transform as RectTransform,
                        eventData.position,
                        canvas.worldCamera,
                        out localPoint))
                {
                    dragRectTransform.anchoredPosition = localPoint;
                }
            }
        }

        private void ApplyDragVisuals()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = dragAlpha;
                canvasGroup.blocksRaycasts = false;
            }

            if (!isFromPalette)
            {
                transform.localScale = originalScale * dragScale;
            }
        }

        private void NotifyDragStart()
        {
            if (commandBlock != null)
            {
                commandBlock.StartDrag();
            }

            if (visualFeedback != null)
            {
                visualFeedback.ShowDragStart();
            }

            AudioManager.Instance?.PlaySound("drag_start");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            if (isFromPalette && dragPreview != null)
            {
                UpdateDragPreviewPosition(eventData);
            }
            else if (!isFromPalette)
            {
                UpdateOriginalPosition(eventData);
            }

            UpdateVisualFeedback(eventData);
        }

        private void UpdateDragPreviewPosition(PointerEventData eventData)
        {
            if (dragPreview == null || canvas == null) return;
            
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    canvas.worldCamera,
                    out localPoint))
            {
                dragPreview.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
        }

        private void UpdateOriginalPosition(PointerEventData eventData)
        {
            if (dragRectTransform == null || canvas == null) return;

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    canvas.worldCamera,
                    out localPoint))
            {
                dragRectTransform.anchoredPosition = localPoint;
            }
        }

        private void UpdateVisualFeedback(PointerEventData eventData)
        {
            DropZone dropZone = GetDropZoneUnderPointer(eventData);
            if (visualFeedback != null)
            {
                if (dropZone != null)
                {
                    visualFeedback.ShowValidDropZone();
                }
                else
                {
                    visualFeedback.ShowInvalidDropZone();
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            isDragging = false;
            
            DropZone dropZone = GetDropZoneUnderPointer(eventData);
            Debug.Log($"Конец перетаскивания: dropZone={dropZone?.name}, isFromPalette={isFromPalette}");
            
            if (dropZone != null)
            {
                HandleSuccessfulDrop(dropZone);
            }
            else
            {
                HandleFailedDrop();
            }

            CleanupDragState();
            NotifyDragEnd();
        }

        private void HandleSuccessfulDrop(DropZone dropZone)
        {
            if (isFromPalette)
            {
                // Создаем новый блок в рабочей области на основе блока из палитры
                CreateBlockInWorkspace(dropZone);
            }
            else
            {
                // Перемещаем существующий блок
                MoveBlockToDropZone(dropZone);
            }

            if (visualFeedback != null)
            {
                visualFeedback.ShowSuccessFeedback();
            }

            AudioManager.Instance?.PlaySound("drop_success");
        }

        private void CreateBlockInWorkspace(DropZone dropZone)
        {
            // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Создаем копию блока для рабочей области
            GameObject newBlockObj = Instantiate(gameObject, dropZone.transform);
            CommandBlock newBlock = newBlockObj.GetComponent<CommandBlock>();
            
            if (newBlock != null)
            {
                // Настраиваем новый блок для рабочей области
                SetupBlockForWorkspace(newBlockObj);
                newBlock.SetInWorkspace(true, dropZone.BlockCount);
                
                // Уведомляем DropZone о новом блоке
                dropZone.OnBlockDropped?.Invoke(newBlock, dropZone.slotIndex);
                
                Debug.Log($"Блок {newBlock.commandType} успешно создан в рабочей области");
            }
            else
            {
                Debug.LogError("Не удалось получить CommandBlock из созданного объекта");
            }
        }

        private void MoveBlockToDropZone(DropZone dropZone)
        {
            // Перемещаем существующий блок
            transform.SetParent(dropZone.transform);
            SetupBlockForWorkspace(gameObject);
            
            if (commandBlock != null)
            {
                commandBlock.SetInWorkspace(true, dropZone.BlockCount);
                dropZone.OnBlockDropped?.Invoke(commandBlock, dropZone.slotIndex);
            }
        }

        private void SetupBlockForWorkspace(GameObject blockObj)
        {
            RectTransform rectTransform = blockObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;
                rectTransform.sizeDelta = new Vector2(120, 60);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
            }
        }

        private void HandleFailedDrop()
        {
            if (isFromPalette)
            {
                // Для блоков из палитры просто возвращаем в исходное состояние
                RestoreOriginalState();
            }
            else
            {
                // Для блоков из рабочей области возвращаем в исходное место
                RestoreOriginalState();
            }

            AudioManager.Instance?.PlaySound("drop_fail");
        }

        private void RestoreOriginalState()
        {
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(siblingIndex);
            transform.localScale = originalScale;
            transform.position = originalPosition;
        }

        private void CleanupDragState()
        {
            // Очищаем визуальную копию
            if (dragPreview != null)
            {
                Destroy(dragPreview);
                dragPreview = null;
            }

            // Восстанавливаем визуальное состояние
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            if (!isFromPalette)
            {
                transform.localScale = originalScale;
            }
        }

        private void NotifyDragEnd()
        {
            if (commandBlock != null)
            {
                commandBlock.EndDrag();
            }

            if (visualFeedback != null)
            {
                visualFeedback.HideFeedback();
            }
        }


        private DropZone GetDropZoneUnderPointer(PointerEventData eventData)
        {
            // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Улучшенное обнаружение DropZone
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Сначала проверяем прямое попадание
            foreach (var result in results)
            {
                DropZone dropZone = result.gameObject.GetComponent<DropZone>();
                if (dropZone != null) return dropZone;
            }

            // Затем проверяем родительские объекты
            foreach (var result in results)
            {
                DropZone dropZone = result.gameObject.GetComponentInParent<DropZone>();
                if (dropZone != null) return dropZone;
            }

            return null;
        }
    }
}