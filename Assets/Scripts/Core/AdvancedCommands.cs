using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RobotCoder.Core;
using Core;
using RobotCoder.UI;

namespace RobotCoder.Core
{
    public class RepeatCommand : CommandBlock
    {
        [Header("Настройки повтора")]
        [SerializeField] private int repeatCount = 2;
        [SerializeField] private TMP_InputField repeatInputField;
        [SerializeField] private DropZone innerDropZone;
        
        public override void InitializeBlock()
        {
            commandType = CommandType.Repeat;
            blockColor = new Color(0.8f, 0.4f, 0.2f);
            base.InitializeBlock();
            
            SetupRepeatInput();
        }
        
        private void SetupRepeatInput()
        {
            if (repeatInputField != null)
            {
                repeatInputField.text = repeatCount.ToString();
                repeatInputField.onValueChanged.AddListener(OnRepeatCountChanged);
            }
        }
        
        private void OnRepeatCountChanged(string value)
        {
            if (int.TryParse(value, out int newCount))
            {
                repeatCount = Mathf.Clamp(newCount, 1, 10);
                repeatInputField.text = repeatCount.ToString();
            }
        }

        public override bool Execute(RobotController robot)
        {
            HighlightExecution();
            
            var innerBlocks = innerDropZone.GetOrderedBlocks();
            
            for (int i = 0; i < repeatCount; i++)
            {
                foreach (var block in innerBlocks)
                {
                    if (!block.Execute(robot))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        public int GetRepeatCount()
        {
            return repeatCount;
        }
        
        public void SetRepeatCount(int count)
        {
            repeatCount = Mathf.Clamp(count, 1, 10);
            if (repeatInputField != null)
            {
                repeatInputField.text = repeatCount.ToString();
            }
        }
    }

    public class IfCommand : CommandBlock
    {
        [Header("Настройки условия")]
        [SerializeField] private ConditionType condition = ConditionType.PathAhead;
        [SerializeField] private TMP_Dropdown conditionDropdown;
        [SerializeField] private DropZone trueDropZone;
        [SerializeField] private DropZone falseDropZone;
        
        public enum ConditionType
        {
            PathAhead,
            WallAhead,
            GoalReached,
            ItemPresent
        }
        
        public override void InitializeBlock()
        {
            commandType = CommandType.If;
            blockColor = new Color(0.6f, 0.2f, 0.8f);
            base.InitializeBlock();
            
            SetupConditionDropdown();
        }
        
        private void SetupConditionDropdown()
        {
            if (conditionDropdown != null)
            {
                conditionDropdown.ClearOptions();
                
                var options = new System.Collections.Generic.List<string>
                {
                    "Путь свободен",
                    "Стена впереди", 
                    "Цель достигнута",
                    "Предмет рядом"
                };
                
                conditionDropdown.AddOptions(options);
                conditionDropdown.value = (int)condition;
                conditionDropdown.onValueChanged.AddListener(OnConditionChanged);
            }
        }
        
        private void OnConditionChanged(int value)
        {
            condition = (ConditionType)value;
        }

        public override bool Execute(RobotController robot)
        {
            HighlightExecution();
            
            bool conditionResult = EvaluateCondition(robot);
            DropZone targetZone = conditionResult ? trueDropZone : falseDropZone;
            
            var blocksToExecute = targetZone.GetOrderedBlocks();
            
            foreach (var block in blocksToExecute)
            {
                if (!block.Execute(robot))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private bool EvaluateCondition(RobotController robot)
        {
            if (robot == null) return false;
            
            switch (condition)
            {
                case ConditionType.PathAhead:
                    return robot.IsPathAhead();
                case ConditionType.WallAhead:
                    return robot.IsWallAhead();
                case ConditionType.GoalReached:
                    return robot.IsOnGoal();
                case ConditionType.ItemPresent:
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
            if (conditionDropdown != null)
            {
                conditionDropdown.value = (int)condition;
            }
        }
    }
}