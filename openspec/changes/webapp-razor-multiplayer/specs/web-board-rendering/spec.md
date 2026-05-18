## ADDED Requirements

### Requirement: Board display as HTML table
The game board SHALL be rendered as an HTML table with cells colored by player ownership (empty, player 0, player 1). Column headers SHALL contain clickable drop buttons when it is the current player's turn.

#### Scenario: Empty board rendered
- **WHEN** a new game is started with 6 rows and 7 columns
- **THEN** the board SHALL render as a 6x7 table with all cells in the empty state

#### Scenario: Board with pieces
- **WHEN** several moves have been made
- **THEN** each cell SHALL display the correct player color (red for player 0, yellow for player 1, neutral for empty)

#### Scenario: Column buttons disabled when not player's turn
- **WHEN** it is not the viewing player's turn
- **THEN** the column drop buttons SHALL be hidden or disabled

#### Scenario: Full column button disabled
- **WHEN** a column is completely filled
- **THEN** the drop button for that column SHALL be disabled

### Requirement: Piece drop animation
When a piece is placed, the piece SHALL animate from the top of the column to its landing row using a CSS animation.

#### Scenario: Piece drops with animation
- **WHEN** a move is made and the board re-renders
- **THEN** the newly placed piece SHALL have a CSS drop animation from the top of the board to its final row position

#### Scenario: Existing pieces do not re-animate
- **WHEN** the board re-renders after a new move
- **THEN** only the most recently placed piece SHALL animate; all other pieces SHALL be static

### Requirement: Turn indicator
The page SHALL display whose turn it is, with the player name and their piece color.

#### Scenario: Player 0's turn
- **WHEN** it is player 0's turn
- **THEN** the turn indicator SHALL show player 0's name and a red color indicator

#### Scenario: Player 1's turn
- **WHEN** it is player 1's turn
- **THEN** the turn indicator SHALL show player 1's name and a yellow color indicator

### Requirement: Game over display
When the game ends, the page SHALL display the result (winner name or draw) and offer actions to start a new game or return to the lobby.

#### Scenario: Win displayed
- **WHEN** a player wins
- **THEN** the page SHALL show the winner's name and a congratulatory message

#### Scenario: Draw displayed
- **WHEN** the game ends in a draw
- **THEN** the page SHALL show a draw message

#### Scenario: Game over actions
- **WHEN** the game is over
- **THEN** the page SHALL offer links to start a new game and return to the lobby

### Requirement: Save game from play page
During an active game, the player SHALL be able to save the game with a name for later resumption.

#### Scenario: Save game
- **WHEN** a player enters a save name and submits during an active game
- **THEN** the game SHALL be persisted with the given name and a confirmation message SHALL appear
