using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class GenericCommandBlock : CommandBlock
    {
        public override bool Execute(RobotController robot)
        {
            return true;
        }

        public override void InitializeBlock()
        {
            ConfigureByType();
            base.InitializeBlock();
        }

        private void ConfigureByType()
        {
            switch (commandType)
            {
                case CommandType.MoveForward:
                    commandName = "Вперед";
                    blockColor = new Color(0.2f, 0.7f, 0.2f);
                    break;
                case CommandType.TurnLeft:
                    commandName = "Налево";
                    blockColor = new Color(0.2f, 0.2f, 0.7f);
                    break;
                case CommandType.TurnRight:
                    commandName = "Направо";
                    blockColor = new Color(0.7f, 0.2f, 0.2f);
                    break;
                case CommandType.Jump:
                    commandName = "Прыжок";
                    blockColor = new Color(0.7f, 0.5f, 0.2f);
                    break;
                case CommandType.Interact:
                    commandName = "Действие";
                    blockColor = new Color(0.7f, 0.2f, 0.7f);
                    break;
                case CommandType.Repeat:
                    commandName = "Повтор";
                    blockColor = new Color(0.8f, 0.4f, 0.2f);
                    break;
                case CommandType.If:
                    commandName = "Если";
                    blockColor = new Color(0.6f, 0.2f, 0.8f);
                    break;
                case CommandType.Else:
                    commandName = "Иначе";
                    blockColor = new Color(0.4f, 0.4f, 0.4f);
                    break;
                default:
                    commandName = "Команда";
                    blockColor = Color.blue;
                    break;
            }
        }
    }
}