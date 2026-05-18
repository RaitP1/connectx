## 1. Project Scaffolding and Bootstrap

- [x] 1.1 Create `src/WebApp/` ASP.NET MVC project with web SDK, add project references to Domain, Application, and Infrastructure
- [x] 1.2 Add WebApp project to `ConnectX.slnx` solution file
- [x] 1.3 Configure `Program.cs` with MVC, Razor views, session middleware, EF persistence via `AddEfPersistence`, and default routing
- [x] 1.4 Add `DefaultConfigSeeder` that seeds four presets on first launch when repository is empty
- [x] 1.5 Create shared Razor layout (`_Layout.cshtml`) with navigation bar (Home, New Game, Configurations) and a `HomeController` with index view

## 2. Configuration Management (ConfigController)

- [x] 2.1 Create `ConfigController` with `Index` action listing all configurations via `IConfigRepository`
- [x] 2.2 Add `Create` GET/POST actions with a form for all 11 GameConfig fields (name, rows, columns, win condition, player names/symbols/types, topology), with validation via `GameConfig.IsValid()`
- [x] 2.3 Add `Edit` GET/POST actions that load an existing config into the form and save changes
- [x] 2.4 Add `Delete` POST action with confirmation, anti-forgery token validation on all POST actions
- [x] 2.5 Create Razor views for config list, create form, and edit form with dropdowns for topology and player types

## 3. Game Controller and Session State

- [x] 3.1 Create `GameController` with `New` action that lists available configurations for game start
- [x] 3.2 Add `Start` POST action that creates a `GameBrain` from selected config, serializes `GameStateDto` to session, and redirects to the play view
- [x] 3.3 Add `Play` GET action that deserializes game state from session and renders the board view
- [x] 3.4 Add `Move` POST action that deserializes game state, executes the human move, runs AI follow-up if next player is AI, updates session, and redirects to play view
- [x] 3.5 Add AI vs AI handling that runs all moves to completion and redirects to game-over view
- [x] 3.6 Add game-over detection after each move with redirect to `GameOver` view showing winner or draw

## 4. Save/Load Game State

- [x] 4.1 Add `Save` GET/POST actions — form for save name, persist via `IGameRepository` using `GameStateMapper.ToDto`
- [x] 4.2 Add `Load` GET action listing saved games from `IGameRepository`
- [x] 4.3 Add `Resume` POST action that loads a `GameStateDto`, converts to `GameBrain` via `GameStateMapper.ToDomain`, stores in session, and redirects to play view
- [x] 4.4 Add `DeleteSave` POST action to remove a saved game, with anti-forgery token validation on all POST actions

## 5. Board Rendering (Razor Views)

- [x] 5.1 Create board partial view rendering an HTML table with colored circles (red/yellow), dynamic rows/columns from GameConfig
- [x] 5.2 Add clickable column headers that POST to the Move action with the column number
- [x] 5.3 Add turn indicator showing current player name and symbol
- [x] 5.4 Add game-over view with final board, result message, and disabled column selection
- [x] 5.5 Add CSS styling for board grid, piece colors, hover effects on available columns, and layout

## 6. Architecture and Integration Tests

- [x] 6.1 Update architecture enforcement tests to verify WebApp assembly references only Domain, Application, and Infrastructure
- [x] 6.2 Write integration tests using `WebApplicationFactory` for GameController actions (start game, make move, save, load)
- [x] 6.3 Write integration tests for ConfigController actions (list, create, edit, delete)
- [x] 6.4 Verify all existing tests still pass with the new WebApp project in the solution
