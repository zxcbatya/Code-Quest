using System.Collections.Generic;
using TMPro;
using RobotCoder.UI;
using UnityEngine;

namespace Core
{
    public class BlockPalette : MonoBehaviour
    {
        [SerializeField] private Transform blockContainer;
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private float blockSpacing = 10f;
        [SerializeField] private TextMeshProUGUI paletteTitle;
        [SerializeField] private bool allowMoveForward = true;
        [SerializeField] private bool allowTurnLeft = true;
        [SerializeField] private bool allowTurnRight = true;
        [SerializeField] private bool allowJump = false;
        [SerializeField] private bool allowInteract = false;
        [SerializeField] private bool allowRepeat = false;
        [SerializeField] private bool allowIf = false;

        private readonly Dictionary<CommandType, GameObject>
            _templateBlocks = new Dictionary<CommandType, GameObject>();

        private void Start()
        {
            InitializePalette();
            CreatePalette();
        }

        private void InitializePalette()
        {
            string titleText = LocalizationManager.Instance?.GetText("COMMAND_PALETTE");
            if (paletteTitle != null)
            {
                paletteTitle.text = titleText;
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

            // Only create missing blocks, don't refresh existing ones
            CreatePalette();
        }

        private void CreatePalette()
        {
            // Only create blocks that don't already exist in the palette
            // This ensures blocks are never deleted or recreated once they exist
            if (allowMoveForward && !_templateBlocks.ContainsKey(CommandType.MoveForward)) 
                CreateTemplateBlock(CommandType.MoveForward);
            if (allowTurnLeft && !_templateBlocks.ContainsKey(CommandType.TurnLeft)) 
                CreateTemplateBlock(CommandType.TurnLeft);
            if (allowTurnRight && !_templateBlocks.ContainsKey(CommandType.TurnRight)) 
                CreateTemplateBlock(CommandType.TurnRight);
            if (allowJump && !_templateBlocks.ContainsKey(CommandType.Jump)) 
                CreateTemplateBlock(CommandType.Jump);
            if (allowInteract && !_templateBlocks.ContainsKey(CommandType.Interact)) 
                CreateTemplateBlock(CommandType.Interact);
            if (allowRepeat && !_templateBlocks.ContainsKey(CommandType.Repeat)) 
                CreateTemplateBlock(CommandType.Repeat);
            if (allowIf && !_templateBlocks.ContainsKey(CommandType.If)) 
                CreateTemplateBlock(CommandType.If);
        }

        private void CreateTemplateBlock(CommandType commandType)
        {
            if (blockPrefab == null || blockContainer == null) return;
            
            GameObject blockObj = Instantiate(blockPrefab, blockContainer);
            if (blockObj != null)
            {
                blockObj.tag = "BlockPalette";

                CommandBlock commandBlock = AddCommandComponent(blockObj, commandType);
                if (commandBlock != null)
                {
                    try
                    {
                        commandBlock.InitializeBlock();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error initializing command block: {e.Message}");
                    }
                }

                // Не добавляем проверку на null для dragDropHandler, потому что он может отсутствовать
                _templateBlocks[commandType] = blockObj;
            }
        }

        private CommandBlock AddCommandComponent(GameObject blockObj, CommandType commandType)
        {
            if (blockObj == null) return null;
            
            var commandBlock = blockObj.GetComponent<CommandBlock>();
            if (commandBlock == null)
            {
                commandBlock = blockObj.AddComponent<GenericCommandBlock>();
            }

            commandBlock.commandType = commandType;
            return commandBlock;
        }

        public void RefreshPalette()
        {
            CreatePalette();
        }

        public bool HasTemplateBlock(CommandType commandType)
        {
            return _templateBlocks.ContainsKey(commandType) && _templateBlocks[commandType] != null;
        }

        public GameObject GetTemplateBlock(CommandType commandType)
        {
            return _templateBlocks.GetValueOrDefault(commandType);
        }

        // Remove the Update method that was causing unnecessary refreshes
    }
}