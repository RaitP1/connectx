## 1. Project Setup

- [x] 1.1 Create WebApp.csproj as Razor Pages project with references to Application and Infrastructure
- [x] 1.2 Add WebApp to ConnectX.slnx solution file
- [x] 1.3 Create Program.cs with Razor Pages services, session config, EF persistence, DB init, and config seeding
- [x] 1.4 Create shared layout (_Layout.cshtml) with navigation (Home, New Game, Load Game, Configs)
- [x] 1.5 Create _ViewImports.cshtml and _ViewStart.cshtml
- [x] 1.6 Add wwwroot with site.css (board styles, animations) and site.js (polling logic)
- [x] 1.7 Create Home/Index page (landing page with links to game and config sections)

## 2. Configuration Management Pages

- [x] 2.1 Create Configs/Index page listing all configurations with properties
- [x] 2.2 Create Configs/Create page with form for new configuration (name, rows, columns, win condition, topology, player types)
- [x] 2.3 Create Configs/Edit page for modifying existing configurations
- [x] 2.4 Add delete handler on Configs/Index page
- [x] 2.5 Add form validation (board size 3-20, win condition 3-10, win condition vs board dimensions)

## 3. Game Lobby and Persistence Pages

- [x] 3.1 Create Games/New page listing configs to start a new game
- [x] 3.2 Implement game creation handler: create GameBrain, save to DB via IGameRepository, assign player slot 0 in session
- [x] 3.3 Create Games/List page showing all saved games with status, join/resume/delete actions
- [x] 3.4 Implement join handler: assign player slot 1 in session, redirect to play
- [x] 3.5 Implement resume handler: load game from DB, assign player slot, redirect to play
- [x] 3.6 Implement delete handler: remove game from DB

## 4. Game Play and Multiplayer

- [x] 4.1 Create Games/Play page with board rendering, turn indicator, save form
- [x] 4.2 Create _Board partial rendering the board as HTML table with column drop buttons
- [x] 4.3 Implement move handler: validate turn ownership, apply move via GameBrain, run AI follow-up if needed, save to DB
- [x] 4.4 Implement JSON poll endpoint (GET handler returning board state, current player, game over status)
- [x] 4.5 Add client-side JavaScript polling (2s interval when not player's turn, auto-refresh board on opponent move)
- [x] 4.6 Create Games/GameOver page (or section) showing result and post-game actions
- [x] 4.7 Implement AI-vs-AI: run game to completion on creation, display final board

## 5. Board Animations

- [x] 5.1 Add CSS @keyframes drop animation for newly placed pieces
- [x] 5.2 Track last-placed piece position to apply animation class only to the new piece
- [x] 5.3 Add game-over visual effect (highlight winning text/result area)

## 6. Cross-App Continuity

- [x] 6.1 Switch ConsoleApp Program.cs default from AddJsonPersistence() to AddEfPersistence() with shared DB path
- [x] 6.2 Add EnsureCreated() call in ConsoleApp startup for EF mode
- [x] 6.3 Verify a game saved in console appears in web game list and vice versa

## 7. Integration Tests

- [x] 7.1 Create ConnectXWebFactory for WebApplicationFactory-based testing
- [x] 7.2 Add tests for config pages (list, create, edit, delete)
- [x] 7.3 Add tests for game lifecycle (create, play moves, save, load, delete)
- [x] 7.4 Add tests for multiplayer flow (create game, join game, turn enforcement, poll endpoint)
- [x] 7.5 Add tests for AI game modes (H2AI move triggers AI response, AI2AI runs to completion)
