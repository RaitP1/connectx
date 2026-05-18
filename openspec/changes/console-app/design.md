## Context

Phases 0–3 delivered a complete game engine (Domain), AI opponent with minimax (Application), and JSON file persistence (Infrastructure). The ConsoleApp project exists with correct project references but contains only a placeholder `Program.cs`. All interfaces (`IConfigRepository`, `IGameRepository`, `IAIPlayer`) and mapping (`GameStateMapper`) are ready to consume.

The console app is the first presentation layer — it must compose all layers via DI and provide a full-featured terminal UI for playing Connect-X.

## Goals / Non-Goals

**Goals:**
- Deliver a playable console game supporting Human vs Human, Human vs AI, and AI vs AI modes
- Provide a reusable menu system with unlimited nesting depth, cursor-based navigation, and updateable labels
- Support mid-game save/load via JSON persistence
- Ship default configuration presets seeded on first launch
- Keep all game logic in Domain/Application — the console app is pure presentation and composition

**Non-Goals:**
- No color/ANSI escape code support (keep it portable across terminals)
- No WebApp or SignalR (Phase 6)
- No EF Core persistence (Phase 5)
- No new Domain or Application types — consume existing APIs only
- No configuration editing beyond the preset values (board width, height, win condition, topology, player types)

## Decisions

### 1. Menu system as standalone classes (`Menu` + `MenuItem`)

Menu and MenuItem are self-contained types with no dependency on game logic. The menu tree is built by composing MenuItem instances with `Func<string>` action callbacks, enabling the GameController to wire behavior without the menu knowing about game concepts.

**Alternatives considered:**
- Single procedural loop with switch statements — simpler initially but cannot support dynamic nesting, updateable labels, or reuse
- Third-party library (Spectre.Console) — adds a dependency and more capability than needed; the menu requirements are narrow

### 2. GameController as the orchestrator

A single `GameController` class coordinates menu construction, game loop execution, and persistence calls. It holds references to the repositories, AI factory, and menu system. This avoids scattering wiring logic across multiple classes.

**Alternatives considered:**
- Separate controller per concern (MenuController, GameLoopController, PersistenceController) — over-abstraction for a console app; increases indirection without proportional benefit

### 3. DI via `Microsoft.Extensions.DependencyInjection`

The composition root in `Program.cs` builds a `ServiceCollection`, calls `AddJsonPersistence()`, registers ConsoleApp services, and resolves `GameController` to run. This stays consistent with the Infrastructure layer's existing `ServiceCollectionExtensions`.

**Alternatives considered:**
- Manual construction (new everything) — works for a small app, but breaks the pattern established in Infrastructure and makes switching to EF Core persistence (Phase 5) require code changes in Program.cs wiring

### 4. Arrow-key input for both menu and board

Both the menu cursor and the column selector use `Console.ReadKey(true)` with arrow key handling. This provides a consistent UX. Number keys serve as shortcuts in menus for quick access.

### 5. Seed default configs via IConfigRepository on startup

If `IConfigRepository.List()` returns an empty list, Program.cs seeds four presets (Classical, Connect-3, Connect-5, Connect-4 Cylinder) using `IConfigRepository.Save()`. This avoids a separate seeding mechanism and uses the same persistence path as user-created configs.

## Risks / Trade-offs

- **Terminal compatibility**: Raw `Console.ReadKey` and cursor positioning may behave differently on non-standard terminals (e.g., some IDE integrated terminals). → Mitigation: Stick to basic console APIs (`SetCursorPosition`, `Clear`, `ReadKey`); avoid ANSI escape codes.
- **Blocking AI turns**: On Hard difficulty with large boards, minimax can take seconds. The console will appear frozen. → Mitigation: Acceptable for a console app; the AI difficulty depths (3/5/7) were tuned for reasonable response times.
- **No undo in game loop**: The roadmap doesn't specify undo during play (only AI uses `UndoMove` internally). → Mitigation: Explicitly out of scope; can be added later without architectural changes since `GameBrain.UndoMove()` already exists.
