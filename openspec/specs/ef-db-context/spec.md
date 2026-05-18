## ADDED Requirements

### Requirement: AppDbContext exposes config and game state entity sets
`AppDbContext` SHALL expose `DbSet<ConfigEntity>` and `DbSet<GameStateEntity>` for EF Core to manage.

#### Scenario: DbContext has required entity sets
- **WHEN** `AppDbContext` is instantiated
- **THEN** it exposes a `DbSet<ConfigEntity>` and a `DbSet<GameStateEntity>`

### Requirement: ConfigEntity maps all GameConfig fields
`ConfigEntity` SHALL store all fields from `GameConfig`: Name, Rows, Columns, WinCondition, Player1Name, Player1Symbol, Player2Name, Player2Symbol, Topology, Player1Type, and Player2Type. Name SHALL be the primary key.

#### Scenario: ConfigEntity round-trips all GameConfig fields
- **WHEN** a `GameConfig` with all fields populated (including Cylinder topology and AI player types) is saved via the DbContext
- **THEN** loading the entity back produces values identical to the original for every field

#### Scenario: Name is the primary key
- **WHEN** two configs with the same Name are saved
- **THEN** the second save overwrites the first (upsert behavior)

### Requirement: GameStateEntity maps all GameStateDto fields
`GameStateEntity` SHALL store all fields from `GameStateDto`: Name, Config (as a nested structure), Board (as serialized data), CurrentPlayer, and SavedAt. Name SHALL be the primary key.

#### Scenario: GameStateEntity round-trips all GameStateDto fields
- **WHEN** a `GameStateDto` with a populated board, non-default config, and current player set to 2 is saved
- **THEN** loading the entity back produces values identical to the original for every field including Board cell values, CurrentPlayer, and SavedAt

### Requirement: Board stored as JSON column
The Board (`int?[][]`) SHALL be stored as a JSON string column using a value converter.

#### Scenario: Board with mixed values serializes correctly
- **WHEN** a board containing null cells, player 1 cells, and player 2 cells is saved and loaded
- **THEN** the loaded board has identical dimensions and cell values to the original

#### Scenario: Empty board serializes correctly
- **WHEN** a board with all null cells is saved and loaded
- **THEN** the loaded board has identical dimensions and all cells are null

### Requirement: PlayerType stored as owned entity
`PlayerType` fields (IsAI, Difficulty) SHALL be stored as owned entity columns inline on the parent table.

#### Scenario: Human player type round-trips
- **WHEN** a config with `PlayerType(IsAI: false, Difficulty: null)` is saved and loaded
- **THEN** the loaded PlayerType has IsAI=false and Difficulty=null

#### Scenario: AI player type round-trips
- **WHEN** a config with `PlayerType(IsAI: true, Difficulty: Hard)` is saved and loaded
- **THEN** the loaded PlayerType has IsAI=true and Difficulty=Hard

### Requirement: Enums stored as strings
`EBoardTopology` and `EAIDifficulty` enum values SHALL be stored as their string representation in the database.

#### Scenario: Enum values survive round-trip as strings
- **WHEN** a config with Topology=Cylinder and Player2 Difficulty=Medium is saved and loaded
- **THEN** the values are Cylinder and Medium respectively (not integer ordinals)

### Requirement: Schema created automatically
The database schema SHALL be created automatically when `AppDbContext` is first used if it does not already exist.

#### Scenario: Fresh database gets schema
- **WHEN** `AddEfPersistence` is called with a connection string pointing to a non-existent database
- **THEN** the database file and all required tables are created without error
