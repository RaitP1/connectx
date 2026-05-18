## ADDED Requirements

### Requirement: Config repository interface
The system SHALL define an `IConfigRepository` interface in `Application.Config.Interfaces` with methods for listing, saving, loading, and deleting game configurations.

#### Scenario: Interface defines List method
- **WHEN** `IConfigRepository` is inspected
- **THEN** it SHALL declare a `List()` method returning `IReadOnlyList<GameConfig>`

#### Scenario: Interface defines Save method
- **WHEN** `IConfigRepository` is inspected
- **THEN** it SHALL declare a `Save(GameConfig config)` method

#### Scenario: Interface defines Load method
- **WHEN** `IConfigRepository` is inspected
- **THEN** it SHALL declare a `Load(string name)` method returning `GameConfig?`

#### Scenario: Interface defines Delete method
- **WHEN** `IConfigRepository` is inspected
- **THEN** it SHALL declare a `Delete(string name)` method

### Requirement: Config repository uses player 1 name as identifier
The system SHALL use the config's `Player1Name` property as the logical identifier for save and load operations. The repository implementation determines how the name maps to storage.

#### Scenario: Save uses config identity
- **WHEN** `Save(config)` is called
- **THEN** the config SHALL be persisted and retrievable by its identifying name

#### Scenario: Load returns null for missing config
- **WHEN** `Load(name)` is called with a name that has no saved config
- **THEN** it SHALL return null
