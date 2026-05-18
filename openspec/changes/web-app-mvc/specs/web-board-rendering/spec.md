## ADDED Requirements

### Requirement: Visual board display
The board Razor view SHALL render the game board as an HTML table with colored circles representing pieces. Empty cells SHALL be displayed as white/empty circles. Player 1 pieces SHALL be one color and Player 2 pieces a different color.

#### Scenario: Empty board rendered
- **WHEN** a new game starts with a 6x7 board
- **THEN** the view renders a table with 6 rows and 7 columns, all cells showing empty circles

#### Scenario: Board with pieces
- **WHEN** the board has Player 1 in (5,3) and Player 2 in (4,3)
- **THEN** the view renders Player 1's cell with a red circle and Player 2's cell with a yellow circle, with all other cells empty

### Requirement: Column selection via click
Each column SHALL have a clickable header or top area. Clicking a column SHALL submit the move to the GameController.

#### Scenario: Click on available column
- **WHEN** the user clicks on column 3
- **THEN** a POST request is submitted to the GameController's move action with column 3

#### Scenario: Visual feedback for available columns
- **WHEN** the board is displayed during a human player's turn
- **THEN** columns with available space have a clickable hover effect indicating they are selectable

### Requirement: Current turn indicator
The board view SHALL display whose turn it is, showing the player's name and symbol.

#### Scenario: Player 1's turn
- **WHEN** the board is displayed and it is Player 1's turn
- **THEN** the view shows "Player 1 (X)'s turn" or equivalent indicator

#### Scenario: Player 2's turn
- **WHEN** the board is displayed and it is Player 2's turn
- **THEN** the view shows "Player 2 (O)'s turn" or equivalent indicator

### Requirement: Game-over display
When the game is over, the board view SHALL display the final board state with a result message and disable column selection.

#### Scenario: Winner display
- **WHEN** Player 1 wins the game
- **THEN** the view shows "Player 1 (X) wins!" and column headers are no longer clickable

#### Scenario: Draw display
- **WHEN** the game ends in a draw
- **THEN** the view shows "It's a draw!" and column headers are no longer clickable

### Requirement: Game action buttons
The board view SHALL include buttons for saving the current game and quitting to the main menu.

#### Scenario: Save button present during active game
- **WHEN** the game is in progress
- **THEN** the view includes a "Save Game" button that links to the save action

#### Scenario: Quit button present
- **WHEN** the game view is displayed
- **THEN** the view includes a "Quit" button that returns to the home page

### Requirement: Board respects configuration dimensions
The board view SHALL dynamically render rows and columns based on the current `GameConfig` dimensions, not a fixed size.

#### Scenario: Small board (4x5)
- **WHEN** a game is started with a 4-row, 5-column configuration
- **THEN** the board renders with 4 rows and 5 columns

#### Scenario: Large board (9x9)
- **WHEN** a game is started with a 9-row, 9-column configuration
- **THEN** the board renders with 9 rows and 9 columns
