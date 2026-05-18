## Context

The application uses `IConfigRepository` and `IGameRepository` interfaces (Application layer) with a single JSON file-based implementation (Infrastructure layer). All domain models are immutable sealed records. The repository API is synchronous. EF Core SQLite packages are already referenced in Infrastructure.csproj.

## Goals / Non-Goals

**Goals:**
- Implement a second persistence backend using EF Core + SQLite behind the same repository interfaces
- Prove the repository abstraction is swappable — switching backends is a one-line DI change
- Store complex domain types (jagged arrays, enums, value objects) correctly in a relational schema
- Maintain the same synchronous repository contract

**Non-Goals:**
- Async repository interfaces — the existing contract is synchronous, and changing it would be a cross-cutting change across all consumers
- Database migrations CLI tooling in ConsoleApp — migrations are applied via `EnsureCreated()` at registration time for simplicity
- Multi-provider support at runtime (e.g., JSON + EF simultaneously) — only one backend is active per app run
- WebApp integration — that belongs to Phase 6

## Decisions

### Entity types separate from domain records
EF Core needs mutable entity classes with settable properties for materialization. The domain uses immutable sealed records. Rather than polluting domain types with EF attributes or compromise immutability, introduce dedicated entity types (`ConfigEntity`, `GameStateEntity`) in Infrastructure and map between them in the repositories.

*Alternative considered*: Configure EF to map domain records directly using constructor binding. Rejected because `int?[][]` Board and nested `PlayerType`/`GameConfig` inside `GameStateDto` would require fragile shadow properties and complex value converters that are hard to maintain.

### Board serialized as JSON column
The game board (`int?[][]`) does not have a natural relational representation. Storing it as a JSON string column with a value converter is the simplest approach — no junction tables, no fixed-width column arrays.

*Alternative considered*: Normalize board into a `Cell(Row, Col, Value)` table. Rejected — adds query complexity for no benefit since the board is always loaded/saved as a whole.

### PlayerType as owned entities
`PlayerType` (IsAI + Difficulty) maps naturally to owned entity types — columns live inline on the parent table (e.g., `Player1Type_IsAI`, `Player1Type_Difficulty`). This avoids a separate table while preserving the structure.

### Enums stored as strings
Consistent with the JSON persistence backend. Uses `HasConversion<string>()` so database values are human-readable and resilient to enum reordering.

### EnsureCreated for schema setup
For a local-only game app, `EnsureCreated()` called at DI registration time is sufficient. No migration history table, no `dotnet ef` tooling required at runtime.

*Alternative considered*: Ship migrations and call `Database.Migrate()`. More correct for production apps, but adds complexity (migration files, design-time factory) for a local game with no schema evolution path yet.

### Scoped DbContext lifetime
`AppDbContext` is registered as scoped (EF Core default). The repositories are also scoped so they share a context per operation scope. ConsoleApp creates a scope per repository call.

## Risks / Trade-offs

- **[Synchronous EF calls]** → EF Core supports sync methods (`ToList()`, `SaveChanges()`). Performance is acceptable for a local game with small datasets. If the interface is made async later, the EF implementation benefits immediately.
- **[EnsureCreated vs Migrate]** → `EnsureCreated()` cannot apply incremental schema changes. If schema evolves, must drop and recreate or switch to migrations. Acceptable for Phase 5; revisit if schema changes in Phase 6.
- **[JSON column not queryable]** → Board data stored as JSON cannot be filtered in SQL. Not needed — boards are always loaded whole via `Load(name)`.
- **[Name as identifier]** → Both repositories use string `Name` as the primary key, consistent with JSON repos. Names must be unique per entity type.
