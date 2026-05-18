## Why

Phases 0–5 built the game engine, AI, persistence (JSON + EF), and a console UI — but there is no web-based way to play. An ASP.NET MVC + Razor web application gives Connect-X a graphical browser UI, making the game accessible without a terminal. This is the second presentation layer, proving the Clean Architecture abstraction works across UI technologies.

## What Changes

- Add a `WebApp` ASP.NET MVC project with Razor views for all game screens
- Build MVC controllers that reuse the existing Domain (GameBrain, GameConfig), Application (IAIPlayer, repositories, mappers), and Infrastructure (EF persistence) layers
- Implement HTML/CSS board rendering with click-to-drop column selection, replacing ASCII rendering
- Add session-based game state management so each browser tab has its own game
- Wire DI composition root in `Program.cs` with EF persistence and default config seeding
- Reuse the existing `DefaultConfigSeeder` for first-launch preset population
- Update architecture enforcement tests to include the WebApp assembly

## Capabilities

### New Capabilities

- `web-game-controller`: MVC controller handling game flow — new game, column moves, AI turns, save/load, game-over detection
- `web-config-controller`: MVC controller for browsing, creating, editing, and deleting game configurations
- `web-board-rendering`: Razor partial view and CSS for visual board display with click-to-drop interaction and game status
- `web-app-bootstrap`: WebApp Program.cs with EF persistence DI, session configuration, default config seeding, and routing

### Modified Capabilities

- `architecture-enforcement`: Architecture tests updated to verify WebApp does not violate the dependency rule (Infrastructure must not reference WebApp)

## Impact

- **New project**: `src/WebApp/` — ASP.NET MVC project referencing Application, Infrastructure, and Domain
- **Solution file**: `ConnectX.slnx` updated to include WebApp project
- **Architecture tests**: Updated to include WebApp assembly in dependency checks
- **NuGet packages**: `Microsoft.AspNetCore.App` framework reference (implicit in web SDK), `Microsoft.EntityFrameworkCore.Sqlite` (already in Infrastructure)
- **No changes** to Domain, Application, or Infrastructure layers — the web layer consumes them as-is
