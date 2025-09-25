using System.Collections.Generic;
using RobotCoder.Core;
using TMPro;
using RobotCoder.UI;
using UnityEngine;
using UnityEngine.UI;

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
        
        private List<GameObject> spawnedBlocks = new List<GameObject>();

        private void Start()
        {
            InitializePalette();
            CreatePalette();
        }
        
        private void InitializePalette()
        {
            string titleText = LocalizationManager.Instance?.GetText("COMMAND_PALETTE") ?? "Команды";
            paletteTitle.text = titleText;
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
            
            if (allowMoveForward) CreateBlock(CommandType.MoveForward);
            if (allowTurnLeft) CreateBlock(CommandType.TurnLeft);
            if (allowTurnRight) CreateBlock(CommandType.TurnRight);
            if (allowJump) CreateBlock(CommandType.Jump);
            if (allowInteract) CreateBlock(CommandType.Interact);
            if (allowRepeat) CreateBlock(CommandType.Repeat);
            if (allowIf) CreateBlock(CommandType.If);
        }

        private void CreateBlock(CommandType commandType)
        {
            GameObject blockObj = Instantiate(blockPrefab, blockContainer);
            blockObj.tag = "BlockPalette";
            
            CommandBlock commandBlock = AddCommandComponent(blockObj, commandType);
            commandBlock.InitializeBlock();
            
            spawnedBlocks.Add(blockObj);
        }
        
        private CommandBlock AddCommandComponent(GameObject blockObj, CommandType commandType)
        {
            var commandBlock = blockObj.GetComponent<CommandBlock>();
            if (commandBlock == null)
            {
                commandBlock = blockObj.AddComponent<GenericCommandBlock>();
            }
            
            commandBlock.commandType = commandType;
            return commandBlock;
        }

        private void RefreshPalette()
        {
            CreatePalette();
        }

        private void ClearPalette()
        {
            foreach (var block in spawnedBlocks)
            {
                if (block != null)
                {
                    Destroy(block);
                }
            }
            spawnedBlocks.Clear();
        }
    }
}