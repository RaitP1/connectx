## Why

The application currently supports only file-based JSON persistence. Adding a second persistence backend (SQLite via Entity Framework Core) proves the repository abstraction works and gives users a production-grade storage option with ACID transactions, concurrent access safety, and queryable data — all swappable with a single DI registration change.

## What Changes

- Add `AppDbContext` with `DbSet<ConfigEntity>` and `DbSet<GameStateEntity>` for EF Core SQLite storage
- Add EF entity types that map domain records to relational tables, with value converters for complex types (Board as JSON column, PlayerType as owned entities, enums as strings)
- Implement `EfConfigRepository` fulfilling the existing `IConfigRepository` contract
- Implement `EfGameRepository` fulfilling the existing `IGameRepository` contract
- Add `AddEfPersistence(connectionString)` extension method to Infrastructure DI registration
- Generate EF Core migration for the initial schema
- Update `ConsoleApp` to allow switching between JSON and EF persistence via a one-line DI change

## Capabilities

### New Capabilities

- `ef-db-context`: AppDbContext configuration, entity mappings, value converters, and migration
- `ef-config-persistence`: EF Core implementation of IConfigRepository backed by SQLite
- `ef-game-persistence`: EF Core implementation of IGameRepository backed by SQLite
- `ef-persistence-di`: DI registration extension method for wiring EF persistence

### Modified Capabilities

- `app-bootstrap`: ConsoleApp Program.cs gains the ability to swap between `AddJsonPersistence()` and `AddEfPersistence()` — no behavioral change, just a new wiring option

## Impact

- **Infrastructure project**: New `Persistence/EF/` directory with DbContext, entities, repositories, and migrations
- **Infrastructure DI**: New `AddEfPersistence()` alongside existing `AddJsonPersistence()`
- **ConsoleApp**: One-line change in Program.cs to switch backends (existing `AddJsonPersistence()` call can be swapped)
- **NuGet packages**: Already present — `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design` are in Infrastructure.csproj
- **Tests**: New integration tests verifying EF repositories pass the same behavioral contracts as JSON repositories
