## ADDED Requirements

### Requirement: Board topology types
The system SHALL support two board topologies: Rectangle and Cylinder, represented as an enum `EBoardTopology`.

#### Scenario: Rectangle topology exists
- **WHEN** a game is configured with Rectangle topology
- **THEN** the board behaves as a flat rectangle with hard column boundaries

#### Scenario: Cylinder topology exists
- **WHEN** a game is configured with Cylinder topology
- **THEN** the board wraps horizontally so the leftmost and rightmost columns are adjacent

### Requirement: AI difficulty levels
The system SHALL support three AI difficulty levels: Easy, Medium, and Hard, represented as an enum `EAIDifficulty`.

#### Scenario: All difficulty levels available
- **WHEN** configuring an AI player
- **THEN** the user can select Easy, Medium, or Hard difficulty

### Requirement: Player type definition
The system SHALL represent a player type as a value containing whether the player is AI-controlled and, if so, the AI difficulty level.

#### Scenario: Human player type
- **WHEN** a player type is created as non-AI
- **THEN** the IsAI property SHALL be false and Difficulty SHALL be null

#### Scenario: AI player type
- **WHEN** a player type is created as AI with Hard difficulty
- **THEN** the IsAI property SHALL be true and Difficulty SHALL be Hard

### Requirement: Game configuration
The system SHALL provide a `GameConfig` that holds all parameters needed to start a game: board rows, board columns, win condition (number in a row to win), player 1 name, player 1 symbol, player 2 name, player 2 symbol, board topology, player 1 type, and player 2 type.

#### Scenario: Complete configuration
- **WHEN** a GameConfig is created with rows=6, columns=7, winCondition=4, two player names/symbols, Rectangle topology, and two human player types
- **THEN** all properties SHALL be accessible and correctly stored

### Requirement: Configuration validation
The system SHALL validate game configuration and report whether it is valid via an `IsValid()` method.

#### Scenario: Valid standard configuration
- **WHEN** IsValid() is called on a config with rows=6, cols=7, winCondition=4
- **THEN** it SHALL return true

#### Scenario: Win condition exceeds board dimensions
- **WHEN** IsValid() is called on a config where winCondition is greater than both rows and columns
- **THEN** it SHALL return false

#### Scenario: Zero or negative dimensions
- **WHEN** IsValid() is called on a config with rows=0 or columns=0
- **THEN** it SHALL return false

#### Scenario: Duplicate player symbols
- **WHEN** IsValid() is called on a config where both players have the same symbol
- **THEN** it SHALL return false

#### Scenario: Empty player names
- **WHEN** IsValid() is called on a config where a player name is empty or whitespace
- **THEN** it SHALL return false
