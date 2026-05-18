## ADDED Requirements

### Requirement: EfGameRepository implements IGameRepository
`EfGameRepository` SHALL implement the `IGameRepository` interface using EF Core and `AppDbContext`.

#### Scenario: Implements the interface
- **WHEN** `EfGameRepository` is resolved from the DI container
- **THEN** it is assignable to `IGameRepository`

### Requirement: Save persists a GameStateDto
`Save(GameStateDto)` SHALL persist the game state to the database. If a state with the same Name already exists, it SHALL be overwritten.

#### Scenario: Save a new game state
- **WHEN** `Save` is called with a GameStateDto named "Game1"
- **THEN** `Load("Game1")` returns a state with all fields matching the saved state

#### Scenario: Save overwrites existing game state
- **WHEN** `Save` is called with name "Game1" and CurrentPlayer=1, then called again with name "Game1" and CurrentPlayer=2
- **THEN** `Load("Game1")` returns a state with CurrentPlayer=2

### Requirement: Load retrieves a game state by name
`Load(name)` SHALL return the matching `GameStateDto` or null if not found.

#### Scenario: Load existing game state
- **WHEN** a game state named "SavedGame" has been saved
- **THEN** `Load("SavedGame")` returns the state with all original field values

#### Scenario: Load non-existent game state
- **WHEN** no game state named "Missing" exists
- **THEN** `Load("Missing")` returns null

### Requirement: List returns all saved game states
`List()` SHALL return all persisted game states as a read-only list.

#### Scenario: List with multiple states
- **WHEN** three game states named "A", "B", "C" have been saved
- **THEN** `List()` returns a collection containing all three states

#### Scenario: List with no states
- **WHEN** no game states exist
- **THEN** `List()` returns an empty collection

### Requirement: Delete removes a game state by name
`Delete(name)` SHALL remove the game state from the database. If the name does not exist, the operation SHALL be a no-op.

#### Scenario: Delete existing game state
- **WHEN** a game state named "OldGame" exists and `Delete("OldGame")` is called
- **THEN** `Load("OldGame")` returns null

#### Scenario: Delete non-existent game state
- **WHEN** no game state named "Ghost" exists and `Delete("Ghost")` is called
- **THEN** no error is thrown

### Requirement: Board data survives round-trip
The Board (`int?[][]`) with a mix of null, player 1, and player 2 cell values SHALL be preserved exactly through save/load.

#### Scenario: Board with moves round-trips
- **WHEN** a game state with a 6x7 board containing 5 moves (mix of player 1 and player 2) is saved and loaded
- **THEN** every cell value matches the original board exactly

### Requirement: Embedded config survives round-trip
The `GameConfig` nested inside `GameStateDto` SHALL be preserved exactly through save/load, including all fields (topology, player types, dimensions).

#### Scenario: Config within game state round-trips
- **WHEN** a game state whose Config has Cylinder topology and AI players is saved and loaded
- **THEN** the loaded Config matches the original on every field

### Requirement: SavedAt timestamp preserved
The `SavedAt` DateTime SHALL be preserved through save/load.

#### Scenario: Timestamp round-trip
- **WHEN** a game state with a specific SavedAt value is saved and loaded
- **THEN** the loaded SavedAt matches the original (to second precision)
