## 1. Enums and Value Types

- [x] 1.1 Create `EBoardTopology` enum (Rectangle, Cylinder)
- [x] 1.2 Create `EAIDifficulty` enum (Easy, Medium, Hard)
- [x] 1.3 Create `PlayerType` value type (IsAI, Difficulty)

## 2. Game Configuration

- [x] 2.1 Create `GameConfig` with all properties (rows, columns, winCondition, player names/symbols, topology, player types)
- [x] 2.2 Implement `IsValid()` validation (dimensions > 0, winCondition fits board, no duplicate symbols, non-empty names)
- [x] 2.3 Write unit tests for GameConfig validation (valid config, invalid dimensions, bad win condition, duplicate symbols, empty names)

## 3. Game Engine — Board and Moves

- [x] 3.1 Create `GameBrain` with constructor from GameConfig (2D nullable int array, empty board, player 0 starts)
- [x] 3.2 Implement `GetCell(row, col)` with cylinder wrap support
- [x] 3.3 Implement `WrapColumn(col)` — modulo for cylinder, identity for rectangle
- [x] 3.4 Implement `MakeMove(column)` — gravity drop to lowest available row, reject full columns, switch player
- [x] 3.5 Implement `IsColumnAvailable(column)` and `GetAvailableColumns()`
- [x] 3.6 Write unit tests for gravity drop, turn switching, full column rejection, column availability

## 4. Game Engine — Win and Draw Detection

- [x] 4.1 Implement `CheckWin(row, col)` — four-direction line scan (horizontal, vertical, diagonal-down, diagonal-up)
- [x] 4.2 Integrate win/draw checks into MakeMove return flow
- [x] 4.3 Write unit tests for horizontal, vertical, and both diagonal wins on rectangle board
- [x] 4.4 Write unit tests for cylinder wrap-around wins (horizontal wrap, diagonal wrap)
- [x] 4.5 Write unit tests for draw detection (full board no winner, win on last move)

## 5. Game Engine — Clone and Undo

- [x] 5.1 Implement move history stack and `UndoMove()`
- [x] 5.2 Implement `Clone()` — deep copy of board, config, current player, and move history
- [x] 5.3 Write unit tests for undo (restores cell, restores player, no-op on empty history)
- [x] 5.4 Write unit tests for clone independence and fidelity

## 6. AI Opponent (Application/AI)

- [x] 6.1 Create `IAIPlayer` interface with `GetMove(GameBrain, int player)` method in `Application.AI`
- [x] 6.2 Implement `MinimaxAI` with alpha-beta pruning and difficulty-based depth (3/5/7) in `Application.AI`
- [x] 6.3 Implement `BoardEvaluator.EvaluatePosition(brain, player)` — score based on open lines, max for win, min for loss — in `Application.AI`
- [x] 6.4 Write unit tests: AI returns valid column, AI blocks opponent win, AI takes winning move
- [x] 6.5 Write unit tests: single available column, AI does not mutate original game state
- [x] 6.6 Write unit tests for position evaluation (winning, losing, neutral positions)
