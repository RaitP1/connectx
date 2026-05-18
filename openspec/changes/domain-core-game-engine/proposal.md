## Why

The Connect-X solution skeleton exists (Phase 0 complete) but contains no game logic. The Domain layer is empty — just a marker class. Without a game engine, no other layer (Application, Infrastructure, ConsoleApp, WebApp) can deliver functionality. Phase 1 builds the pure game engine and AI that everything else depends on.

## What Changes

- Add board topology support (rectangle and cylinder wrap-around)
- Add game configuration with validation (dimensions, win condition, player types, topology)
- Add the core game engine: gravity-based piece dropping, turn management, win/draw detection
- Add cylinder topology column wrapping for board access and win detection
- Add AI opponent using minimax with alpha-beta pruning at three difficulty levels
- Add undo, clone, and position evaluation to support AI and future features

## Capabilities

### New Capabilities
- `game-configuration`: Board dimensions, win condition, player names/symbols, topology, player types, and validation
- `game-engine`: Gravity drop, turn management, board state, win detection (4-direction line scan), draw detection, cylinder wrap
- `ai-opponent`: Minimax with alpha-beta pruning, difficulty levels (Easy/Medium/Hard) controlling search depth

### Modified Capabilities

## Impact

- `src/Domain/` — all new files live here; zero external dependencies
- `tests/` — new unit tests for game logic, configuration validation, and AI
- No changes to Application, Infrastructure, or ConsoleApp projects
- No new NuGet packages required (Domain has zero dependencies)
