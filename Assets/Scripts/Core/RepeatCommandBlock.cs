using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class RepeatCommandBlock : CommandBlock
    {
        [Header("Repeat Settings")]
        [SerializeField] private int repeatCount = 3;
        
        public override bool Execute(RobotController robot)
        {
            // Repeat commands don't execute directly
            // They are handled by the program interpreter
            Debug.Log($"Повторить {repeatCount} раз");
            return true;
        }
        
        public int GetRepeatCount()
        {
            return repeatCount;
        }
        
        public void SetRepeatCount(int count)
        {
            repeatCount = Mathf.Max(1, count);
        }
    }
}