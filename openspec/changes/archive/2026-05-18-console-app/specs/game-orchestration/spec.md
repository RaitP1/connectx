## ADDED Requirements

### Requirement: Start new game from configuration
The system SHALL allow starting a new game using either a selected saved configuration or the current custom settings.

#### Scenario: Start game from preset
- **WHEN** the user selects "Classical" from the configuration list and starts a new game
- **THEN** a new `GameBrain` is created with the Classical config (7x6, connect 4, rectangle)

#### Scenario: Start game from custom settings
- **WHEN** the user modifies board width to 9 and starts a new game
- **THEN** a new `GameBrain` is created with the custom config including width 9

### Requirement: Human vs Human game loop
The system SHALL alternate turns between two human players, each using arrow-key column selection.

#### Scenario: Two humans alternate turns
- **WHEN** a Human vs Human game is in progress
- **THEN** Player 1 and Player 2 alternate column selection turns until the game ends

### Requirement: Human vs AI game loop
The system SHALL alternate between human input and AI-computed moves when one player is configured as AI.

#### Scenario: AI takes its turn automatically
- **WHEN** it is the AI player's turn
- **THEN** the system computes a move via `IAIPlayer.GetMove()` and places it without user input

#### Scenario: Human plays against AI
- **WHEN** a Human vs AI game is in progress and the human makes a move
- **THEN** the AI responds with its computed move on the next turn

### Requirement: AI vs AI game loop
The system SHALL automatically play both sides when both players are configured as AI, with a brief pause between moves for visibility.

#### Scenario: Both AI players alternate
- **WHEN** an AI vs AI game is started
- **THEN** both moves are computed automatically and the game plays to completion

#### Scenario: Pause between AI moves
- **WHEN** an AI makes a move in AI vs AI mode
- **THEN** there is a visible delay before the next AI move so the user can follow the game

### Requirement: Save current game
The system SHALL allow saving the in-progress game state with a user-provided name.

#### Scenario: Save mid-game
- **WHEN** the player triggers save during a game and enters the name "my-game"
- **THEN** the game state is persisted via `IGameRepository.Save()` with the name "my-game"

### Requirement: Load and continue saved game
The system SHALL allow loading a previously saved game and resuming play from the saved state.

#### Scenario: Load saved game
- **WHEN** the user selects a saved game from the list
- **THEN** the game resumes with the saved board state, current player, and configuration

### Requirement: Delete saved game
The system SHALL allow deleting a saved game from the list.

#### Scenario: Delete a save
- **WHEN** the user selects a saved game and confirms deletion
- **THEN** the save is removed via `IGameRepository.Delete()` and no longer appears in the list

### Requirement: Save configuration
The system SHALL allow saving the current custom configuration with a user-provided name.

#### Scenario: Save custom config
- **WHEN** the user enters a name and saves the current settings
- **THEN** the configuration is persisted via `IConfigRepository.Save()`

### Requirement: Load configuration
The system SHALL allow loading a saved configuration to use as the active settings.

#### Scenario: Load preset config
- **WHEN** the user selects "Connect-3" from the config list
- **THEN** the active configuration updates to 5x4 board, connect 3, rectangle

### Requirement: Delete configuration
The system SHALL allow deleting a saved configuration.

#### Scenario: Delete a config
- **WHEN** the user selects a configuration and confirms deletion
- **THEN** the config is removed via `IConfigRepository.Delete()` and no longer appears in the list

### Requirement: Change board dimensions
The system SHALL allow changing the board width and height through the settings menu.

#### Scenario: Change board width
- **WHEN** the user selects "Change Board Width" and enters 9
- **THEN** the active configuration's column count updates to 9

#### Scenario: Change board height
- **WHEN** the user selects "Change Board Height" and enters 8
- **THEN** the active configuration's row count updates to 8

### Requirement: Change win condition
The system SHALL allow changing the number of pieces in a row needed to win.

#### Scenario: Change win condition
- **WHEN** the user selects "Change Win Condition" and enters 5
- **THEN** the active configuration's win condition updates to 5

### Requirement: Change board topology
The system SHALL allow switching between Rectangle and Cylinder topology.

#### Scenario: Toggle topology
- **WHEN** the user selects "Change Topology" while topology is Rectangle
- **THEN** the topology changes to Cylinder (and vice versa)

### Requirement: Configure player types
The system SHALL allow setting each player as Human or AI (with difficulty selection for AI).

#### Scenario: Set player to AI with difficulty
- **WHEN** the user configures Player 1 as AI and selects Hard difficulty
- **THEN** Player 1's type updates to AI with Hard difficulty

#### Scenario: Set player to Human
- **WHEN** the user configures Player 2 as Human
- **THEN** Player 2's type updates to Human

### Requirement: View current settings
The system SHALL display a summary of all current configuration values.

#### Scenario: Show settings summary
- **WHEN** the user selects "View Current Settings"
- **THEN** the display shows board dimensions, win condition, topology, and both player configurations

### Requirement: Show how to play
The system SHALL display game rules and controls.

#### Scenario: Display help
- **WHEN** the user selects "How to Play"
- **THEN** the system shows the game objective, controls (arrow keys, Enter, S, Q), and basic rules
