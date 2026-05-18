## ADDED Requirements

### Requirement: Map GameBrain to GameStateDto
The system SHALL provide a `GameStateMapper` in `Application.Game.Mapping` with a static method `ToDto(GameBrain brain, string name)` that creates a `GameStateDto` snapshot of the current game state.

#### Scenario: ToDto captures board state
- **WHEN** `ToDto` is called on a GameBrain with pieces on the board
- **THEN** the resulting DTO's Board SHALL contain the same cell values as the brain's board

#### Scenario: ToDto captures current player
- **WHEN** `ToDto` is called on a GameBrain where it is player 1's turn
- **THEN** the resulting DTO's CurrentPlayer SHALL be 1

#### Scenario: ToDto captures config
- **WHEN** `ToDto` is called on a GameBrain
- **THEN** the resulting DTO's Config SHALL equal the brain's Config

#### Scenario: ToDto sets SavedAt to current time
- **WHEN** `ToDto` is called
- **THEN** the resulting DTO's SavedAt SHALL be approximately the current UTC time

#### Scenario: ToDto sets name
- **WHEN** `ToDto` is called with name "my-save"
- **THEN** the resulting DTO's Name SHALL be "my-save"

### Requirement: Map GameStateDto to GameBrain
The system SHALL provide a static method `ToDomain(GameStateDto dto)` on `GameStateMapper` that reconstructs a `GameBrain` from a saved DTO.

#### Scenario: ToDomain restores board state
- **WHEN** `ToDomain` is called with a DTO containing a board with pieces
- **THEN** the resulting GameBrain's cells SHALL match the DTO's Board values

#### Scenario: ToDomain restores current player
- **WHEN** `ToDomain` is called with a DTO where CurrentPlayer is 1
- **THEN** the resulting GameBrain's CurrentPlayer SHALL be 1

#### Scenario: ToDomain restores config
- **WHEN** `ToDomain` is called with a DTO
- **THEN** the resulting GameBrain's Config SHALL equal the DTO's Config

#### Scenario: ToDomain restores cylinder topology
- **WHEN** `ToDomain` is called with a DTO whose Config has Cylinder topology
- **THEN** the resulting GameBrain SHALL use Cylinder column wrapping

### Requirement: Round-trip fidelity
Mapping a GameBrain to DTO and back SHALL produce a GameBrain with identical observable state.

#### Scenario: Round-trip preserves board
- **WHEN** a GameBrain is mapped to DTO via `ToDto` and back via `ToDomain`
- **THEN** every cell in the restored brain SHALL match the original brain

#### Scenario: Round-trip preserves current player
- **WHEN** a GameBrain with CurrentPlayer=1 is mapped to DTO and back
- **THEN** the restored brain's CurrentPlayer SHALL be 1

#### Scenario: Round-trip preserves game-over state
- **WHEN** a GameBrain with a winning position is mapped to DTO and back
- **THEN** the restored brain's IsGameOver and Winner SHALL match the original

### Requirement: Board array conversion
The mapper SHALL convert between Domain's 2D array (`int?[,]`) and DTO's jagged array (`int?[][]`) correctly.

#### Scenario: 2D to jagged conversion
- **WHEN** `ToDto` converts a 6x7 board
- **THEN** the jagged array SHALL have 6 rows, each with 7 columns, with matching values

#### Scenario: Jagged to 2D conversion
- **WHEN** `ToDomain` converts a jagged array with 6 rows of 7 columns
- **THEN** the resulting GameBrain's board SHALL have 6 rows and 7 columns with matching values
