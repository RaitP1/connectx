## ADDED Requirements

### Requirement: Game construction from config
The system SHALL construct a game engine (`GameBrain`) from a `GameConfig`, initializing an empty board and setting player 1 as the current player.

#### Scenario: New game from config
- **WHEN** a GameBrain is created with a valid GameConfig (6 rows, 7 cols)
- **THEN** all cells SHALL be empty, current player SHALL be 0 (player 1), and the game SHALL not be over

### Requirement: Gravity-based piece dropping
The system SHALL drop pieces to the lowest available row in the specified column when `MakeMove(column)` is called.

#### Scenario: Drop into empty column
- **WHEN** MakeMove is called on an empty column
- **THEN** the piece SHALL land in the bottom row of that column

#### Scenario: Drop into partially filled column
- **WHEN** MakeMove is called on a column with 3 pieces already placed
- **THEN** the piece SHALL land in row index (rows - 4) — directly above existing pieces

#### Scenario: Drop into full column
- **WHEN** MakeMove is called on a column that is completely filled
- **THEN** the move SHALL be rejected (return false or equivalent)

### Requirement: Player turn switching
The system SHALL alternate the current player after each successful move.

#### Scenario: Turn alternation
- **WHEN** player 0 makes a valid move
- **THEN** the current player SHALL switch to player 1

#### Scenario: Failed move does not switch turn
- **WHEN** a move is rejected (full column)
- **THEN** the current player SHALL remain unchanged

### Requirement: Board cell access
The system SHALL provide read access to any cell on the board via `GetCell(row, col)`.

#### Scenario: Read empty cell on rectangle board
- **WHEN** GetCell is called on an empty cell within bounds on a rectangle board
- **THEN** it SHALL return null

#### Scenario: Read occupied cell
- **WHEN** GetCell is called on a cell occupied by player 0
- **THEN** it SHALL return 0

#### Scenario: Cylinder wrap-around cell access
- **WHEN** GetCell is called with a column index beyond the board width on a cylinder board
- **THEN** the column SHALL wrap around using modulo arithmetic

### Requirement: Column availability
The system SHALL report whether a specific column can accept a piece via `IsColumnAvailable(column)` and list all available columns via `GetAvailableColumns()`.

#### Scenario: Available column
- **WHEN** IsColumnAvailable is called on a column with at least one empty row
- **THEN** it SHALL return true

#### Scenario: Full column
- **WHEN** IsColumnAvailable is called on a completely filled column
- **THEN** it SHALL return false

#### Scenario: Get all available columns
- **WHEN** GetAvailableColumns is called on a board where columns 0, 2, and 5 have space
- **THEN** it SHALL return a collection containing exactly 0, 2, and 5

### Requirement: Horizontal win detection
The system SHALL detect a win when a player has the required number of consecutive pieces in a horizontal line.

#### Scenario: Horizontal win on rectangle board
- **WHEN** player 0 places 4 consecutive pieces horizontally on a rectangle board with winCondition=4
- **THEN** the game SHALL detect a win for player 0

#### Scenario: Horizontal win wrapping on cylinder board
- **WHEN** player 0 has pieces at columns [5, 6, 0, 1] on a 7-column cylinder board with winCondition=4
- **THEN** the game SHALL detect a win for player 0

### Requirement: Vertical win detection
The system SHALL detect a win when a player has the required number of consecutive pieces in a vertical line.

#### Scenario: Vertical win
- **WHEN** player 1 places 4 pieces stacked vertically in the same column with winCondition=4
- **THEN** the game SHALL detect a win for player 1

### Requirement: Diagonal win detection
The system SHALL detect a win when a player has the required number of consecutive pieces in a diagonal line (both directions).

#### Scenario: Diagonal down-right win
- **WHEN** player 0 has 4 pieces along a top-left to bottom-right diagonal with winCondition=4
- **THEN** the game SHALL detect a win for player 0

#### Scenario: Diagonal up-right win
- **WHEN** player 0 has 4 pieces along a bottom-left to top-right diagonal with winCondition=4
- **THEN** the game SHALL detect a win for player 0

#### Scenario: Diagonal win with cylinder wrap
- **WHEN** a diagonal line wraps horizontally on a cylinder board and reaches winCondition length
- **THEN** the game SHALL detect a win

### Requirement: Draw detection
The system SHALL detect a draw when the board is completely full and no player has won.

#### Scenario: Full board with no winner
- **WHEN** all cells are filled and no win condition is met
- **THEN** the game SHALL report a draw

#### Scenario: Win on last move
- **WHEN** the last empty cell is filled and it completes a win
- **THEN** the game SHALL report a win, not a draw

### Requirement: Column wrapping
The system SHALL provide a `WrapColumn(col)` method that applies modulo arithmetic for cylinder topology and identity for rectangle topology.

#### Scenario: Wrap on cylinder board
- **WHEN** WrapColumn is called with column=9 on a 7-column cylinder board
- **THEN** it SHALL return 2

#### Scenario: Negative wrap on cylinder board
- **WHEN** WrapColumn is called with column=-1 on a 7-column cylinder board
- **THEN** it SHALL return 6

#### Scenario: No wrap on rectangle board
- **WHEN** WrapColumn is called with column=3 on a rectangle board
- **THEN** it SHALL return 3

### Requirement: Game cloning
The system SHALL support deep cloning via `Clone()` that produces an independent copy of the entire game state.

#### Scenario: Clone independence
- **WHEN** a game is cloned and a move is made on the clone
- **THEN** the original game state SHALL remain unchanged

#### Scenario: Clone fidelity
- **WHEN** a game is cloned
- **THEN** the clone SHALL have identical board state, current player, and configuration

### Requirement: Move undo
The system SHALL support undoing the last move via `UndoMove()`, restoring the previous board state and current player.

#### Scenario: Undo restores cell
- **WHEN** UndoMove is called after a move at column 3
- **THEN** the cell where the piece was placed SHALL be empty again

#### Scenario: Undo restores current player
- **WHEN** UndoMove is called
- **THEN** the current player SHALL revert to the player who made the undone move

#### Scenario: Undo on empty history
- **WHEN** UndoMove is called with no moves made
- **THEN** it SHALL have no effect or return false

### Requirement: Position evaluation
The system SHALL provide an `EvaluatePosition(player)` method that returns a numeric score reflecting how favorable the board position is for the given player.

#### Scenario: Winning position scores highest
- **WHEN** EvaluatePosition is called for a player who has already won
- **THEN** the score SHALL be the maximum positive value

#### Scenario: Losing position scores lowest
- **WHEN** EvaluatePosition is called for a player whose opponent has won
- **THEN** the score SHALL be the maximum negative value

#### Scenario: Neutral position
- **WHEN** EvaluatePosition is called on an empty board
- **THEN** the score SHALL be approximately zero
