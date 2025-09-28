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
        [SerializeField] private bool allowJump = true;
        [SerializeField] private bool allowInteract = true;
        [SerializeField] private bool allowRepeat = true;
        [SerializeField] private bool allowIf = true;

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
            if (levelData == null) return;
            
            allowMoveForward = levelData.allowMoveForward;
            allowTurnLeft = levelData.allowTurnLeft;
            allowTurnRight = levelData.allowTurnRight;
            allowJump = levelData.allowJump;
            allowInteract = levelData.allowInteract;
            allowRepeat = levelData.allowRepeat;
            allowIf = levelData.allowIf;

            // Recreate palette with updated settings
            CreatePalette();
        }

        private void CreatePalette()
        {
            // Create blocks that don't already exist in the palette
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

            _templateBlocks[commandType] = blockObj;
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
    }
}