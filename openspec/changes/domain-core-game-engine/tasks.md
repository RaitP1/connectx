## 1. Enums and Value Types

- [ ] 1.1 Create `BoardTopology` enum (Rectangle, Cylinder)
- [ ] 1.2 Create `AIDifficulty` enum (Easy, Medium, Hard)
- [ ] 1.3 Create `PlayerType` value type (IsAI, Difficulty)

## 2. Game Configuration

- [ ] 2.1 Create `GameConfig` with all properties (rows, columns, winCondition, player names/symbols, topology, player types)
- [ ] 2.2 Implement `IsValid()` validation (dimensions > 0, winCondition fits board, no duplicate symbols, non-empty names)
- [ ] 2.3 Write unit tests for GameConfig validation (valid config, invalid dimensions, bad win condition, duplicate symbols, empty names)

## 3. Game Engine — Board and Moves

- [ ] 3.1 Create `GameBrain` with constructor from GameConfig (2D nullable int array, empty board, player 0 starts)
- [ ] 3.2 Implement `GetCell(row, col)` with cylinder wrap support
- [ ] 3.3 Implement `WrapColumn(col)` — modulo for cylinder, identity for rectangle
- [ ] 3.4 Implement `MakeMove(column)` — gravity drop to lowest available row, reject full columns, switch player
- [ ] 3.5 Implement `IsColumnAvailable(column)` and `GetAvailableColumns()`
- [ ] 3.6 Write unit tests for gravity drop, turn switching, full column rejection, column availability

## 4. Game Engine — Win and Draw Detection

- [ ] 4.1 Implement `CheckWin(row, col)` — four-direction line scan (horizontal, vertical, diagonal-down, diagonal-up)
- [ ] 4.2 Integrate win/draw checks into MakeMove return flow
- [ ] 4.3 Write unit tests for horizontal, vertical, and both diagonal wins on rectangle board
- [ ] 4.4 Write unit tests for cylinder wrap-around wins (horizontal wrap, diagonal wrap)
- [ ] 4.5 Write unit tests for draw detection (full board no winner, win on last move)

## 5. Game Engine — Clone, Undo, Evaluate

- [ ] 5.1 Implement move history stack and `UndoMove()`
- [ ] 5.2 Implement `Clone()` — deep copy of board, config, current player, and move history
- [ ] 5.3 Implement `EvaluatePosition(player)` — score based on open lines, max for win, min for loss
- [ ] 5.4 Implement `MakeMoveWithoutValidation(column)` for AI internal use
- [ ] 5.5 Write unit tests for undo (restores cell, restores player, no-op on empty history)
- [ ] 5.6 Write unit tests for clone independence and fidelity
- [ ] 5.7 Write unit tests for position evaluation (winning, losing, neutral positions)

## 6. AI Opponent

- [ ] 6.1 Create `IAIPlayer` interface with `GetMove(GameBrain, int player)` method
- [ ] 6.2 Implement `MinimaxAI` with alpha-beta pruning and difficulty-based depth (3/5/7)
- [ ] 6.3 Write unit tests: AI returns valid column, AI blocks opponent win, AI takes winning move
- [ ] 6.4 Write unit tests: single available column, AI does not mutate original game state
