## ADDED Requirements

### Requirement: GameStateDto properties
The system SHALL define a `GameStateDto` record in `Application.Game.Dto` with properties: Name (string), Config (GameConfig), Board (int?[][]), CurrentPlayer (int), and SavedAt (DateTime).

#### Scenario: DTO contains all required properties
- **WHEN** `GameStateDto` is inspected
- **THEN** it SHALL have properties Name, Config, Board, CurrentPlayer, and SavedAt with the specified types

### Requirement: Board stored as jagged array
The system SHALL represent the game board in `GameStateDto` as a jagged array (`int?[][]`) where the outer array represents rows and inner arrays represent columns.

#### Scenario: Board dimensions match config
- **WHEN** a `GameStateDto` is created from a game with 6 rows and 7 columns
- **THEN** `Board` SHALL have 6 elements (rows), each with 7 elements (columns)

#### Scenario: Board preserves cell values
- **WHEN** a `GameStateDto` is created from a game where player 0 placed at row 5, col 3
- **THEN** `Board[5][3]` SHALL be 0

#### Scenario: Empty cells are null
- **WHEN** a `GameStateDto` is created from a game with unoccupied cells
- **THEN** those cells SHALL be null in the jagged array

### Requirement: GameStateDto is immutable
The system SHALL define `GameStateDto` as an immutable record type.

#### Scenario: DTO is a record
- **WHEN** `GameStateDto` is inspected via reflection
- **THEN** it SHALL be a C# record type with init-only properties
