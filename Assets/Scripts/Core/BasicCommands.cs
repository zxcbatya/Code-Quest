using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class MoveForwardCommand : CommandBlock
    {
        public override void InitializeBlock()
        {
            commandType = CommandType.MoveForward;
            commandName = "Вперед";
            blockColor = new Color(0.2f, 0.7f, 0.2f); // Зеленый
            base.InitializeBlock();
        }

        public override bool Execute(RobotController robot)
        {
            HighlightExecution();
            return robot.MoveForward();
        }
    }

    public class TurnLeftCommand : CommandBlock
    {
        public override void InitializeBlock()
        {
            commandType = CommandType.TurnLeft;
            commandName = "Налево";
            blockColor = new Color(0.2f, 0.2f, 0.7f); // Синий
            base.InitializeBlock();
        }

        public override bool Execute(RobotController robot)
        {
            HighlightExecution();
            return robot.TurnLeft();
        }
    }

    public class TurnRightCommand : CommandBlock
    {
        public override void InitializeBlock()
        {
            commandType = CommandType.TurnRight;
            commandName = "Направо";
            blockColor = new Color(0.7f, 0.2f, 0.2f); // Красный
            base.InitializeBlock();
        }

        public override bool Execute(RobotController robot)
        {
            HighlightExecution();
            return robot.TurnRight();
        }
    }

    public class JumpCommand : CommandBlock
    {
        public override void InitializeBlock()
        {
            commandType = CommandType.Jump;
            commandName = "Прыжок";
            blockColor = new Color(0.7f, 0.5f, 0.2f); // Оранжевый
            base.InitializeBlock();
        }

        public override bool Execute(RobotController robot)
        {
            HighlightExecution();
            return robot.Jump();
        }
    }

    public class InteractCommand : CommandBlock
    {
        public override void InitializeBlock()
        {
            commandType = CommandType.Interact;
            commandName = "Действие";
            blockColor = new Color(0.7f, 0.2f, 0.7f); // Фиолетовый
            base.InitializeBlock();
        }

        public override bool Execute(RobotController robot)
        {
            HighlightExecution();
            return robot.Interact();
        }
    }
}
