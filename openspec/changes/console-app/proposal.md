## Why

Phases 0–3 built the game engine, AI, and JSON persistence — but there is no way for a user to actually play the game. The ConsoleApp project is a placeholder (`Hello, World!`). A console UI is needed to wire all layers together into a playable application with menu navigation, board rendering, game modes, and save/load support.

## What Changes

- Build a reusable console menu system with nested submenus, cursor-based selection, and updateable labels
- Implement ASCII board rendering with column selection input and game-over display
- Create a game controller that orchestrates the menu tree, game loop (Human vs Human, Human vs AI, AI vs AI), and persistence operations (save/load/delete for both game states and configurations)
- Wire DI composition root in `Program.cs` with `AddJsonPersistence()` and seed default configurations on first launch
- Ship four precreated configuration presets: Classical, Connect-3, Connect-5, Connect-4 Cylinder

## Capabilities

### New Capabilities
- `console-menu`: Reusable menu system with unlimited nesting, cursor-based selection, number key shortcuts, updateable labels, and level-aware navigation items
- `board-rendering`: ASCII board display, arrow-key column selection, save/quit hotkeys, and game-over presentation
- `game-orchestration`: Game controller coordinating menus, game loop (all three player-mode combinations), AI turn integration, and persistence operations (save/load/delete for configs and game states)
- `app-bootstrap`: DI composition root, default config seeding, and entry-point wiring

### Modified Capabilities

_None — this change adds a new presentation layer without modifying existing Domain, Application, or Infrastructure behavior._

## Impact

- **ConsoleApp project**: Complete rewrite of `Program.cs`; new files under `UI/` and root controller
- **Dependencies**: ConsoleApp already references Application + Infrastructure (no new project references needed)
- **Infrastructure**: JSON persistence exercised via repository interfaces — no changes to implementation
- **Domain/Application**: Consumed read-only through existing public APIs — no modifications
