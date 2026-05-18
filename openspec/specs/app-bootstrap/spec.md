## ADDED Requirements

### Requirement: DI composition root
`Program.cs` SHALL build a `ServiceCollection`, register all required services, and resolve the entry-point controller to run the application. The persistence backend SHALL be selectable by calling either `AddJsonPersistence()` or `AddEfPersistence(connectionString)`.

#### Scenario: Application starts with all services registered
- **WHEN** the application launches
- **THEN** the chosen persistence method is called, ConsoleApp services are registered, and the game controller is resolved from the service provider

#### Scenario: Application starts with EF persistence
- **WHEN** the application launches with `AddEfPersistence` configured
- **THEN** `IConfigRepository` and `IGameRepository` resolve to EF implementations and all operations function correctly

#### Scenario: Application starts with JSON persistence
- **WHEN** the application launches with `AddJsonPersistence` configured
- **THEN** `IConfigRepository` and `IGameRepository` resolve to JSON implementations and all operations function correctly

### Requirement: Seed default configurations on first launch
On startup, if `IConfigRepository.List()` returns an empty list, the system SHALL seed four precreated configurations.

#### Scenario: First launch seeds presets
- **WHEN** the application starts and no configurations exist
- **THEN** four presets are created: "Classical" (7x6, connect 4, rectangle), "Connect-3" (5x4, connect 3, rectangle), "Connect-5" (9x7, connect 5, rectangle), "Connect-4 Cylinder" (7x6, connect 4, cylinder)

#### Scenario: Subsequent launches skip seeding
- **WHEN** the application starts and configurations already exist
- **THEN** no additional presets are created

### Requirement: Default player configuration for presets
All precreated configurations SHALL use Human for both players with default names ("Player 1", "Player 2") and symbols ("X", "O").

#### Scenario: Preset player defaults
- **WHEN** the "Classical" preset is loaded
- **THEN** Player 1 is Human named "Player 1" with symbol "X" and Player 2 is Human named "Player 2" with symbol "O"

### Requirement: Menu tree constructed at startup
The entry point SHALL build the complete menu tree and enter the menu loop.

#### Scenario: Application presents main menu
- **WHEN** the application starts and seeding is complete
- **THEN** the main menu is displayed with options for New Game, Load Game, Settings, How to Play, and Exit
