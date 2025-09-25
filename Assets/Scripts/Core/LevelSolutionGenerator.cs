using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class LevelSolutionGenerator : MonoBehaviour
    {
        [Header("Solution Settings")]
        [SerializeField] private int maxSolutionLength = 50;
        [SerializeField] private bool allowRepeatCommands = true;
        [SerializeField] private bool allowConditionalCommands = true;
        
        public struct Solution
        {
            public List<CommandType> commands;
            public int commandCount;
            public bool isOptimal;
        }
        
        // Generate an optimal solution for a level
        public Solution GenerateOptimalSolution(LevelData levelData)
        {
            Solution solution = new Solution();
            solution.commands = new List<CommandType>();
            
            if (levelData == null || levelData.goalPositions == null || levelData.goalPositions.Length == 0)
            {
                return solution;
            }
            
            // For now, we'll create a simple solution
            // In a real implementation, you would use pathfinding algorithms
            
            // Simple solution: Move right to (7,0)
            for (int i = 0; i < 7; i++)
            {
                solution.commands.Add(CommandType.MoveForward);
            }
            
            solution.commandCount = solution.commands.Count;
            solution.isOptimal = true;
            
            return solution;
        }
        
        // Generate a sample solution for testing
        public Solution GenerateSampleSolution(LevelData levelData, int difficulty)
        {
            Solution solution = new Solution();
            solution.commands = new List<CommandType>();
            
            switch (difficulty)
            {
                case 1: // Very simple
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.TurnRight);
                    solution.commands.Add(CommandType.MoveForward);
                    break;
                    
                case 2: // Simple with turn
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.TurnLeft);
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.TurnRight);
                    solution.commands.Add(CommandType.MoveForward);
                    break;
                    
                case 3: // With repeat
                    if (allowRepeatCommands)
                    {
                        solution.commands.Add(CommandType.MoveForward);
                        solution.commands.Add(CommandType.Repeat);
                        // In a real implementation, you would handle nested commands
                    }
                    else
                    {
                        // Flatten the repeat
                        solution.commands.Add(CommandType.MoveForward);
                        for (int i = 0; i < 3; i++)
                        {
                            solution.commands.Add(CommandType.MoveForward);
                        }
                    }
                    break;
                    
                case 4: // With conditionals
                    if (allowConditionalCommands)
                    {
                        solution.commands.Add(CommandType.If);
                        // In a real implementation, you would handle conditional logic
                    }
                    else
                    {
                        // Simple alternative
                        solution.commands.Add(CommandType.MoveForward);
                        solution.commands.Add(CommandType.TurnRight);
                        solution.commands.Add(CommandType.MoveForward);
                    }
                    break;
                    
                default: // Complex
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.TurnLeft);
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.TurnRight);
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.MoveForward);
                    solution.commands.Add(CommandType.TurnLeft);
                    solution.commands.Add(CommandType.MoveForward);
                    break;
            }
            
            solution.commandCount = solution.commands.Count;
            solution.isOptimal = difficulty <= 2; // Simple solutions are optimal
            
            return solution;
        }
        
        // Validate if a solution can solve a level
        public bool ValidateSolution(LevelData levelData, List<CommandType> commands)
        {
            if (levelData == null || commands == null) return false;
            
            // In a real implementation, you would:
            // 1. Simulate the robot executing the commands
            // 2. Check if it reaches a goal position
            // 3. Ensure it doesn't exceed max commands
            // 4. Verify all commands are allowed
            
            // For now, we'll do a simple validation
            if (commands.Count > levelData.maxCommands)
            {
                return false; // Too many commands
            }
            
            // Check if only allowed commands are used
            foreach (CommandType command in commands)
            {
                if (!IsCommandAllowed(command, levelData))
                {
                    return false; // Command not allowed
                }
            }
            
            return true; // Basic validation passed
        }
        
        // Check if a command is allowed in a level
        private bool IsCommandAllowed(CommandType command, LevelData levelData)
        {
            switch (command)
            {
                case CommandType.MoveForward: return levelData.allowMoveForward;
                case CommandType.TurnLeft: return levelData.allowTurnLeft;
                case CommandType.TurnRight: return levelData.allowTurnRight;
                case CommandType.Jump: return levelData.allowJump;
                case CommandType.Interact: return levelData.allowInteract;
                case CommandType.Repeat: return levelData.allowRepeat;
                case CommandType.If: return levelData.allowIf;
                case CommandType.Else: return levelData.allowElse;
                default: return false;
            }
        }
        
        // Get hint for next command based on current position
        public CommandType GetHintCommand(LevelData levelData, Vector2Int currentPosition)
        {
            if (levelData == null || levelData.goalPositions == null || levelData.goalPositions.Length == 0)
            {
                return CommandType.MoveForward; // Default hint
            }
            
            // Simple hint: move toward the first goal
            Vector2Int goal = levelData.goalPositions[0];
            Vector2Int diff = goal - currentPosition;
            
            // If we're at the goal, no hint needed
            if (diff == Vector2Int.zero)
            {
                return CommandType.Interact; // Celebrate!
            }
            
            // Simple direction-based hint
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                // Move horizontally
                return diff.x > 0 ? CommandType.MoveForward : CommandType.TurnLeft;
            }
            else
            {
                // Move vertically
                return diff.y > 0 ? CommandType.MoveForward : CommandType.TurnRight;
            }
        }
        
        // Generate a sequence of hints for a level
        public List<CommandType> GenerateHintSequence(LevelData levelData)
        {
            List<CommandType> hints = new List<CommandType>();
            
            if (levelData == null) return hints;
            
            // Generate a simple hint sequence
            Solution optimalSolution = GenerateOptimalSolution(levelData);
            hints = new List<CommandType>(optimalSolution.commands);
            
            return hints;
        }
    }
}