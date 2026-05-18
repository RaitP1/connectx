## MODIFIED Requirements

### Requirement: DI composition root
`Program.cs` SHALL build a `ServiceCollection`, register all required services, and resolve the entry-point controller to run the application. The persistence backend SHALL default to `AddEfPersistence(connectionString)` to enable cross-app continuity. `AddJsonPersistence()` SHALL remain available as an alternative.

#### Scenario: Application starts with all services registered
- **WHEN** the application launches
- **THEN** EF persistence is configured by default, ConsoleApp services are registered, and the game controller is resolved from the service provider

#### Scenario: Application starts with EF persistence
- **WHEN** the application launches with `AddEfPersistence` configured
- **THEN** `IConfigRepository` and `IGameRepository` resolve to EF implementations and all operations function correctly

#### Scenario: Application starts with JSON persistence
- **WHEN** the application launches with `AddJsonPersistence` configured instead of EF
- **THEN** `IConfigRepository` and `IGameRepository` resolve to JSON implementations and all operations function correctly

#### Scenario: Database created on first EF launch
- **WHEN** the console app starts with EF persistence and no database exists
- **THEN** the database SHALL be created via `EnsureCreated()`
