## 1. Menu System

- [ ] 1.1 Create `MenuItem` class with label (static or dynamic via `Func<string>`), action callback (`Func<string>`), and optional child `Menu`
- [ ] 1.2 Create `Menu` class with item registration, cursor-based selection (Up/Down/Enter), number key shortcuts, wrapping navigation, and level-aware navigation items (Exit / Return to Previous / Return to Main based on depth)
- [ ] 1.3 Add hot key uniqueness validation — throw on duplicate shortcut keys within same menu level
- [ ] 1.4 Write unit tests for menu: cursor wrapping, number key dispatch, level-aware navigation items, duplicate key rejection, updateable labels

## 2. Board Rendering and Input

- [ ] 2.1 Create `GameUI` class with `DrawBoard(GameBrain)` — ASCII grid with column numbers, cell borders, and player symbols
- [ ] 2.2 Implement `GetPlayerMove(GameBrain)` — arrow-key column selector (Left/Right/Enter) with wrapping, S to save, Q to quit, skip full columns
- [ ] 2.3 Implement `ShowGameOver(GameBrain, GameConfig)` — final board display with winner name or draw message
- [ ] 2.4 Implement current turn indicator showing player name and symbol
- [ ] 2.5 Write unit tests for board rendering: correct grid dimensions, symbol placement, column selector logic

## 3. Game Controller

- [ ] 3.1 Create `GameController` with constructor accepting `IConfigRepository`, `IGameRepository`, and service provider
- [ ] 3.2 Implement `PlayGame(GameConfig)` — game loop handling Human vs Human, Human vs AI, and AI vs AI modes with AI move computation via `MinimaxAI`
- [ ] 3.3 Implement save/load/delete for game states: prompt for name, call `IGameRepository`, list available saves
- [ ] 3.4 Implement save/load/delete for configurations: prompt for name, call `IConfigRepository`, list available configs
- [ ] 3.5 Implement settings mutations: change board width, height, win condition, topology toggle, player type configuration (Human/AI with difficulty)
- [ ] 3.6 Implement `ViewCurrentSettings()` showing all active config values and `ShowHowToPlay()` with rules and controls
- [ ] 3.7 Build complete menu tree: Main → New Game / Load Game / Settings / How to Play / Exit, with Settings submenus for board config, player config, save/load config
- [ ] 3.8 Write unit tests for controller: game mode dispatch, settings mutation immutability, config seeding logic

## 4. App Bootstrap

- [ ] 4.1 Rewrite `Program.cs` as composition root: build `ServiceCollection`, call `AddJsonPersistence()`, register ConsoleApp services, resolve and run `GameController`
- [ ] 4.2 Implement default config seeding: seed Classical (7x6, 4, rectangle), Connect-3 (5x4, 3, rectangle), Connect-5 (9x7, 5, rectangle), Connect-4 Cylinder (7x6, 4, cylinder) when config list is empty
- [ ] 4.3 Verify end-to-end: application launches, main menu displays, can start and play a game, save/load works
