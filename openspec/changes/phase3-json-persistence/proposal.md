## Why

The Application layer defines `IConfigRepository` and `IGameRepository` interfaces but no implementation exists yet. Without a persistence backend, games and configurations cannot be saved or loaded. JSON file persistence is the first backend — simple, human-readable, and requires no database setup.

## What Changes

- Add `FilesystemHelper` to resolve and ensure storage directories (`~/ConnectX/config/`, `~/ConnectX/savegames/`)
- Implement `JsonConfigRepository` fulfilling `IConfigRepository` — serialize configs as `{name}.json` using `System.Text.Json`
- Implement `JsonGameRepository` fulfilling `IGameRepository` — serialize game states as `{name}_{timestamp}.json`
- Add filename sanitization and path traversal protection on all file operations
- Add `ServiceCollectionExtensions.AddJsonPersistence()` in Infrastructure to register both repositories
- Add integration tests for full CRUD round-trips and security edge cases

## Capabilities

### New Capabilities
- `json-config-persistence`: Save, load, list, and delete game configurations as JSON files with filename sanitization
- `json-game-persistence`: Save, load, list, and delete game states as JSON files with filename sanitization
- `filesystem-storage`: Resolve and manage storage directories for JSON persistence
- `json-persistence-di`: Register JSON persistence implementations via DI extension method

### Modified Capabilities
_(none — existing interfaces are unchanged; this change provides implementations)_

## Impact

- **Infrastructure layer**: New files under `Persistence/Json/`
- **Dependencies**: Uses `System.Text.Json` (built-in, no new NuGet packages)
- **Filesystem**: Creates directories under `~/ConnectX/` at runtime
- **DI**: New `AddJsonPersistence()` extension method available for composition roots
- **Tests project**: New integration tests exercising file I/O with temp directories
