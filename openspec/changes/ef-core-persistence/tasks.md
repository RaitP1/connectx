## 1. Entity Types and DbContext

- [ ] 1.1 Create `ConfigEntity` class with properties matching all `GameConfig` fields, including nested `PlayerType` properties
- [ ] 1.2 Create `GameStateEntity` class with properties matching all `GameStateDto` fields, including nested `GameConfig` and `Board` as string
- [ ] 1.3 Create `AppDbContext` with `DbSet<ConfigEntity>` and `DbSet<GameStateEntity>`, configure entity mappings in `OnModelCreating` (PlayerType as owned, enums as strings, Board as JSON value converter, Name as primary key)

## 2. EF Config Repository

- [ ] 2.1 Implement `EfConfigRepository` fulfilling `IConfigRepository` — `Save` upserts, `Load` returns null if not found, `List` returns all, `Delete` is a no-op if missing
- [ ] 2.2 Add mapping methods between `ConfigEntity` and `GameConfig` domain record

## 3. EF Game Repository

- [ ] 3.1 Implement `EfGameRepository` fulfilling `IGameRepository` — `Save` upserts, `Load` returns null if not found, `List` returns all, `Delete` is a no-op if missing
- [ ] 3.2 Add mapping methods between `GameStateEntity` and `GameStateDto`, including Board serialization and embedded Config conversion

## 4. DI Registration

- [ ] 4.1 Add `AddEfPersistence(connectionString)` extension method that registers `AppDbContext`, `EfConfigRepository`, and `EfGameRepository`, and calls `EnsureCreated`
- [ ] 4.2 Update `ConsoleApp/Program.cs` with a commented-out `AddEfPersistence` call showing how to swap backends

## 5. Integration Tests

- [ ] 5.1 Write EF config repository integration tests: save/load/list/delete round-trips, upsert behavior, complex field round-trip (Cylinder topology, AI players)
- [ ] 5.2 Write EF game repository integration tests: save/load/list/delete round-trips, board data round-trip, embedded config round-trip, SavedAt timestamp preservation
- [ ] 5.3 Write DI registration test: verify `AddEfPersistence` resolves both repositories correctly
