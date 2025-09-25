using UnityEngine;
using UnityEngine.EventSystems;
using Core;
using UnityEngine.UI;

namespace RobotCoder.UI
{
    public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Color highlightColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        [SerializeField] private Color normalColor = new Color(0f, 0.8f, 1f, 1f);
        [SerializeField] private Image backgroundImage;
        
        public int slotIndex = -1;
        public System.Action<CommandBlock, int> OnBlockDropped;

        private void Awake()
        {
            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            backgroundImage.color = normalColor;
        }
        
        public void AcceptBlock(CommandBlock block)
        {
            if (block == null) return;
            
            block.transform.SetParent(transform);
            SetupBlockForWorkspace(block.gameObject);
            block.SetInWorkspace(true, GetNextExecutionOrder());
            OnBlockDropped?.Invoke(block, slotIndex);
        }
        
        private int GetNextExecutionOrder()
        {
            return BlockCount;
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
                    child.SetParent(null);
                }
            }
        }

        public void RemoveBlock(CommandBlock block)
        {
            if (block != null && block.transform.parent == transform)
            {
                block.transform.SetParent(null);
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
            if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<CommandBlock>() != null)
            {
                backgroundImage.color = highlightColor;
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            backgroundImage.color = normalColor;
        }
    }
}