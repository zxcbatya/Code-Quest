# Level Creation Guide

## Overview

This guide explains how to create new levels for the Robot Coder game. Levels are defined using ScriptableObject assets that contain all the necessary information for the level layout, robot starting position, goals, and available commands.

## Creating a New Level

### Step 1: Create Level Data Asset

1. In the Unity Editor, right-click in the Project window
2. Select `Create` → `Robot Coder` → `Level Data`
3. Name your level asset (e.g., "Level_01")

### Step 2: Configure Basic Level Properties

In the Inspector window, configure these properties:

- **Level Index**: Sequential number for the level
- **Level Name**: Display name for the level
- **Description**: Brief description of the level's objective
- **Difficulty**: Rate from 1-5 (affects UI presentation)
- **Max Commands**: Maximum commands player can use
- **Optimal Commands**: Ideal command count for 3 stars

### Step 3: Set Robot Starting Position

- **Start Position**: Grid coordinates where robot begins (e.g., 0,0)
- **Start Direction**: Initial direction (0=North, 1=East, 2=South, 3=West)

### Step 4: Define Goal Positions

- Add one or more Vector2Int positions for goal locations
- Robot must reach any/all goals to complete level (based on requireAllGoals setting)

### Step 5: Configure Grid Layout

#### Grid Dimensions
- **Grid Width**: Number of columns (default 8)
- **Grid Height**: Number of rows (default 8)

#### Tile Types
- **Empty**: Normal walkable space
- **Wall**: Impassable barrier
- **Goal**: Level completion point
- **Pit**: Hazard that stops robot
- **Button**: Interactive element
- **Door**: Conditional barrier
- **Key**: Collectible item

### Step 6: Enable Available Commands

Check the boxes for commands players can use:
- Move Forward
- Turn Left
- Turn Right
- Jump
- Interact
- Repeat
- If
- Else

## Example Level Configurations

### Beginner Level (Level 1)
```
Level Index: 1
Level Name: "First Steps"
Description: "Learn to move the robot"
Difficulty: 1
Max Commands: 10
Optimal Commands: 5

Start Position: (0,0)
Start Direction: 1 (East)

Goal Positions: [(7,0)]

Grid: 8x8 with simple path, no obstacles

Available Commands:
- Move Forward
- Turn Left
- Turn Right
```

### Intermediate Level (Level 5)
```
Level Index: 5
Level Name: "The Maze"
Description: "Navigate through a simple maze"
Difficulty: 2
Max Commands: 20
Optimal Commands: 12

Start Position: (0,0)
Start Direction: 1 (East)

Goal Positions: [(7,7)]

Grid: 8x8 with walls creating a maze pattern

Available Commands:
- Move Forward
- Turn Left
- Turn Right
- Jump
```

### Advanced Level (Level 10)
```
Level Index: 10
Level Name: "Loop Challenge"
Description: "Use loops to solve efficiently"
Difficulty: 4
Max Commands: 30
Optimal Commands: 15

Start Position: (0,0)
Start Direction: 1 (East)

Goal Positions: [(7,7)]

Grid: 8x8 with complex pattern requiring loops

Available Commands:
- Move Forward
- Turn Left
- Turn Right
- Jump
- Repeat
```

## Custom Level Types

### Creating Specialized Levels

To create a level with unique properties:

1. Create a new C# script inheriting from `LevelData`
2. Override the `OnEnable` method to set default values
3. Add custom properties as needed
4. Create the asset using your new type

Example:
```csharp
[CreateAssetMenu(fileName = "PuzzleLevel", menuName = "Robot Coder/Puzzle Level")]
public class PuzzleLevelData : LevelData
{
    [Header("Puzzle Settings")]
    public int puzzlePieces = 4;
    public bool requireAllPieces = true;
    
    private void OnEnable()
    {
        // Set default values for puzzle levels
        levelName = "Puzzle Level";
        difficulty = 3;
        // ... other defaults
    }
}
```

## Testing Levels

### In-Editor Testing
1. Create a test scene with necessary managers
2. Add your level to the LevelManager
3. Enter play mode
4. Use test controls to verify level functionality

### Playtesting Checklist
- [ ] Robot starts in correct position
- [ ] All walls and obstacles function correctly
- [ ] Goals are reachable
- [ ] Commands work as expected
- [ ] Level can be completed
- [ ] Star ratings are achievable
- [ ] UI displays correctly

## Level Progression

### Difficulty Curve
1. **Levels 1-3**: Basic movement
2. **Levels 4-6**: Simple obstacles
3. **Levels 7-9**: Jump mechanics
4. **Levels 10-12**: Repeat loops
5. **Levels 13+**: Conditional logic

### Command Introduction
- Introduce one new command type every 2-3 levels
- Combine previously learned commands in new ways
- Gradually increase complexity

## Best Practices

### Design Principles
1. **Clear Objective**: Each level should have one main learning goal
2. **Incremental Difficulty**: Add complexity gradually
3. **Multiple Solutions**: Allow different approaches to solve
4. **Visual Clarity**: Make goals and obstacles obvious
5. **Fair Challenge**: Achievable but not trivial

### Grid Design
- Keep important elements away from edges
- Provide visual feedback for interactions
- Balance open areas with structured paths
- Use symmetry for aesthetic appeal

### Command Balance
- Enable only necessary commands
- Disable commands that break level design
- Consider command combinations
- Test with minimal command solutions

## Troubleshooting

### Common Issues

**Robot gets stuck**
- Check for enclosed spaces
- Verify wall placements
- Ensure goal is reachable

**Level is too hard**
- Add visual hints
- Reduce optimal command count
- Simplify grid layout

**Level is too easy**
- Reduce max commands
- Add more obstacles
- Require more complex solutions

### Debugging Tools
- Use the LevelTest component
- Check console for error messages
- Verify LevelData serialization
- Test in different screen resolutions

## Advanced Features

### Dynamic Elements
Future versions could include:
- Moving obstacles
- Timed elements
- Interactive NPCs
- Environmental hazards

### Procedural Generation
For randomly generated levels:
- Define generation rules
- Create template patterns
- Implement validation checks
- Balance difficulty algorithmically

## Conclusion

Creating good levels is key to a successful educational game. Focus on clear learning objectives, gradual difficulty progression, and engaging challenges. Test frequently and gather feedback from your target audience to refine the experience.