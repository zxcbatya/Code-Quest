using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class ElseCommandBlock : CommandBlock
    {
        public override bool Execute(RobotController robot)
        {
            // Else commands don't execute directly
            // They are handled by the program interpreter
            Debug.Log("Иначе");
            return true;
        }
    }
}