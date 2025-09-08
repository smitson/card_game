# Unity Card Game Project (Grandma's Solitaire)

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

This is a Unity 3D solitaire card game project built with Unity 2021.3.23f1 LTS. The project is designed for mobile platforms with touch controls and includes a complete card game implementation with scoring system.

## Working Effectively

### Prerequisites and Unity Installation

**CRITICAL: Unity 2021.3.23f1 LTS is required**

Install Unity Hub and Unity Editor:
- Download Unity Hub from https://unity3d.com/get-unity/download
- Install Unity 2021.3.23f1 LTS through Unity Hub
- Linux users: Use the official Unity Hub installer or download Unity Editor directly from https://download.unity3d.com/download_unity/213b516bf396/LinuxEditorInstaller/Unity-2021.3.23f1.tar.xz
- Ensure you have at least 10GB free disk space for Unity installation and project builds

### Project Setup and Build Process

**Bootstrap the project:**
- Clone the repository to your local machine
- Open Unity Hub
- Click "Add" and select the project root directory (`/card_game`)
- Unity will automatically detect this as a Unity 2021.3.23f1 project
- Click on the project to open it in Unity Editor

**Initial project load time: 5-10 minutes on first open** - NEVER CANCEL during Unity's initial import process.

### Building the Project

**Development Builds:**
- Open Unity Editor with the project loaded
- Go to `File > Build Settings`
- Select your target platform (PC, Mac & Linux Standalone, Android, or iOS)
- Click "Build" or "Build and Run"
- **Build time: 3-8 minutes depending on platform** - NEVER CANCEL. Set timeout to 15+ minutes.

**Platform-specific build commands:**
- PC/Mac/Linux: Select "PC, Mac & Linux Standalone" in Build Settings
- Android: Requires Android SDK and NDK. Install via Unity Hub > Installs > Add Modules
- iOS: Requires Xcode (Mac only)

### Testing and Validation

**Unity Test Framework is available (v1.1.31) but no automated tests exist currently.**

**Manual Testing Process:**
- **ALWAYS manually test the game after making changes**
- Build and run the project
- **Test complete game flow (MANDATORY validation):**
  1. **Card dealing**: New game should automatically deal 52 cards in pyramid layout
  2. **Card selection**: Click/touch cards to select (selected card turns yellow)
  3. **Card removal**: Click pairs of cards that add up to 13 (e.g., Ace+Queen, 6+7, King alone)
  4. **Score tracking**: Score should decrease from 52 as cards are removed (lower score is better)
  5. **Undo functionality**: Test undo button - should restore last removed card pair and add 2 to score
  6. **Deck dealing**: Click deck button to deal new cards when no more moves available
  7. **Game completion**: Remove all cards to win (score saved if it's a new best)
  8. **Reset/Play Again**: Test reset button to start a new game
- **Game should run at 60 FPS on target platforms**
- **Scoring validation**: Best score should be stored in PlayerPrefs and persist between sessions

**Key Game Scenes to Test:**
- `Assets/Scenes/SolitaireGame.unity` - Main game scene
- `Assets/Scenes/ScoreScreen.unity` - Score display scene

### Code Navigation and Structure

**Key Directories:**
- `Assets/Scripts/` - All C# game logic
- `Assets/Scenes/` - Unity scene files
- `Assets/Sprites/` - 2D art assets
- `Assets/Playing Cards/` - Card asset pack (3D models and textures)
- `Assets/Prefabs/` - Reusable game objects
- `ProjectSettings/` - Unity project configuration
- `Packages/` - Unity package dependencies

**Core Scripts (always check these when making gameplay changes):**
- `Solitaire.cs` - **CORE GAME LOGIC**: Card dealing (PlayCards), scoring (updateScores), undo system (UndoCards), card movement (MoveCards), deck generation (GenerateDeck)
- `UIButtons.cs` - UI button handlers (Undo, Reset, Play Again) - connects UI to Solitaire.cs methods
- `UpdateSprite.cs` - Card visual updates and animations - handles card face/back display and selection highlighting
- `InputManager.cs` - Singleton input manager using Unity Input System - handles touch/mouse events
- `TouchControls.cs` - Touch gesture detection for mobile platforms
- `UserInput.cs` - User interaction processing and card selection logic
- `ScoreScreen.cs` - Score display and statistics tracking (score ranges, total games)

**Critical Game Logic Methods in Solitaire.cs:**
- `PlayCards()` - Initializes new game, generates and shuffles deck
- `MoveCards()` - Positions cards in pyramid layout, decreases score
- `UndoCards()` - Restores last move, adds 2 penalty to score
- `DealFromDeck()` - Deals new cards when player clicks deck
- `updateScores()` - Updates best score and statistics in PlayerPrefs

**When modifying game logic:**
- **ALWAYS test in `Solitaire.cs` first** - this contains the core game state and logic
- Check `UIButtons.cs` for UI-related changes - ensure buttons call correct Solitaire methods
- Validate touch controls work on both mobile and desktop platforms
- **Card selection logic**: Selected cards turn yellow (handled in UpdateSprite.cs)
- **Scoring system**: Lower scores are better (start at 52, decrease as cards removed)
- **Game rule**: Remove pairs of cards that sum to 13 (A=1, J=11, Q=12, K=13 alone)

**Game State Debugging:**
- Check Unity Console for Debug.Log outputs from game scripts
- Monitor `dealtCards` list for card state tracking
- Verify `currentScore` and `bestScore` values during gameplay
- Use Unity Inspector to check public variables on Solitaire component

### Development Workflow

**Making Changes:**
1. Open Unity Editor with the project
2. Make your changes to scripts or scenes
3. Unity auto-compiles C# scripts (compilation errors appear in Console)
4. Test changes in Play Mode first (click Play button in Unity Editor)
5. Build and test on target platform for final validation
6. **ALWAYS test complete user scenarios** - starting a game, playing cards, winning/losing

**Script Editing:**
- Use Visual Studio Code (configured via `.vscode/launch.json`)
- Unity automatically recompiles when you save C# files
- Check Unity Console for compilation errors
- Use Unity Inspector to modify public variables on MonoBehaviour scripts

### Common Issues and Troubleshooting

**Unity Editor Issues:**
- "Library folder is corrupted": Delete `Library/` folder and restart Unity
- "Package resolution failed": Delete `Packages/packages-lock.json` and restart Unity
- Slow compilation: Ensure Windows Defender/antivirus excludes Unity project folder

**Build Issues:**
- Android builds require proper SDK/NDK setup through Unity Hub
- iOS builds require Mac with Xcode installed
- Build errors often relate to missing platform-specific modules

**Performance Issues:**
- Target framerate is 60 FPS
- Test on actual mobile devices, not just Unity Editor
- Check Unity Profiler if performance issues occur

### Platform-Specific Notes

**Mobile (Android/iOS):**
- Touch input is primary interaction method
- Test on actual devices for accurate performance
- Consider various screen resolutions and aspect ratios

**Desktop (PC/Mac/Linux):**
- Mouse input simulates touch
- Keyboard shortcuts may not be implemented
- Test window resizing behavior

## Project Dependencies

**Unity Packages (managed in `Packages/manifest.json`):**
- Unity Input System 1.5.1 (touch/input handling)
- TextMeshPro 3.0.6 (UI text rendering)
- Unity Test Framework 1.1.31 (available for future testing)
- Cinemachine 2.8.9 (camera system)
- 2D Feature set for sprite handling

**External Assets:**
- Playing Cards Pack from GameAssetStudio (3D card models and animations)
- License: See `Assets/Playing Cards/Readme_Eng.txt`

## Build Targets and Distribution

**Supported Platforms:**
- PC (Windows, Mac, Linux) - Primary target
- Android - Mobile target
- iOS - Mobile target (requires Mac for building)

**Build Configuration:**
- Default resolution: 1920x1080 (PC), adaptive (mobile)
- Company: "Catom Industries"
- Product: "Grandma's solitaire"
- Default orientation: Landscape (can rotate to portrait)

## Validation Checklist

Before committing any changes, ALWAYS:
- [ ] Code compiles without errors in Unity Console
- [ ] Play Mode test completes successfully (Play button in Unity Editor)
- [ ] Build completes without errors (test target platform)
- [ ] **CRITICAL: Complete game test scenario:**
  - [ ] New game starts with 52 cards in pyramid layout
  - [ ] Card selection works (cards turn yellow when selected)  
  - [ ] Card pair removal works (sum to 13 rule)
  - [ ] Score decreases correctly as cards are removed
  - [ ] Undo button works and adds 2-point penalty
  - [ ] Deck button deals new cards when available
  - [ ] Game completion detected when all cards removed
  - [ ] Reset/Play Again functionality works
  - [ ] Score persistence (best score saved between sessions)
- [ ] Performance: Game runs at stable framerate (60 FPS target)
- [ ] UI: All buttons and interactions work on target platform (mobile/desktop)
- [ ] Touch controls work correctly (if mobile target)

**Automated Validation (when available):**
- Unity Test Framework is installed but no tests currently exist
- Consider adding Unit Tests for card logic (GenerateDeck, card sum validation)
- Consider adding Integration Tests for game state transitions

## Time Expectations

**NEVER CANCEL these operations - set appropriate timeouts:**
- Unity first-time project open: 5-10 minutes
- Script compilation: 30 seconds - 2 minutes
- Build process: 3-8 minutes (timeout: 15+ minutes)
- Platform module installation: 10-30 minutes (timeout: 45+ minutes)

## Quick Reference Commands

**Common Unity Editor shortcuts:**
- `Ctrl/Cmd + S` - Save scene/project
- `Ctrl/Cmd + P` - Play/Stop play mode
- `Ctrl/Cmd + Shift + B` - Open Build Settings
- `F5` - Refresh Project window

**Project file locations for quick access:**
```
Assets/Scenes/SolitaireGame.unity     # Main game scene - primary development scene
Assets/Scenes/ScoreScreen.unity       # Score display scene  
Assets/Scripts/Solitaire.cs           # Core game logic - most important script
Assets/Scripts/UIButtons.cs           # UI functionality - button event handlers
Assets/Scripts/UpdateSprite.cs        # Card rendering and selection visuals
Assets/Scripts/InputManager.cs        # Input system singleton manager
ProjectSettings/ProjectSettings.asset  # Build configuration and player settings
Packages/manifest.json                 # Package dependencies
```

## Common Development Tasks

**Adding new game features:**
1. Modify core logic in `Solitaire.cs`
2. Add UI elements and wire to `UIButtons.cs` 
3. Test in Play Mode first, then build and test
4. Follow complete validation checklist

**Debugging card logic:**
1. Add Debug.Log statements to `Solitaire.cs` methods
2. Use Unity Console window to monitor game state
3. Check Inspector values on Solitaire GameObject
4. Verify card naming conventions match GenerateDeck() output (e.g., "SA" = Spade Ace)

**Performance optimization:**
1. Use Unity Profiler (Window > Analysis > Profiler)
2. Target 60 FPS on mobile devices
3. Check Draw Calls and SetPass Calls in Game view stats
4. Monitor memory usage during gameplay

**Mobile platform testing:**
1. Test touch gestures vs mouse clicks
2. Verify UI scaling on different screen sizes
3. Test performance on actual mobile devices
4. Check battery usage during extended gameplay

ALWAYS follow this validation workflow and NEVER skip manual testing of game functionality.