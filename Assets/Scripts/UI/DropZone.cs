using UnityEngine;
using UnityEngine.EventSystems;
using Core;
using UnityEngine.UI;

namespace RobotCoder.UI
{
    public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Visual Feedback")]
        [SerializeField] private Color highlightColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        [SerializeField] private Color normalColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        [SerializeField] private float highlightIntensity = 1.5f;
        
        private Image backgroundImage;
        public int slotIndex = -1;
        
        // Событие сброса блока
        public System.Action<CommandBlock, int> OnBlockDropped;
        
        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            if (backgroundImage == null)
            {
                // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Создаем Image если его нет
                backgroundImage = gameObject.AddComponent<Image>();
                backgroundImage.color = new Color(0.8f, 0.8f, 0.8f, 0.3f); // Полупрозрачный серый
                Debug.Log("✓ Создан Image компонент для DropZone");
            }
            
            if (backgroundImage != null)
            {
                normalColor = backgroundImage.color;
                // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: Убеждаемся, что DropZone может получать raycast
                backgroundImage.raycastTarget = true;
                Debug.Log("✓ DropZone Image настроен");
            }
            
            // Убеждаемся, что у DropZone есть все необходимые компоненты
            if (GetComponent<RectTransform>() == null)
            {
                Debug.LogError("DropZone должен иметь RectTransform!");
            }
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            // КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: DropZone больше не обрабатывает OnDrop
            // Вся логика теперь в DragDropHandler
            Debug.Log("OnDrop вызван, но логика обрабатывается в DragDropHandler");
            
            // Восстанавливаем нормальный цвет
            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
        }
        
        public void ReceiveBlock(CommandBlock commandBlock)
        {
            if (commandBlock == null) return;
            
            // Создаем копию блока
            GameObject blockCopy = Instantiate(commandBlock.gameObject, transform);
            CommandBlock blockToPlace = blockCopy.GetComponent<CommandBlock>();
            
            // Настраиваем копию для рабочей области
            SetupBlockForWorkspace(blockToPlace.gameObject);
            
            // Устанавливаем блок как находящийся в рабочей области
            blockToPlace.SetInWorkspace(true, GetNextExecutionOrder());
            
            // Вызываем событие
            OnBlockDropped?.Invoke(blockToPlace, slotIndex);
        }
        
        public void AcceptBlock(CommandBlock commandBlock)
        {
            if (commandBlock == null) return;
            
            // Перемещаем существующий блок
            commandBlock.transform.SetParent(transform);
            SetupBlockForWorkspace(commandBlock.gameObject);
            
            // Устанавливаем блок как находящийся в рабочей области
            commandBlock.SetInWorkspace(true, GetNextExecutionOrder());
            
            // Вызываем событие
            OnBlockDropped?.Invoke(commandBlock, slotIndex);
        }
        
        private int GetNextExecutionOrder()
        {
            return BlockCount; // Следующий порядок выполнения
        }
        
        private void SetupBlockForWorkspace(GameObject blockObj)
        {
            // Настраиваем RectTransform для блока в рабочей области
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
            
            // Убеждаемся, что у блока есть все необходимые компоненты
            SetupBlockComponents(blockObj);
        }
        
        private void SetupBlockComponents(GameObject blockObj)
        {
            // Убеждаемся, что у блока есть все необходимые компоненты
            if (blockObj.GetComponent<DragDropHandler>() == null)
            {
                blockObj.AddComponent<DragDropHandler>();
            }
            
            if (blockObj.GetComponent<CanvasGroup>() == null)
            {
                blockObj.AddComponent<CanvasGroup>();
            }
            
            if (blockObj.GetComponent<Button>() == null)
            {
                blockObj.AddComponent<Button>();
            }
            
            // Настраиваем RectTransform для блока в рабочей области
            RectTransform rectTransform = blockObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(120, 60);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.localScale = Vector3.one;
            }
        }

        // --- API expected by WorkspacePanel ---
        // Упразднён внутренний список. Истина состояния — дочерние объекты с компонентом CommandBlock.

        public int BlockCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<CommandBlock>() != null) count++;
                }
                return count;
            }
        }

        public System.Collections.Generic.IReadOnlyList<CommandBlock> Blocks
        {
            get
            {
                var list = new System.Collections.Generic.List<CommandBlock>(transform.childCount);
                for (int i = 0; i < transform.childCount; i++)
                {
                    var block = transform.GetChild(i).GetComponent<CommandBlock>();
                    if (block != null) list.Add(block);
                }
                return list;
            }
        }

        public void ClearAllBlocks()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child.GetComponent<CommandBlock>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void RemoveBlock(CommandBlock block)
        {
            if (block == null) return;
            if (block.transform.parent == transform)
            {
                Destroy(block.gameObject);
            }
        }


        public CommandBlock[] GetOrderedBlocks()
        {
            var list = new System.Collections.Generic.List<CommandBlock>(transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                var block = transform.GetChild(i).GetComponent<CommandBlock>();
                if (block != null) list.Add(block);
            }
            return list.ToArray();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Проверяем, есть ли перетаскиваемый блок
            if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<CommandBlock>() != null)
            {
                // Подсвечиваем зону сброса
                if (backgroundImage != null)
                {
                    backgroundImage.color = highlightColor * highlightIntensity;
                }
                
                // Добавляем легкую анимацию пульсации
                StartCoroutine(PulseAnimation());
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            // Восстанавливаем нормальный цвет
            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
            
            // Останавливаем анимацию пульсации
            StopAllCoroutines();
        }
        
        private System.Collections.IEnumerator PulseAnimation()
        {
            while (true)
            {
                float pulse = Mathf.PingPong(Time.time * 2f, 0.3f) + 0.7f;
                if (backgroundImage != null)
                {
                    backgroundImage.color = highlightColor * pulse;
                }
                yield return null;
            }
        }
    }
}