## Context

The Domain project is empty (only a marker class). Phase 0 established the solution skeleton with architecture tests enforcing the dependency rule. The Domain layer must remain pure — zero references to other ConnectX projects or external frameworks.

The roadmap requires: game configuration, a gravity-based game engine with rectangle and cylinder topologies, win/draw detection, and an AI opponent with minimax alpha-beta pruning.

## Goals / Non-Goals

**Goals:**
- Self-contained game engine playable entirely through unit tests
- Support both rectangle and cylinder board topologies
- Configurable board dimensions and win condition
- AI opponent at three difficulty levels via minimax with alpha-beta pruning
- Clone and undo operations to support AI lookahead and future save/load

**Non-Goals:**
- Persistence (Phase 3)
- UI rendering (Phase 4/6)
- DTOs, interfaces, or mapping (Phase 2 — Application layer)
- Network multiplayer logic (Phase 6)
- EF Core entities or database schema

## Decisions

**Board representation: 2D nullable int array (`int?[rows, cols]`)**
- Null = empty, 0 = player 1, 1 = player 2
- Simple, cache-friendly, easy to clone via `Array.Copy`
- Alternative: jagged array — rejected because 2D is cleaner for row/col access and the Application layer handles DTO conversion to jagged arrays later

**Cylinder wrap via modulo arithmetic in a single `WrapColumn` method**
- All board access and win-check column indices pass through `WrapColumn`
- Rectangle topology: `WrapColumn` is identity (clamps or rejects out-of-bounds)
- Cylinder topology: `WrapColumn` applies `((col % cols) + cols) % cols`
- Alternative: separate board classes per topology — rejected; the difference is one line of modulo math, not worth a class hierarchy

**Win detection: four-direction line scan from last-placed piece**
- Directions: horizontal, vertical, diagonal-down, diagonal-up
- Count consecutive same-player pieces in both directions along each axis
- Early-exit on first win found
- Alternative: full-board scan — rejected; O(board) per move vs O(winCondition) per move

**AI: minimax with alpha-beta pruning, depth controlled by difficulty enum**
- Easy = depth 3, Medium = depth 5, Hard = depth 7
- Position evaluation heuristic scores open lines of consecutive pieces
- Interface `IAIPlayer` with method `GetMove(GameBrain, int player)` allows swapping AI strategies later
- Alternative: Monte Carlo Tree Search — rejected; minimax is simpler, well-understood, and sufficient for Connect-X board sizes

**Undo via move history stack**
- Each `MakeMove` pushes `(row, col)` onto a stack
- `UndoMove` pops and clears the cell, switches player back
- Keeps undo O(1) without needing to snapshot the full board
- AI uses clone + undo for its search tree

## Risks / Trade-offs

**[Risk] Hard AI (depth 7) may be slow on large boards** → Acceptable for Phase 1; can add iterative deepening or time limits in a future phase if needed.

**[Risk] Position evaluation heuristic quality** → Simple heuristic (count open lines) is good enough for casual play. Can be refined later without changing the interface.

**[Trade-off] 2D array requires clone via nested copy** → O(rows × cols) per clone, but boards are small (typically <100 cells). Simplicity wins over optimization here.
