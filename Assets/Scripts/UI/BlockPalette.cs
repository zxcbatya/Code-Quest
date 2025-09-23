using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Core;

namespace UI
{
    public class BlockPalette : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform paletteContent;
        [SerializeField] private GameObject blockButtonPrefab;
        
        [Header("Level Data")]
        [SerializeField] private LevelData currentLevelData;
        
        private List<GameObject> blockButtons = new List<GameObject>();
        
        private void Start()
        {
            LoadLevelData();
            GenerateBlockPalette();
        }
        
        private void LoadLevelData()
        {
            // Загружаем данные текущего уровня
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName.StartsWith("Level_"))
            {
                string levelNumberStr = sceneName.Substring(6);
                if (int.TryParse(levelNumberStr, out int level))
                {
                    currentLevelData = Resources.Load<LevelData>($"Levels/Level_{level:D2}");
                }
            }
            
            // Если данные уровня не найдены, используем стандартные
            if (currentLevelData == null)
            {
                currentLevelData = ScriptableObject.CreateInstance<LevelData>();
            }
        }
        
        private void GenerateBlockPalette()
        {
            ClearPalette();
            
            // Создаем кнопки для доступных блоков
            if (currentLevelData.allowMoveForward)
                CreateBlockButton(CommandType.MoveForward);
                
            if (currentLevelData.allowTurnLeft)
                CreateBlockButton(CommandType.TurnLeft);
                
            if (currentLevelData.allowTurnRight)
                CreateBlockButton(CommandType.TurnRight);
                
            if (currentLevelData.allowJump)
                CreateBlockButton(CommandType.Jump);
                
            if (currentLevelData.allowInteract)
                CreateBlockButton(CommandType.Interact);
                
            if (currentLevelData.allowRepeat)
                CreateBlockButton(CommandType.Repeat);
                
            if (currentLevelData.allowIf)
                CreateBlockButton(CommandType.If);
                
            if (currentLevelData.allowElse)
                CreateBlockButton(CommandType.Else);
        }
        
        private void CreateBlockButton(CommandType commandType)
        {
            if (blockButtonPrefab == null) return;
            
            GameObject buttonObj = Instantiate(blockButtonPrefab, paletteContent);
            CommandBlock commandBlock = buttonObj.GetComponent<CommandBlock>();
            
            if (commandBlock != null)
            {
                commandBlock.commandType = commandType;
                commandBlock.InitializeBlock(); // Переинициализируем с новым типом
                
                // Настройка визуального представления кнопки
                SetupBlockButtonVisual(buttonObj, commandBlock);
            }
            
            blockButtons.Add(buttonObj);
        }
        
        private void SetupBlockButtonVisual(GameObject buttonObj, CommandBlock commandBlock)
        {
            // Настройка цвета кнопки
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = commandBlock.blockColor;
            }
            
            // Настройка текста
            TMPro.TextMeshProUGUI textComponent = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = commandBlock.GetLocalizedCommandName();
            }
        }
        
        private void ClearPalette()
        {
            foreach (GameObject button in blockButtons)
            {
                if (button != null)
                {
                    Destroy(button);
                }
            }
            blockButtons.Clear();
        }
        
        public List<CommandType> GetAvailableCommands()
        {
            List<CommandType> availableCommands = new List<CommandType>();
            
            if (currentLevelData.allowMoveForward)
                availableCommands.Add(CommandType.MoveForward);
                
            if (currentLevelData.allowTurnLeft)
                availableCommands.Add(CommandType.TurnLeft);
                
            if (currentLevelData.allowTurnRight)
                availableCommands.Add(CommandType.TurnRight);
                
            if (currentLevelData.allowJump)
                availableCommands.Add(CommandType.Jump);
                
            if (currentLevelData.allowInteract)
                availableCommands.Add(CommandType.Interact);
                
            if (currentLevelData.allowRepeat)
                availableCommands.Add(CommandType.Repeat);
                
            if (currentLevelData.allowIf)
                availableCommands.Add(CommandType.If);
                
            if (currentLevelData.allowElse)
                availableCommands.Add(CommandType.Else);
                
            return availableCommands;
        }
        
        public void RefreshPalette()
        {
            GenerateBlockPalette();
        }
    }
}