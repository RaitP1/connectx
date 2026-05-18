## ADDED Requirements

### Requirement: AI player interface
The system SHALL define an `IAIPlayer` interface in `Application.AI` with a method `GetMove(GameBrain brain, int player)` that returns the column index for the AI's chosen move.

#### Scenario: Interface contract
- **WHEN** any IAIPlayer implementation is given a game brain and player index
- **THEN** it SHALL return a valid column index from the available columns

### Requirement: Minimax AI with alpha-beta pruning
The system SHALL provide a `MinimaxAI` implementation of `IAIPlayer` in `Application.AI` that uses minimax search with alpha-beta pruning to select moves. Position evaluation is provided by `BoardEvaluator.EvaluatePosition(brain, player)` in the same namespace.

#### Scenario: AI selects valid move
- **WHEN** MinimaxAI.GetMove is called on a board with available columns
- **THEN** it SHALL return a column index that is in GetAvailableColumns()

#### Scenario: AI blocks opponent win
- **WHEN** the opponent has 3 in a row with an open fourth slot (winCondition=4)
- **THEN** the AI SHALL place in the blocking column

#### Scenario: AI takes winning move
- **WHEN** the AI has 3 in a row with an open fourth slot (winCondition=4)
- **THEN** the AI SHALL place in the winning column

### Requirement: Difficulty controls search depth
The system SHALL control AI strength through search depth: Easy = depth 3, Medium = depth 5, Hard = depth 7.

#### Scenario: Easy AI uses shallow search
- **WHEN** a MinimaxAI is created with EAIDifficulty.Easy
- **THEN** it SHALL search to a maximum depth of 3

#### Scenario: Medium AI uses moderate search
- **WHEN** a MinimaxAI is created with EAIDifficulty.Medium
- **THEN** it SHALL search to a maximum depth of 5

#### Scenario: Hard AI uses deep search
- **WHEN** a MinimaxAI is created with EAIDifficulty.Hard
- **THEN** it SHALL search to a maximum depth of 7

### Requirement: AI handles edge cases
The system SHALL handle board states where only one move is available or the game is nearly over.

#### Scenario: Single available column
- **WHEN** only one column has space remaining
- **THEN** the AI SHALL return that column

#### Scenario: AI operates on game clone
- **WHEN** the AI computes its move
- **THEN** the original GameBrain state SHALL remain unchanged after GetMove returns
