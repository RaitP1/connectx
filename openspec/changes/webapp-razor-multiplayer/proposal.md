## Why

The project requirements mandate an ASP.NET Core web application using Razor Pages with true multiplayer support (unlimited parallel games, players in different browsers/tabs), game animations, and cross-app continuity (games started in console can continue in web and vice versa). The previous MVC-based web app was reverted because it used Controllers+Views instead of Razor Pages and lacked real multiplayer — each session was isolated with no shared game state between players.

## What Changes

- Build a new ASP.NET Core Razor Pages web application (`src/WebApp`) reusing Domain, Application, and Infrastructure class libraries
- Implement a lobby system where players create and join games via unique game IDs, with game state persisted in the database (not session-only)
- Support all game modes: human vs human (two browsers), human vs AI, AI vs AI
- Add polling-based board refresh so player 2 sees player 1's moves without manual reload
- Add CSS animations for piece drops and game-over effects
- Wire both console and web apps to use the same EF persistence backend by default, enabling cross-app game continuity
- Implement full game CRUD in the web UI: create, save, load, continue, delete games
- Implement configuration CRUD in the web UI: create, edit, delete game configs
- Seed default configurations on first launch (same as console app)

## Capabilities

### New Capabilities
- `web-app-bootstrap`: Razor Pages application setup, DI composition root, session/middleware config, default config seeding
- `web-game-lobby`: Game creation, joining, listing active games, game lifecycle (start/join/resume/delete)
- `web-multiplayer`: Shared game state between players, turn enforcement, polling for opponent moves, player identification
- `web-board-rendering`: Board display as Razor partial, piece-drop CSS animations, column interaction, game-over display
- `web-config-management`: Configuration CRUD pages (list, create, edit, delete) using Razor Pages
- `web-game-persistence`: Save/load/continue/delete games from the web UI, leveraging existing IGameRepository
- `cross-app-continuity`: Both apps default to EF persistence with the same database, enabling games to move between console and web

### Modified Capabilities
- `app-bootstrap`: Console app switches from JSON to EF persistence by default, matching web app for cross-app continuity

## Impact

- `src/WebApp/` — entirely new Razor Pages project (was deleted in revert)
- `src/ConsoleApp/Program.cs` — switch default persistence from JSON to EF
- `ConnectX.slnx` — re-add WebApp project reference
- `tests/` — new integration tests for web app pages
- Dependencies: Microsoft.AspNetCore.App framework, existing EF/Infrastructure packages
