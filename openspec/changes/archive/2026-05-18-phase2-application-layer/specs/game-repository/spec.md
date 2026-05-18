## ADDED Requirements

### Requirement: Game repository interface
The system SHALL define an `IGameRepository` interface in `Application.Game.Interfaces` with methods for listing, saving, loading, and deleting saved game states.

#### Scenario: Interface defines List method
- **WHEN** `IGameRepository` is inspected
- **THEN** it SHALL declare a `List()` method returning `IReadOnlyList<GameStateDto>`

#### Scenario: Interface defines Save method
- **WHEN** `IGameRepository` is inspected
- **THEN** it SHALL declare a `Save(GameStateDto state)` method

#### Scenario: Interface defines Load method
- **WHEN** `IGameRepository` is inspected
- **THEN** it SHALL declare a `Load(string name)` method returning `GameStateDto?`

#### Scenario: Interface defines Delete method
- **WHEN** `IGameRepository` is inspected
- **THEN** it SHALL declare a `Delete(string name)` method

### Requirement: Game repository operates on GameStateDto
The system SHALL use `GameStateDto` as the boundary type for all game repository operations, not `GameBrain` directly.

#### Scenario: Repository does not depend on GameBrain
- **WHEN** `IGameRepository` is inspected
- **THEN** it SHALL not reference `GameBrain` in any method signature
