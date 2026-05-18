## ADDED Requirements

### Requirement: AddEfPersistence registers EF repositories
`AddEfPersistence(connectionString)` SHALL register `AppDbContext`, `EfConfigRepository` as `IConfigRepository`, and `EfGameRepository` as `IGameRepository` in the service collection.

#### Scenario: EF repositories resolve from DI
- **WHEN** `AddEfPersistence("Data Source=test.db")` is called on a ServiceCollection and a provider is built
- **THEN** `IConfigRepository` resolves to `EfConfigRepository` and `IGameRepository` resolves to `EfGameRepository`

### Requirement: Database schema created on registration
`AddEfPersistence` SHALL ensure the database and schema exist when the context is first used (via `EnsureCreated`).

#### Scenario: New database is initialized
- **WHEN** `AddEfPersistence` is called with a connection string pointing to a non-existent database file
- **THEN** the database file is created and repositories function without manual setup

### Requirement: Connection string configures the database location
The connection string passed to `AddEfPersistence` SHALL control which SQLite database file is used.

#### Scenario: Custom path used
- **WHEN** `AddEfPersistence("Data Source=/tmp/mygame.db")` is called
- **THEN** the SQLite database is created at `/tmp/mygame.db`

### Requirement: Swappable with JSON persistence
`AddEfPersistence` and `AddJsonPersistence` SHALL be interchangeable — the application behaves identically regardless of which is called, differing only in storage backend.

#### Scenario: Same operations work after swap
- **WHEN** `AddEfPersistence` replaces `AddJsonPersistence` in Program.cs
- **THEN** save, load, list, and delete operations on configs and game states work identically
