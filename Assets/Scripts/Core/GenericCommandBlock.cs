using UnityEngine;
using Core;
using RobotCoder.Core;

namespace Core
{
    /// <summary>
    /// Универсальный блок команды для префабов.
    /// Автоматически настраивается через BlockPalette в зависимости от CommandType.
    /// </summary>
    public class GenericCommandBlock : CommandBlock
    {
        public override bool Execute(RobotController robot)
        {
            // Заглушка - реальная логика будет в конкретных командах
            Debug.Log($"Executing {commandType} command");
            return true;
        }

        public override void InitializeBlock()
        {
            // Настраиваем базовые параметры в зависимости от типа команды
            ConfigureByType();
            base.InitializeBlock();
        }

        private void ConfigureByType()
        {
            switch (commandType)
            {
                case CommandType.MoveForward:
                    commandName = "Вперед";
                    blockColor = new Color(0.2f, 0.7f, 0.2f); // Зеленый
                    break;
                case CommandType.TurnLeft:
                    commandName = "Налево";
                    blockColor = new Color(0.2f, 0.2f, 0.7f); // Синий
                    break;
                case CommandType.TurnRight:
                    commandName = "Направо";
                    blockColor = new Color(0.7f, 0.2f, 0.2f); // Красный
                    break;
                case CommandType.Jump:
                    commandName = "Прыжок";
                    blockColor = new Color(0.7f, 0.5f, 0.2f); // Оранжевый
                    break;
                case CommandType.Interact:
                    commandName = "Действие";
                    blockColor = new Color(0.7f, 0.2f, 0.7f); // Фиолетовый
                    break;
                case CommandType.Repeat:
                    commandName = "Повтор";
                    blockColor = new Color(0.8f, 0.4f, 0.2f); // Оранжевый
                    break;
                case CommandType.If:
                    commandName = "Если";
                    blockColor = new Color(0.6f, 0.2f, 0.8f); // Фиолетовый
                    break;
                case CommandType.Else:
                    commandName = "Иначе";
                    blockColor = new Color(0.4f, 0.4f, 0.4f); // Серый
                    break;
                default:
                    commandName = "Команда";
                    blockColor = Color.blue;
                    break;
            }
        }
    }
}
