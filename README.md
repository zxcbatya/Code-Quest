# Robot Coder - Educational Programming Game

[![Unity](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20WebGL-green.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-Proprietary-orange.svg)](LICENSE)

An engaging educational game that teaches programming concepts through visual block-based coding. Guide a robot through challenging levels by creating programs with drag-and-drop commands!

![Robot Coder Gameplay](https://placehold.co/800x400/4A90E2/FFFFFF?text=Robot+Coder+Gameplay+Preview)

## 🎯 About the Game

Robot Coder is an innovative educational platform designed for children aged 10-15 to learn programming fundamentals in a fun, interactive environment. Players control a robot through grid-based levels by arranging visual programming blocks, gradually mastering concepts from basic sequences to advanced conditionals.

### Core Features
- **Visual Programming Interface** - Intuitive drag-and-drop block system
- **Progressive Difficulty** - 10+ levels introducing programming concepts step-by-step
- **Educational Focus** - Teaches sequencing, loops, and conditionals
- **Cross-Platform** - Runs on PC and WebGL browsers

## 🚀 Getting Started

### Prerequisites
- Unity 2021.3 LTS or later
- TextMeshPro package
- Basic understanding of Unity UI system

### Installation
```bash
# Clone the repository
git clone https://github.com/yourusername/robot-coder.git

# Open in Unity
# Navigate to the project folder and open with Unity 2021.3+
```

### Quick Setup
1. Open the project in Unity
2. Import required assets from Package Manager
3. Open the main scene: `Assets/Scenes/Main.unity`
4. Press Play to start!

## 🎮 Gameplay Overview

### Core Mechanics
- **3D Top-Down Grid World** - Navigate a robot on a chess-like grid
- **Visual Block Programming** - Assemble commands by dragging blocks
- **Real-time Execution** - Watch your robot execute your program step-by-step

### Programming Concepts
| Level Range | Concepts Taught | Key Features |
|-------------|----------------|--------------|
| 1-5 | Sequencing | Basic movement commands |
| 6-10 | Loops | Repeat X times blocks |
| 11+ | Conditionals | IF/ELSE logic blocks |

### Available Commands
- ▶️ **Move Forward** - Move one space forward
- ↺ **Turn Left** - Rotate 90° counterclockwise
- ↻ **Turn Right** - Rotate 90° clockwise
- ⬆️ **Jump** - Leap over obstacles
- ✋ **Interact** - Activate objects

## 🏗️ Project Structure

```
Assets/
├── Scripts/
│   ├── Core/              # Game logic and systems
│   │   ├── CommandBlock.cs
│   │   ├── ProgramInterpreter.cs
│   │   ├── RobotController.cs
│   │   └── LevelManager.cs
│   ├── UI/                # Interface components
│   │   ├── MainMenuManager.cs
│   │   ├── GameUIController.cs
│   │   └── WorkspacePanel.cs
│   └── Utilities/         # Helper classes
├── Prefabs/               # Reusable game objects
├── Resources/             # ScriptableObjects and data
├── Scenes/                # Game scenes
└── Art/                   # Visual assets
```

## 🛠️ Development Setup

### Scene Configuration
1. **Main Camera** - Orthographic top-down view
2. **Grid System** - 8x8 default grid layout
3. **Lighting** - Directional light for clear visibility

### Core Component Setup
1. **GameManager** - Add to scene with all required references
2. **LevelManager** - Configure with LevelData ScriptableObjects
3. **UI Canvas** - Contains all interface elements
4. **Robot** - Player character with RobotController component

### Level Creation Process
1. Create new LevelData ScriptableObject
2. Configure grid size and robot start position
3. Place goals, walls, and interactive elements
4. Set available commands for the level
5. Add to LevelManager's level array

## 🎨 Visual Design

### Art Style Guidelines
- **Low-poly aesthetic** - Clean, simple geometric shapes
- **Bright color palette** - High contrast for accessibility
- **Consistent proportions** - All elements fit the grid system

### Recommended Asset Sources
- **Robot Models**: OpenGameArt.org (CC0 license)
- **UI Elements**: Unity Asset Store free Sci-Fi packs
- **Sound Effects**: Freesound.org (Creative Commons)

## 🧪 Testing & Debugging

### Debug Controls
- `Space` - Start/Stop program execution
- `R` - Reset current program
- `F1` - Display system status
- `Esc` - Pause game

### Common Issues
- **Robot not moving**: Check ProgramInterpreter connections
- **Blocks not dragging**: Verify EventSystem exists
- **Levels not loading**: Confirm LevelManager configuration

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📚 Documentation

- [System Documentation](SYSTEM_DOCUMENTATION.md) - Complete technical documentation
- [Level Design Guide](docs/LEVEL_DESIGN.md) - Creating new levels
- [Command Implementation](docs/COMMANDS.md) - Adding new programming blocks

## 📄 License

This project is proprietary and intended for educational purposes. See [LICENSE](LICENSE) for details.

## 🙋 Support

For questions and support, please open an issue in the repository or contact the development team.