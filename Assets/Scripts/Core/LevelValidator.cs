using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class LevelValidator : MonoBehaviour
    {
        // Validate if a level is properly configured
        public bool ValidateLevel(LevelData levelData, out List<string> errors)
        {
            errors = new List<string>();
            
            if (levelData == null)
            {
                errors.Add("Level data is null");
                return false;
            }
            
            // Validate level index
            if (levelData.levelIndex <= 0)
            {
                errors.Add("Level index must be greater than 0");
            }
            
            // Validate level name
            if (string.IsNullOrEmpty(levelData.levelName))
            {
                errors.Add("Level name is required");
            }
            
            // Validate grid dimensions
            if (levelData.gridWidth <= 0 || levelData.gridHeight <= 0)
            {
                errors.Add("Grid dimensions must be greater than 0");
            }
            
            // Validate grid layout
            if (levelData.gridLayout == null)
            {
                errors.Add("Grid layout is not initialized");
            }
            else if (levelData.gridLayout.GetLength(0) != levelData.gridWidth || 
                     levelData.gridLayout.GetLength(1) != levelData.gridHeight)
            {
                errors.Add("Grid layout dimensions don't match grid width/height");
            }
            
            // Validate start position
            if (!IsValidGridPosition(levelData.startPosition, levelData))
            {
                errors.Add("Start position is outside grid bounds");
            }
            else if (IsPositionBlocked(levelData.startPosition, levelData))
            {
                errors.Add("Start position is blocked by obstacle");
            }
            
            // Validate goal positions
            if (levelData.goalPositions == null || levelData.goalPositions.Length == 0)
            {
                errors.Add("At least one goal position is required");
            }
            else
            {
                foreach (Vector2Int goal in levelData.goalPositions)
                {
                    if (!IsValidGridPosition(goal, levelData))
                    {
                        errors.Add($"Goal position {goal} is outside grid bounds");
                    }
                    else if (IsPositionBlocked(goal, levelData))
                    {
                        errors.Add($"Goal position {goal} is blocked by obstacle");
                    }
                }
            }
            
            // Validate command availability
            if (!levelData.allowMoveForward)
            {
                errors.Add("Move forward command should typically be allowed");
            }
            
            // Validate command limits
            if (levelData.maxCommands <= 0)
            {
                errors.Add("Maximum commands must be greater than 0");
            }
            
            if (levelData.optimalCommands <= 0)
            {
                errors.Add("Optimal commands must be greater than 0");
            }
            
            if (levelData.optimalCommands > levelData.maxCommands)
            {
                errors.Add("Optimal commands cannot exceed maximum commands");
            }
            
            return errors.Count == 0;
        }
        
        // Check if a position is valid within the grid
        private bool IsValidGridPosition(Vector2Int position, LevelData levelData)
        {
            return position.x >= 0 && position.x < levelData.gridWidth &&
                   position.y >= 0 && position.y < levelData.gridHeight;
        }
        
        // Check if a position is blocked by an obstacle
        private bool IsPositionBlocked(Vector2Int position, LevelData levelData)
        {
            if (!IsValidGridPosition(position, levelData)) return true;
            
            LevelData.TileType tileType = levelData.GetTile(position.x, position.y);
            return tileType == LevelData.TileType.Wall || 
                   tileType == LevelData.TileType.Pit;
        }
        
        // Check if a path exists from start to any goal
        public bool IsLevelSolvable(LevelData levelData, out List<string> warnings)
        {
            warnings = new List<string>();
            
            if (levelData == null) return false;
            
            // This is a simplified check
            // In a real implementation, you would use pathfinding
            
            // Check if start position can reach at least one goal
            bool canReachGoal = false;
            
            if (levelData.goalPositions != null)
            {
                foreach (Vector2Int goal in levelData.goalPositions)
                {
                    // Simple reachability check
                    if (CanReachPosition(levelData.startPosition, goal, levelData))
                    {
                        canReachGoal = true;
                        break;
                    }
                }
            }
            
            if (!canReachGoal)
            {
                warnings.Add("Нет пути от стартовой позиции к цели");
            }
            
            // Check if level has any available commands
            bool hasCommands = levelData.allowMoveForward || 
                              levelData.allowTurnLeft || 
                              levelData.allowTurnRight ||
                              levelData.allowJump ||
                              levelData.allowInteract ||
                              levelData.allowRepeat ||
                              levelData.allowIf ||
                              levelData.allowElse;
                              
            if (!hasCommands)
            {
                warnings.Add("Нет доступных команд для решения уровня");
            }
            
            return canReachGoal && hasCommands;
        }
        
        // Simplified position reachability check
        private bool CanReachPosition(Vector2Int start, Vector2Int goal, LevelData levelData)
        {
            // This is a very simplified check
            // In a real implementation, you would use proper pathfinding
            
            // For now, just check if both positions are valid and not blocked
            return IsValidGridPosition(start, levelData) && 
                   IsValidGridPosition(goal, levelData) &&
                   !IsPositionBlocked(start, levelData) && 
                   !IsPositionBlocked(goal, levelData);
        }
        
        // Generate a validation report
        public string GenerateValidationReport(LevelData levelData)
        {
            List<string> errors;
            bool isValid = ValidateLevel(levelData, out errors);
            
            List<string> warnings;
            bool isSolvable = IsLevelSolvable(levelData, out warnings);
            
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine($"=== Отчет валидации уровня {levelData.levelName} ===");
            report.AppendLine();
            
            report.AppendLine("Статус валидации:");
            report.AppendLine($"  Действителен: {isValid}");
            report.AppendLine($"  Решаем: {isSolvable}");
            report.AppendLine();
            
            if (errors.Count > 0)
            {
                report.AppendLine("Ошибки:");
                foreach (string error in errors)
                {
                    report.AppendLine($"  - {error}");
                }
                report.AppendLine();
            }
            
            if (warnings.Count > 0)
            {
                report.AppendLine("Предупреждения:");
                foreach (string warning in warnings)
                {
                    report.AppendLine($"  - {warning}");
                }
                report.AppendLine();
            }
            
            report.AppendLine("Настройки уровня:");
            report.AppendLine($"  Индекс: {levelData.levelIndex}");
            report.AppendLine($"  Сложность: {levelData.difficulty}/5");
            report.AppendLine($"  Размер сетки: {levelData.gridWidth}x{levelData.gridHeight}");
            report.AppendLine($"  Макс. команд: {levelData.maxCommands}");
            report.AppendLine($"  Оптимально: {levelData.optimalCommands}");
            report.AppendLine();
            
            return report.ToString();
        }
    }
}