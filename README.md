# Robot Coder - Educational Programming Game

## Project Overview

Robot Coder is an educational programming game designed for children aged 10-15. Players use visual programming blocks to control a robot through various levels, learning programming concepts in a fun and interactive way.

## Recent Updates and Fixes

### Memory Management Improvements
- Fixed memory leaks when transitioning between scenes
- Added SceneCleanupManager for automatic resource cleanup
- Implemented proper event listener cleanup in all UI components
- Added MemoryMonitor for tracking memory usage

### Pause System Fixes
- Fixed issue where game remained paused when returning to main menu
- Added ResumeGame() method to properly reset Time.timeScale
- Improved scene transition handling

### Event System Improvements
- Added ClearEventListeners() methods to all UI components
- Implemented proper event subscription/unsubscription patterns
- Fixed duplicate event handler registration

### Code Quality Enhancements
- Added null checks throughout all components
- Improved error handling and logging
- Added GameIntegrityChecker for system diagnostics
- Created comprehensive documentation

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/              # Core game systems
│   ├── UI/                # User interface components
│   └── ...                # Other script folders
├── Prefabs/               # Reusable game objects
├── Resources/             # Game resources
├── Scenes/                # Game scenes
└── ...                    # Other asset folders
```

## Core Systems

### 1. Robot Controller
- Controls robot movement and actions
- Handles animation and positioning
- Manages robot state (position, direction)

### 2. Level System
- LevelData: ScriptableObject for level configuration
- LevelManager: Manages level loading and progression
- GridManager: Handles grid-based movement and collision

### 3. Programming System
- CommandBlock: Base class for all programming blocks
- ProgramInterpreter: Executes sequences of commands
- WorkspacePanel: Area where players arrange commands

### 4. UI System
- MainMenuManager: Main menu navigation
- GameUIController: In-game UI management
- LevelSelector: Level selection interface

### 5. Game Management
- GameManager: Overall game state management
- SaveManager: Player progress and settings
- SettingsManager: Game configuration

## Creating Levels

### 1. Create Level Data Asset
1. Right-click in Project window
2. Select "Robot Coder" → "Level Data"
3. Configure level properties:
   - Level index and name
   - Robot start position and direction
   - Goal positions
   - Grid layout
   - Available commands

### 2. Configure Grid Layout
- Use the grid editor in the LevelData inspector
- Place walls, goals, and other elements
- Set grid dimensions (default 8x8)

### 3. Set Level Constraints
- Define maximum commands allowed
- Set optimal command count for stars
- Enable/disable specific commands

### 4. Add to Level Manager
- Add the level asset to the LevelManager's levels array
- Ensure levels are ordered correctly

## Command Blocks

### Basic Commands
- MoveForward: Move robot one space forward
- TurnLeft: Rotate robot 90° counterclockwise
- TurnRight: Rotate robot 90° clockwise
- Jump: Move forward, jumping over obstacles
- Interact: Interact with objects

### Advanced Commands
- Repeat: Execute a sequence multiple times
- If: Conditional execution based on conditions
- Else: Alternative execution path

## Customization

### Adding New Commands
1. Create a new class inheriting from CommandBlock
2. Implement the Execute method
3. Add to BlockManager prefabs
4. Configure in BlockPalette

### Creating New Level Types
1. Create a new LevelData subclass
2. Override OnEnable to set default values
3. Add custom properties as needed
4. Create corresponding prefabs

### Extending UI
1. Create new UI components in the UI folder
2. Use existing managers as reference
3. Connect to appropriate game systems
4. Test with different screen sizes

## Best Practices

### Performance
- Use object pooling for frequently created objects
- Limit simultaneous animations
- Optimize grid calculations
- Use efficient data structures

### Code Organization
- Keep classes focused on single responsibilities
- Use ScriptableObjects for data-driven design
- Implement interfaces for common functionality
- Follow consistent naming conventions

### Testing
- Test levels with different command sequences
- Verify edge cases (boundaries, obstacles)
- Check UI responsiveness
- Validate save/load functionality

## Deployment

### WebGL Build
1. Set build target to WebGL
2. Configure player settings for web deployment
3. Test in browser
4. Optimize build size and loading times

### Yandex Games
1. Integrate Yandex Games SDK
2. Configure leaderboard and advertisement settings
3. Test in Yandex Games environment
4. Publish to Yandex Games platform

## Troubleshooting

### Common Issues
- Level not loading: Check LevelManager configuration
- Robot not moving: Verify GridManager setup
- Commands not executing: Check ProgramInterpreter
- UI not responding: Validate event connections

### Memory Leak Prevention
- Always unsubscribe from events in OnDestroy
- Stop coroutines when objects are destroyed
- Use SceneCleanupManager for scene transitions
- Monitor memory usage with MemoryMonitor

### Pause System Issues
- Ensure Time.timeScale is reset when needed
- Use ResumeGame() before scene transitions
- Check that pause panels properly hide/show

### Debugging Tips
- Use debug logs liberally during development
- Test each system independently
- Use Unity's profiler for performance issues
- Check console for error messages
- Run GameIntegrityChecker for diagnostics

## Contributing

1. Fork the repository
2. Create a feature branch
3. Implement changes
4. Test thoroughly
5. Submit pull request

## License

This project is proprietary and intended for educational purposes.

## Additional Documentation

For more detailed information, see:
- [SYSTEM_DOCUMENTATION.md](SYSTEM_DOCUMENTATION.md) - Complete system documentation
- [README_RU.md](README_RU.md) - Russian version of this documentation