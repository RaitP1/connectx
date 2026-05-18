## Why

The Connect-X solution skeleton exists (Phase 0 complete) but contains no game logic. The Domain layer is empty — just a marker class. Without a game engine, no other layer (Application, Infrastructure, ConsoleApp, WebApp) can deliver functionality. Phase 1 builds the pure game engine and AI that everything else depends on.

## What Changes

- Add board topology support (rectangle and cylinder wrap-around)
- Add game configuration with validation (dimensions, win condition, player types, topology)
- Add the core game engine: gravity-based piece dropping, turn management, win/draw detection
- Add cylinder topology column wrapping for board access and win detection
- Add AI opponent using minimax with alpha-beta pruning at three difficulty levels (in Application layer)
- Add board position evaluation as an Application-layer service used by AI
- Add undo and clone to support AI and future features
- Apply `E` prefix convention to all enums (`EBoardTopology`, `EAIDifficulty`)

## Capabilities

### New Capabilities
- `game-configuration`: Board dimensions, win condition, player names/symbols, topology, player types, and validation
- `game-engine`: Gravity drop, turn management, board state, win detection (4-direction line scan), draw detection, cylinder wrap
- `ai-opponent`: Minimax with alpha-beta pruning, difficulty levels (Easy/Medium/Hard) controlling search depth — lives in Application/AI, not Domain

### Modified Capabilities

## Impact

- `src/Domain/` — game engine, configuration, enums, value types; zero external dependencies
- `src/Application/AI/` — IAIPlayer interface, MinimaxAI, BoardEvaluator; depends only on Domain
- `tests/` — unit tests for game logic, configuration validation, AI, and board evaluation
- No changes to Infrastructure or ConsoleApp projects
- No new NuGet packages required
