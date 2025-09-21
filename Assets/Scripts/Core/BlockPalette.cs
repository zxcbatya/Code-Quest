using System.Collections.Generic;
using RobotCoder.Core;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class BlockPalette : MonoBehaviour
    {
        [Header("Palette Settings")]
        [SerializeField] private Transform blockContainer;
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private float blockSpacing = 10f;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TextMeshProUGUI paletteTitle;
        
        [Header("Visual Settings")]
        [SerializeField] private Color paletteBackgroundColor = new Color(0.2f, 0.2f, 0.3f, 0.9f);
        
        [Header("Available Commands")]
        [SerializeField] private bool allowMoveForward = true;
        [SerializeField] private bool allowTurnLeft = true;
        [SerializeField] private bool allowTurnRight = true;
        [SerializeField] private bool allowJump = false;
        [SerializeField] private bool allowInteract = false;
        [SerializeField] private bool allowRepeat = false;
        [SerializeField] private bool allowIf = false;
        
        private List<GameObject> spawnedBlocks = new List<GameObject>();

        private void Start()
        {
            InitializePalette();
            CreatePalette();
        }
        
        private void InitializePalette()
        {
            if (paletteTitle != null)
            {
                string titleText = LocalizationManager.Instance?.GetText("COMMAND_PALETTE") ?? "Команды";
                paletteTitle.text = titleText;
            }
            
            // Настраиваем контейнер блоков
            if (blockContainer != null)
            {
                VerticalLayoutGroup layoutGroup = blockContainer.GetComponent<VerticalLayoutGroup>();
                if (layoutGroup == null)
                {
                    layoutGroup = blockContainer.gameObject.AddComponent<VerticalLayoutGroup>();
                }
                
                layoutGroup.spacing = blockSpacing;
                layoutGroup.childAlignment = TextAnchor.UpperCenter;
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = false;
                layoutGroup.childForceExpandWidth = true;
                layoutGroup.childForceExpandHeight = false;
                
                // Добавляем Content Size Fitter
                ContentSizeFitter sizeFitter = blockContainer.GetComponent<ContentSizeFitter>();
                if (sizeFitter == null)
                {
                    sizeFitter = blockContainer.gameObject.AddComponent<ContentSizeFitter>();
                }
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        public void SetAvailableCommands(LevelData levelData)
        {
            allowMoveForward = levelData.allowMoveForward;
            allowTurnLeft = levelData.allowTurnLeft;
            allowTurnRight = levelData.allowTurnRight;
            allowJump = levelData.allowJump;
            allowInteract = levelData.allowInteract;
            allowRepeat = levelData.allowRepeat;
            allowIf = levelData.allowIf;
            
            RefreshPalette();
        }

        private void CreatePalette()
        {
            ClearPalette();
            
            if (allowMoveForward)
                CreateBlock(CommandType.MoveForward);
                
            if (allowTurnLeft)
                CreateBlock(CommandType.TurnLeft);
                
            if (allowTurnRight)
                CreateBlock(CommandType.TurnRight);
                
            if (allowJump)
                CreateBlock(CommandType.Jump);
                
            if (allowInteract)
                CreateBlock(CommandType.Interact);
                
            if (allowRepeat)
                CreateBlock(CommandType.Repeat);
                
            if (allowIf)
                CreateBlock(CommandType.If);
        }

        private void CreateBlock(CommandType commandType)
        {
            GameObject blockObj = Instantiate(blockPrefab, blockContainer);
            
            // Добавляем соответствующий компонент команды
            CommandBlock commandBlock = AddCommandComponent(blockObj, commandType);
            
            // Настраиваем drag & drop
            DragDropHandler dragHandler = blockObj.GetComponent<DragDropHandler>();
            if (dragHandler == null)
            {
                dragHandler = blockObj.AddComponent<DragDropHandler>();
            }
            dragHandler.SetFromPalette(true);
            
            spawnedBlocks.Add(blockObj);
        }

        private CommandBlock AddCommandComponent(GameObject blockObj, CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.MoveForward:
                    return blockObj.AddComponent<MoveForwardCommand>();
                case CommandType.TurnLeft:
                    return blockObj.AddComponent<TurnLeftCommand>();
                case CommandType.TurnRight:
                    return blockObj.AddComponent<TurnRightCommand>();
                case CommandType.Jump:
                    return blockObj.AddComponent<JumpCommand>();
                case CommandType.Interact:
                    return blockObj.AddComponent<InteractCommand>();
                case CommandType.Repeat:
                    return blockObj.AddComponent<RepeatCommand>();
                case CommandType.If:
                    return blockObj.AddComponent<IfCommand>();
            }
            
            return null;
        }

        private void RefreshPalette()
        {
            CreatePalette();
        }

        private void ClearPalette()
        {
            foreach (GameObject block in spawnedBlocks)
            {
                if (block != null)
                {
                    DestroyImmediate(block);
                }
            }
            spawnedBlocks.Clear();
        }

        public void AnimateBlockCreation()
        {
            // Анимация появления блоков
            StartCoroutine(AnimateBlocks());
        }

        private System.Collections.IEnumerator AnimateBlocks()
        {
            for (int i = 0; i < spawnedBlocks.Count; i++)
            {
                if (spawnedBlocks[i] != null)
                {
                    spawnedBlocks[i].transform.localScale = Vector3.zero;
                    StartCoroutine(ScaleIn(spawnedBlocks[i].transform, i * 0.1f));
                }
            }
            yield return null;
        }

        private System.Collections.IEnumerator ScaleIn(Transform target, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            Vector3 targetScale = Vector3.one;
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                target.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
                yield return null;
            }
            
            target.localScale = targetScale;
        }
    }
}