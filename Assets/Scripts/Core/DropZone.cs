
using System.Collections.Generic;
using System.Linq;
using RobotCoder.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class DropZone : MonoBehaviour
    {
        [Header("Drop Zone Settings")]
        [SerializeField] private DropZoneType zoneType;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.green;
        [SerializeField] private Color invalidColor = Color.red;
        
        [Header("Layout Settings")]
        [SerializeField] private bool useVerticalLayout = true;
        [SerializeField] private float spacing = 10f;
        [SerializeField] private int maxBlocks = 20;
        
        private Image backgroundImage;
        private List<CommandBlock> blocks = new List<CommandBlock>();
        private VerticalLayoutGroup verticalLayout;
        private HorizontalLayoutGroup horizontalLayout;
        
        public enum DropZoneType
        {
            Workspace,      // Основная рабочая область
            Trash,          // Корзина для удаления
            Repeat,         // Внутри блока повтора
            IfTrue,         // Внутри блока IF (истина)
            IfFalse         // Внутри блока IF (ложь)
        }
        
        public List<CommandBlock> Blocks => blocks.ToList();
        public int BlockCount => blocks.Count;
        public bool IsFull => blocks.Count >= maxBlocks;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            SetupLayoutGroup();
        }

        private void SetupLayoutGroup()
        {
            if (useVerticalLayout)
            {
                verticalLayout = GetComponent<VerticalLayoutGroup>();
                if (verticalLayout == null)
                {
                    verticalLayout = gameObject.AddComponent<VerticalLayoutGroup>();
                }
                
                verticalLayout.spacing = spacing;
                verticalLayout.childAlignment = TextAnchor.UpperCenter;
                verticalLayout.childControlWidth = true;
                verticalLayout.childControlHeight = false;
                verticalLayout.childForceExpandWidth = true;
                verticalLayout.childForceExpandHeight = false;
            }
            else
            {
                horizontalLayout = GetComponent<HorizontalLayoutGroup>();
                if (horizontalLayout == null)
                {
                    horizontalLayout = gameObject.AddComponent<HorizontalLayoutGroup>();
                }
                
                horizontalLayout.spacing = spacing;
                horizontalLayout.childAlignment = TextAnchor.MiddleCenter;
                horizontalLayout.childControlWidth = false;
                horizontalLayout.childControlHeight = true;
                horizontalLayout.childForceExpandWidth = false;
                horizontalLayout.childForceExpandHeight = true;
            }
        }

        public bool CanAcceptBlock(CommandBlock block)
        {
            if (block == null) return false;
            if (IsFull) return false;
            
            switch (zoneType)
            {
                case DropZoneType.Workspace:
                    return true; 
                    
                case DropZoneType.Trash:
                    
                case DropZoneType.Repeat:
                case DropZoneType.IfTrue:
                case DropZoneType.IfFalse:
                    return block.commandType != CommandType.Repeat && 
                           block.commandType != CommandType.If;
                           
                default:
                    return true;
            }
        }

        public void AcceptBlock(CommandBlock block)
        {
            if (!CanAcceptBlock(block)) return;
            
            if (block.IsInWorkspace)
            {
                RemoveBlockFromPreviousZone(block);
            }
            
            blocks.Add(block);
            block.transform.SetParent(transform);
            block.SetInWorkspace(true, blocks.Count - 1);
            
            if (zoneType == DropZoneType.Trash)
            {
                RemoveBlock(block);
                return;
            }
            
            UpdateExecutionOrder();
            
            ProgramInterpreter.Instance?.OnProgramChanged();
        }

        private void RemoveBlockFromPreviousZone(CommandBlock block)
        {
            DropZone[] allZones = FindObjectsOfType<DropZone>();
            
            foreach (var zone in allZones)
            {
                if (zone.blocks.Contains(block))
                {
                    zone.RemoveBlock(block);
                    break;
                }
            }
        }

        public void RemoveBlock(CommandBlock block)
        {
            if (blocks.Contains(block))
            {
                blocks.Remove(block);
                
                if (zoneType == DropZoneType.Trash)
                {
                    // Анимация удаления
                    StartCoroutine(AnimateDestroy(block.gameObject));
                }
                else
                {
                    block.SetInWorkspace(false);
                }
                
                UpdateExecutionOrder();
                ProgramInterpreter.Instance?.OnProgramChanged();
            }
        }

        private System.Collections.IEnumerator AnimateDestroy(GameObject obj)
        {
            Vector3 originalScale = obj.transform.localScale;
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
                yield return null;
            }
            
            Destroy(obj);
        }

        public void SetHighlight(bool highlight)
        {
            if (backgroundImage == null) return;
            
            if (highlight)
            {
                bool canAccept = DragDropHandler.CurrentDragging != null && 
                               CanAcceptBlock(DragDropHandler.CurrentDragging.CommandBlock);
                               
                backgroundImage.color = canAccept ? highlightColor : invalidColor;
            }
            else
            {
                backgroundImage.color = normalColor;
            }
        }

        private void UpdateExecutionOrder()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].SetInWorkspace(true, i);
            }
        }

        public void ClearAllBlocks()
        {
            for (int i = blocks.Count - 1; i >= 0; i--)
            {
                RemoveBlock(blocks[i]);
            }
        }

        public CommandBlock[] GetOrderedBlocks()
        {
            return blocks.OrderBy(block => block.ExecutionOrder).ToArray();
        }
    }
}