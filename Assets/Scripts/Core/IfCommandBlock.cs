using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class IfCommandBlock : CommandBlock
    {
        [Header("Condition Settings")]
        [SerializeField] private ConditionType condition = ConditionType.PathAhead;
        
        public enum ConditionType
        {
            PathAhead,
            WallAhead,
            OnGoal,
            ItemNearby
        }
        
        public override bool Execute(RobotController robot)
        {
            // If commands don't execute directly
            // They are handled by the program interpreter
            Debug.Log($"Если {condition}");
            return true;
        }
        
        public bool CheckCondition(RobotController robot)
        {
            if (robot == null) return false;
            
            switch (condition)
            {
                case ConditionType.PathAhead:
                    return robot.IsPathAhead();
                case ConditionType.WallAhead:
                    return robot.IsWallAhead();
                case ConditionType.OnGoal:
                    return robot.IsOnGoal();
                case ConditionType.ItemNearby:
                    return robot.IsItemNearby();
                default:
                    return false;
            }
        }
        
        public ConditionType GetCondition()
        {
            return condition;
        }
        
        public void SetCondition(ConditionType newCondition)
        {
            condition = newCondition;
        }
    }
}