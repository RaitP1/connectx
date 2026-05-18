## MODIFIED Requirements

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
