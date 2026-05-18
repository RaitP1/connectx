# Connect-X Clean Architecture Roadmap

## Context

Build a new Connect-X game using Clean Architecture (Domain, Application, Infrastructure, ConsoleApp, WebApp).

## Target Structure

```
ConnectX/
├── src/
│   ├── Domain/                          <- pure game logic, zero deps
│   ├── Application/                     <- interfaces, DTOs, mapping
│   ├── Infrastructure/                  <- JSON + EF persistence
│   ├── ConsoleApp/                      <- console UI + menu
│   └── WebApp/                          <- Razor Pages + SignalR
├── tests/
│   └── ConnectX.Tests/                  <- arch tests + unit tests
└── ConnectX.sln
```

## Dependency Rule

```
ConsoleApp / WebApp
       |
       v
  Infrastructure
       |
       v
  Application
       |
       v
    Domain
```

Every arrow points inward. Nothing points outward.

---

## Phase 0: Scaffolding

Create the solution structure, project files, and dependency references. No game code yet — just the skeleton that enforces the architecture.

- [x] Create solution: `dotnet new sln -n ConnectX`
- [x] Create projects:
  - `dotnet new classlib -n Domain -o src/Domain`
  - `dotnet new classlib -n Application -o src/Application`
  - `dotnet new classlib -n Infrastructure -o src/Infrastructure`
  - `dotnet new console -n ConsoleApp -o src/ConsoleApp`
  - `dotnet new xunit -n ConnectX.Tests -o tests/ConnectX.Tests`
- [x] Add all projects to solution
- [x] Set up project references (dependency rule):
  - Application -> Domain
  - Infrastructure -> Application
  - ConsoleApp -> Application + Infrastructure
  - ConnectX.Tests -> Domain + Application + Infrastructure
- [x] Add NuGet packages:
  - Infrastructure: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`
  - ConsoleApp: `Microsoft.EntityFrameworkCore.Sqlite` (for migrations CLI)
  - ConnectX.Tests: `xunit`, `Microsoft.NET.Test.Sdk`
- [x] Add `Directory.Build.props` for shared settings:
  - Target framework (net10.0)
  - `<Nullable>enable</Nullable>`
  - `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` (course requirement)
  - `<ImplicitUsings>enable</ImplicitUsings>`
- [x] Write `ArchitectureTests.cs` — enforce dependency rule from day one:
  - Domain references no ConnectX projects
  - Application references only Domain
  - Infrastructure does not reference ConsoleApp or WebApp
- [x] Add `.gitignore` (dotnet template: `dotnet new gitignore`)
- [x] Run tests — all 3 architecture tests pass on empty projects

**Deliverable:** Empty solution where `dotnet build` and `dotnet test` pass. Architecture tests green.

---

## Phase 1: Domain — Core Game Engine

Pure game rules. No persistence, no UI, no dependencies. Enums use `E` prefix, interfaces use `I` prefix.

- [x] `EBoardTopology.cs` — enum: Rectangle, Cylinder
- [x] `EAIDifficulty.cs` — enum: Easy, Medium, Hard
- [x] `PlayerType.cs` — value type: IsAI + Difficulty
- [x] `GameConfig.cs` — board dimensions, win condition, player names/symbols, topology, player types. `IsValid()` validation method
- [x] `GameBrain.cs` — the game engine:
  - Constructor from GameConfig
  - `MakeMove(column)` — gravity drop, win check, draw check, player switch
  - `GetCell(row, col)` — read board state (with cylinder wrap)
  - `CheckWin(row, col)` — four-direction line scan
  - `WrapColumn(col)` — modulo arithmetic for cylinder topology
  - `IsColumnAvailable(column)`, `GetAvailableColumns()`
  - `Clone()`, `UndoMove()` — AI support
- [x] Unit tests for Domain:
  - Gravity drop works
  - Horizontal/vertical/diagonal win detection
  - Cylinder wrap-around win detection
  - Draw detection (full board)
  - Config validation

**Deliverable:** Pure game logic with tests. You could play a game programmatically in a test.

---

## Phase 2: Application — AI, Interfaces, DTOs, Mapping

AI logic, repository contracts, and DTOs. Depends only on Domain.

- [x] `AI/IAIPlayer.cs` — interface: `GetMove(brain, player)`
- [x] `AI/MinimaxAI.cs` — minimax with alpha-beta pruning, depth by difficulty (3/5/7)
- [x] `AI/BoardEvaluator.cs` — static position scoring heuristic, reads board via GameBrain's public API
- [x] `Config/Interfaces/IConfigRepository.cs`:
  - `List()`, `Save(GameConfig)`, `Load(id)`, `Delete(id)`
- [x] `Game/Interfaces/IGameRepository.cs`:
  - `List()`, `Save(GameStateDto)`, `Load(id)`, `Delete(id)`
- [x] `Game/Dto/GameStateDto.cs`:
  - Name, Config (as GameConfig), Board (as `int?[][]` jagged array), CurrentPlayer, SavedAt
- [x] `Game/Mapping/GameStateMapper.cs`:
  - `ToDto(GameBrain, string name)` — snapshot brain into DTO
  - `ToDomain(GameStateDto)` — reconstruct GameBrain from DTO
- [x] `ServiceCollectionExtensions.cs` — registers Application-level services (mapper)
- [x] Unit tests:
  - [x] AI returns valid moves, blocks wins, takes winning moves
  - [x] Board evaluator scores positions correctly
  - [x] Mapper round-trip: brain -> DTO -> brain produces identical state
  - [x] Mapper preserves cylinder topology, player types, current player

**Deliverable:** AI opponent, clean contracts, and mapping. No persistence implementation yet.

---

## Phase 3: Infrastructure — JSON Persistence

First persistence backend. File-based, no database.

- [ ] `Persistence/Json/FilesystemHelper.cs` — resolve `~/ConnectX/config/` and `~/ConnectX/savegames/`
- [ ] `Persistence/Json/JsonConfigRepository.cs` — implements `IConfigRepository`:
  - Save as `{name}.json` with `System.Text.Json`
  - Filename sanitization + path traversal protection
- [ ] `Persistence/Json/JsonGameRepository.cs` — implements `IGameRepository`:
  - Save as `{name}_{timestamp}.json`
  - Same security: sanitize filenames, validate paths stay in allowed directory
- [ ] `ServiceCollectionExtensions.cs`:
  - `AddJsonPersistence()` — registers JSON repo implementations
- [ ] Integration tests:
  - Save/load/list/delete config round-trip
  - Save/load/list/delete game state round-trip
  - Filename sanitization works (special chars stripped)

**Deliverable:** Working JSON persistence. Can save and load games/configs to disk.

---

## Phase 4: ConsoleApp — Playable Game

Wire everything together into a playable console application.

- [ ] `UI/Menu.cs` + `UI/MenuItem.cs` — console menu system:
  - Unlimited depth of nested submenus with breadcrumb navigation
  - Level-aware navigation items:
    - Level 1: Exit only
    - Level 2: Exit + Return to previous
    - Level 3+: Exit + Return to previous + Return to main
  - Cursor-based selection (arrow keys + Enter)
  - Number key shortcuts as alternative input
  - **Updateable labels** — menu items can refresh their display text (e.g., "Board Width [7]" -> "Board Width [10]" after user changes it)
  - **Hot key uniqueness validation** — reject duplicate keys at registration time
  - User action callbacks via `Func<string>` delegates
- [ ] `UI/GameUI.cs` — board rendering and input:
  - `DrawBoard()` — ASCII grid with column numbers and player symbols
  - `GetPlayerMove()` — arrow key column selection, S to save, Q to quit
  - `AnimateDrop()` — optional drop animation
  - `ShowGameOver()` — winner/draw display
- [ ] `GameController.cs` — orchestrates menus, game loop, persistence:
  - `StartNewGame()`, `PlayGame()` — game loop with AI support
  - `SaveCurrentGame()`, `LoadAndContinueGame()`, `DeleteSavedGame()`
  - `SaveConfiguration()`, `LoadConfiguration()`, `DeleteConfiguration()`
  - `ChangeBoardWidth/Height/WinCondition/Topology()`
  - `ConfigurePlayer1Type/Player2Type()`, `SelectGameMode()`
  - `ViewCurrentSettings()`, `ShowHowToPlay()`
- [ ] Precreated configurations — ship default presets on first launch:
  - "Classical" — 7x6 board, connect 4, rectangle
  - "Connect-3" — 5x4 board, connect 3, rectangle
  - "Connect-5" — 9x7 board, connect 5, rectangle
  - "Connect-4 Cylinder" — 7x6 board, connect 4, cylinder
  - Seed via `IConfigRepository.Save()` if config list is empty on startup
- [ ] `Program.cs` — entry point + DI wiring:
  - `services.AddJsonPersistence()`
  - Seed default configs if none exist
  - Build menu tree, run menu loop
- [ ] Manual testing: play full games (HvH, HvAI, AIvAI), save/load mid-game, manage configs

**Deliverable:** Fully functional console game. Feature parity with original `hyper-connectx` ConsoleApp.

---

## Phase 5: Infrastructure — EF Core Persistence

Second persistence backend. SQLite via Entity Framework Core.

- [ ] `Persistence/EF/AppDbContext.cs`:
  - `DbSet<GameConfig>` — PlayerType as owned entities
  - `DbSet<GameStateDto>` — Board + Config as JSON columns (value converters)
  - Restrict cascade deletes
- [ ] `Persistence/EF/EfConfigRepository.cs` — implements `IConfigRepository`
- [ ] `Persistence/EF/EfGameRepository.cs` — implements `IGameRepository`
- [ ] Migrations: `dotnet ef migrations add InitialCreate`
- [ ] Update `ServiceCollectionExtensions.cs`:
  - `AddEfPersistence(connectionString)` — registers EF repo implementations
- [ ] Update ConsoleApp `Program.cs` — swap between JSON/EF with one line change:
  ```csharp
  services.AddJsonPersistence();
  // or
  services.AddEfPersistence("Data Source=~/app.db");
  ```
- [ ] Integration tests:
  - Same save/load/list/delete tests, running against SQLite in-memory
  - Verify JSON value converters serialize Board and Config correctly

**Deliverable:** Two interchangeable persistence backends behind the same interfaces.

---

## Phase 6: WebApp — Browser-Based Multiplayer

ASP.NET Core Razor Pages + SignalR for real-time play.

- [ ] Create project: `dotnet new webapp -n WebApp -o src/WebApp`
- [ ] Add to solution, set references: WebApp -> Application + Infrastructure
- [ ] Add NuGet: `Microsoft.AspNetCore.SignalR`
- [ ] `Services/IGameSessionService.cs` + `GameSessionService.cs`:
  - In-memory `ConcurrentDictionary<Guid, GameSession>` for live games
  - `CreateGame()`, `JoinGame()`, `MakeMove()`, `GetAvailableGames()`
  - Stale game cleanup (completed games older than 1 hour)
- [ ] `Models/GameSession.cs` — live game state (brain + connection IDs + player types)
- [ ] `Models/GameViewModel.cs` — serializable game state for the browser
- [ ] `Hubs/GameHub.cs` — SignalR hub:
  - `JoinGame(gameId)` — assign player number, add to group
  - `MakeMove(gameId, column)` — validate turn, broadcast move
  - `LeaveGame(gameId)` — cleanup
  - `ProcessAIMovesIfNeeded()` — server-side AI loop with delay
  - `OnDisconnectedAsync()` — cleanup stale connections
- [ ] Razor Pages:
  - `Game/Create` — configure board, select game mode, create session
  - `Game/Join` — list available games, join lobby
  - `Game/Play` — the game board (JS + SignalR client)
  - `Game/Load` — load saved games (uses IGameRepository)
- [ ] `wwwroot/js/game.js` — SignalR client:
  - Connect to hub, render board, handle moves, show game over
- [ ] `Program.cs` — DI wiring:
  - `services.AddApplicationServices()`
  - `services.AddJsonPersistence()` (or EF)
  - `services.AddSingleton<IGameSessionService, GameSessionService>()`
  - `app.MapHub<GameHub>("/gameHub")`
- [ ] Update architecture tests:
  - Infrastructure does not reference WebApp
  - Add WebApp assembly check
- [ ] **Cross-app game continuity** — a game saved in ConsoleApp can be loaded and continued in WebApp, and vice versa:
  - Both apps use the same `IGameRepository` / `IConfigRepository` interfaces
  - Both apps must point to the same storage location (same JSON directory or same SQLite DB)
  - WebApp `Game/Load` page reads from the shared repo, reconstructs `GameBrain`, creates a live `GameSession`
  - Test: save mid-game in console -> open web -> load -> continue playing (and reverse)

**Deliverable:** Browser-based Connect-X with real-time multiplayer, AI opponents, game saving, and cross-app game continuity.

---

## Phase Summary

| Phase | What                  | Key Output                       |
| ----- | --------------------- | -------------------------------- |
| 0     | Scaffolding           | Empty solution, arch tests green |
| 1     | Domain                | Game engine with unit tests      |
| 2     | Application           | AI, interfaces, DTOs, mapper     |
| 3     | Infrastructure (JSON) | File-based persistence           |
| 4     | ConsoleApp            | Playable console game            |
| 5     | Infrastructure (EF)   | SQLite persistence, swappable    |
| 6     | WebApp                | SignalR multiplayer in browser   |

Each phase produces something buildable and testable. No phase depends on a later phase.
