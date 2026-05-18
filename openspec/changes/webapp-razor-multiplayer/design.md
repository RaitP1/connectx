## Context

The Connect4 project has a working console app with Domain (GameBrain, GameConfig), Application (AI, DTOs, mappers, repository interfaces), and Infrastructure (JSON + EF persistence) layers. A previous MVC web app was reverted because it used Controllers+Views instead of the required Razor Pages and lacked true multiplayer — sessions were isolated per browser with no shared game state.

The project requirements specify: ASP.NET Core Razor Pages, unlimited parallel games, multiplayer across browsers/tabs, game animations, and cross-app continuity between console and web.

## Goals / Non-Goals

**Goals:**
- Razor Pages web app reusing existing class libraries (Domain, Application, Infrastructure)
- True multiplayer: two humans play the same game from different browsers, seeing each other's moves
- Game lobby: create, join, list, resume, delete games
- Piece-drop CSS animations and game-over effects
- Full config CRUD in the web UI
- Cross-app continuity: console and web share the same EF database

**Non-Goals:**
- Real-time WebSocket/SignalR push (polling is sufficient for this project scope)
- User accounts or authentication (players identified by session-assigned player slot)
- Mobile-specific responsive design
- Chat or social features between players

## Decisions

### 1. Razor Pages over MVC
**Choice**: ASP.NET Core Razor Pages with `@page` directive and PageModel code-behind.
**Rationale**: Project requirement explicitly states "using razor pages." Each page is self-contained with its own model, reducing indirection compared to MVC controllers.

### 2. Database-backed game state for multiplayer
**Choice**: All active games stored in the EF database via `IGameRepository`. Session stores only the player's game ID and player slot, not the full game state.
**Rationale**: For two players to share a game, the state must be in a shared store. The existing `IGameRepository` and `GameStateDto` already support this. Session holds a lightweight reference (game ID + player number).

### 3. Polling for opponent moves
**Choice**: JavaScript polling endpoint (`GET /Game/Poll?gameId=X`) that returns JSON with current game state. Client polls every 2 seconds when waiting for opponent's turn.
**Rationale**: Simple to implement, no additional dependencies. Adequate latency for a turn-based board game. Avoids SignalR complexity.

### 4. Player identification via game slots
**Choice**: When a player creates a game, they're assigned slot 0. When another player joins, they get slot 1. The game ID + player slot is stored in the browser session. A player can only make moves when it's their slot's turn.
**Rationale**: No auth system required. Session-based slot assignment is sufficient for the game scope. Multiple tabs can participate in different games simultaneously.

### 5. Shared EF persistence for cross-app continuity
**Choice**: Both console and web apps default to `AddEfPersistence()` with the same SQLite database path. Console app switches from JSON to EF as default.
**Rationale**: Cross-app continuity requires a shared persistence backend. EF with SQLite is already implemented and tested. Games saved in console appear in web's load list and vice versa.

### 6. CSS-only animations
**Choice**: CSS `@keyframes` for piece-drop animation (piece falls from top of column to landing row) and game-over highlight. No JavaScript animation libraries.
**Rationale**: Lightweight, no additional dependencies. CSS animations are smooth and hardware-accelerated. Sufficient for the visual feedback needed.

### 7. AI runs server-side synchronously
**Choice**: When a human makes a move against AI, the server runs the AI move immediately before returning the response. For AI-vs-AI, a dedicated page auto-advances with timed redirects.
**Rationale**: Reuses existing `MinimaxAI` without modification. Server-side execution is simpler than client-side polling for AI moves. AI response time is fast enough for synchronous handling.

## Risks / Trade-offs

- **Polling latency**: 2-second poll interval means up to 2s delay seeing opponent's move. Acceptable for turn-based gameplay. → Mitigation: Can reduce interval if needed; no architectural change required.
- **SQLite concurrent access**: Both console and web writing to the same SQLite file could cause locking under heavy load. → Mitigation: SQLite WAL mode handles concurrent reads well; write contention is low for turn-based games.
- **Session loss**: If a player's session expires mid-game, they lose their player slot assignment. → Mitigation: 30-minute session timeout; players can rejoin by loading the game from the lobby.
- **No game expiry**: Abandoned games persist in the database indefinitely. → Mitigation: Manual delete available from lobby; automatic cleanup is a non-goal for this scope.
