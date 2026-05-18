## Context

Phases 0–5 established a Connect-X game with Domain (game engine, AI), Application (interfaces, DTOs, mappers), Infrastructure (JSON + EF persistence), and a ConsoleApp presentation layer. The architecture is Clean Architecture with strict dependency rules enforced by tests. A `WebApp` project was anticipated from Phase 0 but deferred.

The web layer must reuse all existing layers without modification. The ConsoleApp's `GameController` contains orchestration logic tightly coupled to console I/O — that logic will be re-expressed as MVC controller actions, not extracted or shared.

## Goals / Non-Goals

**Goals:**

- Deliver a playable Connect-X game in the browser via ASP.NET MVC + Razor
- Reuse Domain, Application, and Infrastructure layers unchanged
- Support all game modes: Human vs Human, Human vs AI, AI vs AI
- Support save/load game state and configuration management (CRUD)
- Use EF Core (SQLite) as the default persistence backend
- Maintain Clean Architecture dependency rules

**Non-Goals:**

- Real-time multiplayer or WebSocket-based gameplay — each browser session is single-player or local two-player
- User authentication or authorization — the app is single-user local
- JavaScript SPA framework — Razor views handle all rendering, minimal JS only for column click handling
- API endpoints for external consumers — controllers serve HTML views
- Mobile-responsive design — functional desktop layout is sufficient
- Extracting shared code from ConsoleApp (e.g., DefaultConfigSeeder) — duplication is acceptable for two small presentation projects

## Decisions

### 1. Session-based game state via TempData/Session

Game state (the `GameBrain` instance) must survive across HTTP requests during a game. Options considered:

- **Database-only**: Save after every move, reload on every request. Simple but high I/O for in-progress games.
- **Session storage**: Serialize `GameBrain` to session. Fast, per-tab isolation possible.
- **Hidden form fields**: Encode board state in the form. Fragile, large payloads.

**Decision**: Store the active game's `GameStateDto` in session (serialized as JSON). The DTO is already designed for serialization. On each move, deserialize → reconstruct `GameBrain` → make move → serialize back. Named saves still go through `IGameRepository` for persistence.

### 2. Razor views with minimal JavaScript

Options considered:

- **Full Razor (zero JS)**: Every move is a form POST with full page reload.
- **Razor + minimal JS**: Board clicks handled by JS that submits to an action, partial view updates the board area.
- **SPA with API**: React/Vue frontend with JSON API. Over-engineered for this scope.

**Decision**: Razor views with minimal JavaScript. Column clicks trigger form submissions. The board partial re-renders on each move. AI moves execute server-side and return the updated board. This keeps the stack pure C# while feeling responsive.

### 3. Controller structure — two controllers

- **GameController**: New game (config selection), make move, AI turn, save, load, game-over display.
- **ConfigController**: List, create, edit, delete configurations.

This mirrors the ConsoleApp's separation between game flow and configuration management.

### 4. Board rendering — HTML table with CSS

The board is rendered as an HTML `<table>` with colored circles (CSS `border-radius: 50%`). Each column header cell is a clickable link/form button. No canvas or SVG — keeps it simple and accessible.

### 5. EF persistence as default

ConsoleApp defaults to JSON persistence. WebApp defaults to EF/SQLite because:

- Web apps benefit from concurrent-safe database access
- SQLite file is self-contained, no server setup needed
- EF repositories are already scoped-lifetime friendly for DI in ASP.NET

### 6. DefaultConfigSeeder duplication

The `DefaultConfigSeeder` class lives in `ConsoleApp` namespace. Rather than creating a shared project for one static method, the WebApp will include its own seeder with the same preset definitions. Two small classes is better than a premature shared library.

## Risks / Trade-offs

- **[Risk] Session size with large boards** → GameStateDto serialization is compact (jagged array of nullable ints). Even a 20x20 board serializes to ~2KB. Acceptable for in-memory session.
- **[Risk] AI computation blocks the request thread** → MinimaxAI on Hard difficulty (depth 7) can take noticeable time on large boards. For this phase, this is acceptable — the request simply takes longer. Async offloading is a future optimization.
- **[Risk] No CSRF protection by default** → ASP.NET MVC includes `ValidateAntiForgeryToken` support. All POST actions will include anti-forgery tokens.
- **[Trade-off] No real-time updates for AI vs AI** → In AI vs AI mode, the server will run all moves and return the final board. A future enhancement could stream moves via SignalR.
- **[Trade-off] Session state lost on app restart** → In-progress games stored only in session will be lost if the server restarts. Saved games (via repository) survive. This is acceptable for a local development app.
