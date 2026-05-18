## 1. Filesystem Helper

- [x] 1.1 Create `FilesystemHelper` class in `Infrastructure/Persistence/Json/` with methods to resolve config and savegames directory paths
- [x] 1.2 Implement filename sanitization (strip non `[a-zA-Z0-9_-]` chars, remove path traversal sequences, reject empty results)
- [x] 1.3 Implement path confinement validation (resolved path must stay within allowed directory)
- [x] 1.4 Write integration tests for directory resolution, sanitization, and path confinement

## 2. JSON Config Repository

- [x] 2.1 Create `JsonConfigRepository` implementing `IConfigRepository` in `Infrastructure/Persistence/Json/`
- [x] 2.2 Implement `Save` — serialize `GameConfig` to `{sanitized-name}.json` with indented JSON and string enums
- [x] 2.3 Implement `Load` — deserialize from file, return null if not found
- [x] 2.4 Implement `List` — read all `.json` files in config directory, deserialize each
- [x] 2.5 Implement `Delete` — remove file, no-op if missing
- [x] 2.6 Write integration tests: save/load round-trip, list, delete, round-trip fidelity for enums and all fields

## 3. JSON Game Repository

- [x] 3.1 Create `JsonGameRepository` implementing `IGameRepository` in `Infrastructure/Persistence/Json/`
- [x] 3.2 Implement `Save` — serialize `GameStateDto` to `{sanitized-name}.json`
- [x] 3.3 Implement `Load` — deserialize from file, return null if not found
- [x] 3.4 Implement `List` — read all `.json` files in savegames directory, deserialize each
- [x] 3.5 Implement `Delete` — remove file, no-op if missing
- [x] 3.6 Write integration tests: save/load round-trip, list, delete, board fidelity, config-within-state fidelity, current player and timestamp preservation

## 4. DI Registration

- [x] 4.1 Create `ServiceCollectionExtensions` in Infrastructure with `AddJsonPersistence()` extension method
- [x] 4.2 Register `JsonConfigRepository` as singleton `IConfigRepository` and `JsonGameRepository` as singleton `IGameRepository`
- [x] 4.3 Write integration test verifying both repositories resolve from service provider after `AddJsonPersistence()`

## 5. Filename Security Tests

- [x] 5.1 Write integration tests for filename sanitization edge cases (special characters stripped, path traversal blocked, empty-after-sanitization throws)
- [x] 5.2 Write integration tests for path confinement (crafted inputs that attempt directory escape)
