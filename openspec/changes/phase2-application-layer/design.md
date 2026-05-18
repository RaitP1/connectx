## Context

Phase 0 established the solution skeleton with architecture tests. Phase 1 built the Domain game engine (`GameBrain`, `GameConfig`, enums, value types) and Application-layer AI (`MinimaxAI`, `BoardEvaluator`, `IAIPlayer`). The Application layer currently has no persistence contracts, DTOs, or mapping. Infrastructure is empty (marker class only). Phases 3-6 (JSON persistence, console UI, EF persistence, web app) all need repository interfaces and a DTO/mapper to serialize game state.

The Domain uses a 2D array (`int?[,]`) for board state, which is not JSON-friendly. The Application layer must define a serializable DTO with a jagged array (`int?[][]`) and handle the conversion.

## Goals / Non-Goals

**Goals:**
- Define repository interfaces (ports) that Infrastructure will implement (adapters)
- Define a serialization-friendly DTO for game state snapshots
- Provide bidirectional mapping between `GameBrain` and `GameStateDto`
- Register Application services via a DI extension method
- Maintain 80%+ test coverage for mapper logic

**Non-Goals:**
- Repository implementations (Phase 3 — JSON, Phase 5 — EF Core)
- Console or web UI (Phases 4, 6)
- Modifying Domain types or AI logic
- Game session management (Phase 6 — WebApp)
- Config DTOs — `GameConfig` is already a record and serializes cleanly as-is

## Decisions

**GameStateDto uses `int?[][]` jagged array for board**
- `System.Text.Json` and EF Core JSON columns both handle jagged arrays natively
- Domain's 2D `int?[,]` requires explicit conversion — the mapper handles this
- Alternative: flatten to `int?[]` with row-major indexing — rejected; jagged array preserves the visual row/column structure and is more debuggable in JSON

**GameConfig embedded directly in GameStateDto (no separate config DTO)**
- `GameConfig` is already an immutable record with simple value types
- It serializes cleanly without transformation
- Alternative: create a `GameConfigDto` wrapper — rejected; adds a mapping layer with zero benefit since the record already works

**Repository interfaces return domain types, not DTOs**
- `IConfigRepository` works with `GameConfig` directly (already serializable)
- `IGameRepository` works with `GameStateDto` (the Application-layer boundary type)
- The mapper sits between `GameBrain` and `GameStateDto` — callers choose when to map
- Alternative: repositories return domain types with internal mapping — rejected; it couples Infrastructure to Domain mapping logic

**Repository interfaces use `string` identifiers (names), not GUIDs**
- Game configs and saves are user-named ("Classical", "my-save-1")
- File-based persistence (Phase 3) maps names directly to filenames
- EF persistence (Phase 5) can use the name as a natural key
- Alternative: GUID keys — rejected; adds indirection with no benefit for a single-user game

**ServiceCollectionExtensions registers only the mapper**
- AI (`MinimaxAI`) is created with a difficulty parameter per-game, not DI-resolved
- Repository implementations are registered by Infrastructure's own extension methods
- Alternative: register `IAIPlayer` via DI — rejected; difficulty is a runtime choice, not a composition-root decision

**GameBrain reconstruction via public constructor + MakeMove replay**
- `GameStateDto` stores the board state and current player
- `ToDomain` creates a new `GameBrain` from config, then uses internal state restoration
- `GameBrain` needs a new constructor overload: `GameBrain(GameConfig config, int?[,] board, int currentPlayer)` — made internal to Domain
- Alternative: replay all moves from history — rejected; move history isn't stored in the DTO, and replaying would re-trigger win checks unnecessarily
- Alternative: reflection to set private fields — rejected; brittle and violates encapsulation

**GameBrain needs an internal restoration constructor**
- The existing private constructor `GameBrain(config, board, currentPlayer, history)` is used by `Clone()`
- The mapper needs similar access but from a different assembly
- Expose as `internal` with `[InternalsVisibleTo("Application")]` on Domain assembly
- Move history stack will be empty on restored games (undo not available after load) — this is acceptable behavior
- Alternative: add a public factory method — rejected; factory method would expose internal state manipulation as public API

## Risks / Trade-offs

**[Risk] Undo unavailable after load** — Move history is not persisted in the DTO, so `UndoMove()` returns false on loaded games. This is acceptable for Phase 2; move history persistence can be added later if needed without changing interfaces.

**[Risk] InternalsVisibleTo couples Domain and Application assemblies at the CLR level** — This is a pragmatic trade-off. The architecture tests already enforce the dependency direction. The alternative (public restoration constructor) would be worse because it exposes internal state manipulation to all consumers.

**[Trade-off] GameConfig used directly instead of DTO** — If `GameConfig` ever gains non-serializable members, a DTO will be needed. Current record shape is clean, so this is deferred.
