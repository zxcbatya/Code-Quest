using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class GenericCommandBlock : CommandBlock
    {
        public override bool Execute(RobotController robot)
        {
            if (robot == null) return false;
            
            switch (commandType)
            {
                case CommandType.MoveForward:
                    return robot.MoveForward();
                case CommandType.TurnLeft:
                    return robot.TurnLeft();
                case CommandType.TurnRight:
                    return robot.TurnRight();
                case CommandType.Jump:
                    return robot.Jump();
                case CommandType.Interact:
                    return robot.Interact();
                default:
                    return true;
            }
        }

        public override void InitializeBlock()
        {
            SetBlockProperties();
            base.InitializeBlock();
        }

        private void SetBlockProperties()
        {
            switch (commandType)
            {
                case CommandType.MoveForward:
                    blockColor = new Color(0.2f, 0.7f, 0.2f);
                    break;
                case CommandType.TurnLeft:
                    blockColor = new Color(0.2f, 0.2f, 0.7f);
                    break;
                case CommandType.TurnRight:
                    blockColor = new Color(0.7f, 0.2f, 0.2f);
                    break;
                case CommandType.Jump:
                    blockColor = new Color(0.7f, 0.5f, 0.2f);
                    break;
                case CommandType.Interact:
                    blockColor = new Color(0.7f, 0.2f, 0.7f);
                    break;
                case CommandType.Repeat:
                    blockColor = new Color(0.8f, 0.4f, 0.2f);
                    break;
                case CommandType.If:
                    blockColor = new Color(0.6f, 0.2f, 0.8f);
                    break;
                case CommandType.Else:
                    blockColor = new Color(0.4f, 0.4f, 0.4f);
                    break;
            }
        }
    }
}