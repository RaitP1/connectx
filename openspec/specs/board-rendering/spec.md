## ADDED Requirements

### Requirement: ASCII board display
The system SHALL render the game board as an ASCII grid showing column numbers along the top, cell borders, and player symbols in occupied cells.

#### Scenario: Empty board renders with borders and column numbers
- **WHEN** a new game starts with a 7x6 board
- **THEN** the display shows column numbers 1–7 across the top and an empty grid with cell borders

#### Scenario: Occupied cells show player symbols
- **WHEN** player 1 (symbol "X") has placed a piece in column 4
- **THEN** the board renders "X" in the bottom row of column 4

### Requirement: Arrow-key column selection
During a human player's turn, the system SHALL display a column selector above the board that the player moves with Left/Right arrow keys and confirms with Enter.

#### Scenario: Move selector right
- **WHEN** the player presses the Right arrow key
- **THEN** the selector moves to the next column (wrapping to column 1 if at the last column)

#### Scenario: Move selector left
- **WHEN** the player presses the Left arrow key
- **THEN** the selector moves to the previous column (wrapping to the last column if at column 1)

#### Scenario: Confirm column selection
- **WHEN** the player presses Enter on a column that has available space
- **THEN** the move is submitted to the game engine

#### Scenario: Full column is skipped or rejected
- **WHEN** the player presses Enter on a column that is full
- **THEN** the move is not submitted and the selector remains active

### Requirement: Save hotkey during game
The system SHALL allow the player to press S during their turn to save the current game state.

#### Scenario: Press S to save
- **WHEN** the player presses S during column selection
- **THEN** the system prompts for a save name and persists the game state via `IGameRepository`

### Requirement: Quit hotkey during game
The system SHALL allow the player to press Q during their turn to quit the current game and return to the menu.

#### Scenario: Press Q to quit
- **WHEN** the player presses Q during column selection
- **THEN** the current game ends and the user returns to the main menu

### Requirement: Game-over display
When the game ends, the system SHALL display the final board state and the outcome (winner name or draw).

#### Scenario: Winner announced
- **WHEN** a player achieves the win condition
- **THEN** the system displays the final board and a message identifying the winner by name

#### Scenario: Draw announced
- **WHEN** the board is full with no winner
- **THEN** the system displays the final board and a draw message

### Requirement: Current turn indicator
The system SHALL display whose turn it is, including the player's name and symbol.

#### Scenario: Show current player info
- **WHEN** it is Player 1's turn (name "Alice", symbol "X")
- **THEN** the display shows "Alice's turn (X)"
