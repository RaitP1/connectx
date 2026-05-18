## 1. Project Scaffolding and Bootstrap

- [ ] 1.1 Create `src/WebApp/` ASP.NET MVC project with web SDK, add project references to Domain, Application, and Infrastructure
- [ ] 1.2 Add WebApp project to `ConnectX.slnx` solution file
- [ ] 1.3 Configure `Program.cs` with MVC, Razor views, session middleware, EF persistence via `AddEfPersistence`, and default routing
- [ ] 1.4 Add `DefaultConfigSeeder` that seeds four presets on first launch when repository is empty
- [ ] 1.5 Create shared Razor layout (`_Layout.cshtml`) with navigation bar (Home, New Game, Configurations) and a `HomeController` with index view

## 2. Configuration Management (ConfigController)

- [ ] 2.1 Create `ConfigController` with `Index` action listing all configurations via `IConfigRepository`
- [ ] 2.2 Add `Create` GET/POST actions with a form for all 11 GameConfig fields (name, rows, columns, win condition, player names/symbols/types, topology), with validation via `GameConfig.IsValid()`
- [ ] 2.3 Add `Edit` GET/POST actions that load an existing config into the form and save changes
- [ ] 2.4 Add `Delete` POST action with confirmation, anti-forgery token validation on all POST actions
- [ ] 2.5 Create Razor views for config list, create form, and edit form with dropdowns for topology and player types

## 3. Game Controller and Session State

- [ ] 3.1 Create `GameController` with `New` action that lists available configurations for game start
- [ ] 3.2 Add `Start` POST action that creates a `GameBrain` from selected config, serializes `GameStateDto` to session, and redirects to the play view
- [ ] 3.3 Add `Play` GET action that deserializes game state from session and renders the board view
- [ ] 3.4 Add `Move` POST action that deserializes game state, executes the human move, runs AI follow-up if next player is AI, updates session, and redirects to play view
- [ ] 3.5 Add AI vs AI handling that runs all moves to completion and redirects to game-over view
- [ ] 3.6 Add game-over detection after each move with redirect to `GameOver` view showing winner or draw

## 4. Save/Load Game State

- [ ] 4.1 Add `Save` GET/POST actions — form for save name, persist via `IGameRepository` using `GameStateMapper.ToDto`
- [ ] 4.2 Add `Load` GET action listing saved games from `IGameRepository`
- [ ] 4.3 Add `Resume` POST action that loads a `GameStateDto`, converts to `GameBrain` via `GameStateMapper.ToDomain`, stores in session, and redirects to play view
- [ ] 4.4 Add `DeleteSave` POST action to remove a saved game, with anti-forgery token validation on all POST actions

## 5. Board Rendering (Razor Views)

- [ ] 5.1 Create board partial view rendering an HTML table with colored circles (red/yellow), dynamic rows/columns from GameConfig
- [ ] 5.2 Add clickable column headers that POST to the Move action with the column number
- [ ] 5.3 Add turn indicator showing current player name and symbol
- [ ] 5.4 Add game-over view with final board, result message, and disabled column selection
- [ ] 5.5 Add CSS styling for board grid, piece colors, hover effects on available columns, and layout

## 6. Architecture and Integration Tests

- [ ] 6.1 Update architecture enforcement tests to verify WebApp assembly references only Domain, Application, and Infrastructure
- [ ] 6.2 Write integration tests using `WebApplicationFactory` for GameController actions (start game, make move, save, load)
- [ ] 6.3 Write integration tests for ConfigController actions (list, create, edit, delete)
- [ ] 6.4 Verify all existing tests still pass with the new WebApp project in the solution
