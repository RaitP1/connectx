## ADDED Requirements

### Requirement: JSON game repository implements IGameRepository
The system SHALL provide `JsonGameRepository` in `Infrastructure.Persistence.Json` that implements `IGameRepository` using JSON file storage.

#### Scenario: Repository is registered as IGameRepository
- **WHEN** `AddJsonPersistence()` is called on the service collection
- **THEN** `JsonGameRepository` SHALL be resolvable as `IGameRepository`

### Requirement: Save game state as JSON file
The system SHALL serialize a `GameStateDto` to a JSON file named `{sanitized-name}.json` in the savegames storage directory.

#### Scenario: Save new game state
- **WHEN** `Save(state)` is called with a state that has no existing file
- **THEN** a new JSON file SHALL be created with the serialized game state

#### Scenario: Save overwrites existing game state
- **WHEN** `Save(state)` is called with a state whose name matches an existing file
- **THEN** the existing file SHALL be overwritten with the new state

#### Scenario: Serialization format
- **WHEN** a game state is saved
- **THEN** the JSON SHALL be indented for human readability and enum values SHALL be serialized as strings

### Requirement: Load game state from JSON file
The system SHALL deserialize a `GameStateDto` from the named JSON file in the savegames storage directory.

#### Scenario: Load existing game state
- **WHEN** `Load(name)` is called with a name matching an existing file
- **THEN** the deserialized `GameStateDto` SHALL be returned

#### Scenario: Load non-existent game state
- **WHEN** `Load(name)` is called with a name that has no matching file
- **THEN** `null` SHALL be returned

### Requirement: List all saved game states
The system SHALL return all saved game states by reading all `.json` files in the savegames storage directory.

#### Scenario: List with saved games
- **WHEN** `List()` is called and game state files exist
- **THEN** all states SHALL be deserialized and returned as `IReadOnlyList<GameStateDto>`

#### Scenario: List with empty directory
- **WHEN** `List()` is called and no game state files exist
- **THEN** an empty list SHALL be returned

### Requirement: Delete saved game state
The system SHALL delete the JSON file for the named game state.

#### Scenario: Delete existing game state
- **WHEN** `Delete(name)` is called with a name matching an existing file
- **THEN** the file SHALL be removed from the savegames directory

#### Scenario: Delete non-existent game state
- **WHEN** `Delete(name)` is called with a name that has no matching file
- **THEN** the operation SHALL complete without error

### Requirement: Game state round-trip fidelity
A game state saved and then loaded SHALL be identical to the original.

#### Scenario: Round-trip preserves board state
- **WHEN** a `GameStateDto` with a partially filled board is saved and loaded
- **THEN** the board array SHALL match cell-by-cell, preserving null (empty) and player values

#### Scenario: Round-trip preserves config within game state
- **WHEN** a `GameStateDto` containing a `GameConfig` with cylinder topology is saved and loaded
- **THEN** the embedded config SHALL match all fields including topology and player types

#### Scenario: Round-trip preserves current player and timestamp
- **WHEN** a `GameStateDto` is saved and loaded
- **THEN** `CurrentPlayer` and `SavedAt` SHALL match the original values
