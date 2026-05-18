## ADDED Requirements

### Requirement: EfConfigRepository implements IConfigRepository
`EfConfigRepository` SHALL implement the `IConfigRepository` interface using EF Core and `AppDbContext`.

#### Scenario: Implements the interface
- **WHEN** `EfConfigRepository` is resolved from the DI container
- **THEN** it is assignable to `IConfigRepository`

### Requirement: Save persists a GameConfig
`Save(GameConfig)` SHALL persist the configuration to the database. If a config with the same Name already exists, it SHALL be overwritten.

#### Scenario: Save a new config
- **WHEN** `Save` is called with a valid GameConfig named "Test"
- **THEN** `Load("Test")` returns a config with all fields matching the saved config

#### Scenario: Save overwrites existing config
- **WHEN** `Save` is called with name "Test" and Rows=6, then called again with name "Test" and Rows=8
- **THEN** `Load("Test")` returns a config with Rows=8

### Requirement: Load retrieves a config by name
`Load(name)` SHALL return the matching `GameConfig` or null if not found.

#### Scenario: Load existing config
- **WHEN** a config named "Classic" has been saved
- **THEN** `Load("Classic")` returns the config with all original field values

#### Scenario: Load non-existent config
- **WHEN** no config named "Missing" exists
- **THEN** `Load("Missing")` returns null

### Requirement: List returns all saved configs
`List()` SHALL return all persisted configurations as a read-only list.

#### Scenario: List with multiple configs
- **WHEN** three configs named "A", "B", "C" have been saved
- **THEN** `List()` returns a collection containing all three configs

#### Scenario: List with no configs
- **WHEN** no configs exist
- **THEN** `List()` returns an empty collection

### Requirement: Delete removes a config by name
`Delete(name)` SHALL remove the config from the database. If the name does not exist, the operation SHALL be a no-op.

#### Scenario: Delete existing config
- **WHEN** a config named "Old" exists and `Delete("Old")` is called
- **THEN** `Load("Old")` returns null

#### Scenario: Delete non-existent config
- **WHEN** no config named "Ghost" exists and `Delete("Ghost")` is called
- **THEN** no error is thrown

### Requirement: Complex config fields round-trip correctly
Configs with non-default values for all fields (Cylinder topology, AI players with difficulty, custom names and symbols) SHALL survive a save/load cycle without data loss.

#### Scenario: Full config round-trip with Cylinder and AI
- **WHEN** a config with Topology=Cylinder, Player1Type=(IsAI=true, Difficulty=Easy), Player2Type=(IsAI=true, Difficulty=Hard), custom names "Alpha"/"Beta", symbols "A"/"B", Rows=9, Columns=7, WinCondition=5 is saved and loaded
- **THEN** every field matches the original exactly
