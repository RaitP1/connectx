## Context

The Application layer defines `IConfigRepository` and `IGameRepository` interfaces with CRUD operations (`List`, `Save`, `Load`, `Delete`). The Infrastructure layer is empty — no implementations exist. The Domain layer uses immutable records (`GameConfig`, `PlayerType`) and the Application layer provides `GameStateDto` as the serialization boundary type. All types are `System.Text.Json`-friendly by default (records with public properties).

## Goals / Non-Goals

**Goals:**
- Implement file-based JSON persistence for both repository interfaces
- Protect against path traversal and filename injection attacks
- Make persistence swappable via a single DI extension method call
- Provide integration tests proving round-trip correctness

**Non-Goals:**
- EF Core / SQLite persistence (Phase 5)
- Concurrent write safety (single-user console app for now)
- File locking or atomic writes
- Encryption of saved data

## Decisions

### 1. Storage directory layout: `~/ConnectX/config/` and `~/ConnectX/savegames/`

Separate directories by entity type. Using `~` (user home) keeps saves portable and discoverable. The helper resolves `~` via `Environment.GetFolderPath(SpecialFolder.UserProfile)` and creates directories on first access.

**Alternative considered**: Single directory with prefixes — rejected because it complicates listing operations and mixes concerns.

### 2. Filename strategy

- Configs: `{sanitized-name}.json` — name derived from config identity (the config's name field passed to Save/Load/Delete)
- Game states: `{sanitized-name}.json` — name from `GameStateDto.Name`

Sanitization strips all characters except `[a-zA-Z0-9_-]` and enforces a maximum filename length. Path traversal sequences (`..`, `/`, `\`) are removed before any filesystem operation. The resolved path is validated to stay within the allowed directory.

**Alternative considered**: GUID-based filenames — rejected because human-readable names improve debuggability and are a course requirement.

### 3. Serialization: `System.Text.Json` with default options

Both `GameConfig` and `GameStateDto` are records with public properties — they serialize cleanly without custom converters. Use `JsonSerializerOptions` with `WriteIndented = true` for human readability and `PropertyNameCaseInsensitive = true` for robust deserialization. Enum values serialize as strings via `JsonStringEnumConverter`.

**Alternative considered**: `Newtonsoft.Json` — rejected because `System.Text.Json` is built-in and sufficient.

### 4. DI registration via `AddJsonPersistence()` extension method

A single `IServiceCollection.AddJsonPersistence()` method in Infrastructure registers both `JsonConfigRepository` and `JsonGameRepository` as the implementations for their respective interfaces. This mirrors the pattern already established by `AddApplicationServices()` in Application.

### 5. Config identity: use the `Name` property on config

The `IConfigRepository` interface uses `string name` for Load/Delete. The config must have a name to persist under. Since `GameConfig` doesn't currently have a `Name` property, and looking at the roadmap and interface signatures, the `Save(GameConfig)` method needs a way to derive the filename. The interface's `Load(string name)` and `Delete(string name)` suggest the name is external to the config record. The repository will accept a config and derive the filename from a name parameter — but since `Save` only takes `GameConfig`, the implementation will need a naming convention. Looking at the interface, configs are identified by name at load/delete time. For saving, we need to add a `Name` property to `GameConfig`, or use an existing field. Given the interface contract `Save(GameConfig config)`, the implementation will use a composite key or a convention. Reviewing the existing code, the simplest approach: add a `Name` property to `GameConfig` in Domain, since config identity is a domain concept.

**Decision**: Propose adding `string Name` as the first parameter of the `GameConfig` record. This is a Domain change required to support persistence identity.

## Risks / Trade-offs

- **[Risk] Directory creation fails (permissions)** → Throw with a clear message explaining which directory couldn't be created. The user can fix OS-level permissions.
- **[Risk] Filename collisions on save** → Overwrite existing file with same name. This is intentional — "save" means "upsert".
- **[Risk] Corrupt JSON files** → Let deserialization exceptions propagate. The caller handles missing/corrupt saves.
- **[Trade-off] No file locking** → Acceptable for single-user console app. Would need revisiting for WebApp concurrent access (Phase 6 will use EF Core instead).
- **[Trade-off] GameConfig needs a Name property** → Small Domain change, but necessary for persistence identity. All existing code constructs GameConfig — callers will need updating.
