## ADDED Requirements

### Requirement: Start a new game from a configuration
The GameController SHALL present a list of saved configurations and start a new game when the user selects one. A fresh `GameBrain` SHALL be created from the selected `GameConfig`, serialized to session, and the board view displayed.

#### Scenario: User selects a configuration and starts a game
- **WHEN** the user visits the new-game page and selects the "Classical" configuration
- **THEN** a new `GameBrain` is created with that config, the game state is stored in session, and the user is redirected to the play view showing an empty board

#### Scenario: No configurations exist
- **WHEN** the user visits the new-game page and no configurations are saved
- **THEN** the page displays a message indicating no configurations are available and links to the configuration management page

### Requirement: Human player makes a move by selecting a column
The GameController SHALL accept a column number via POST, deserialize the game state from session, execute the move on `GameBrain`, and return the updated board view.

#### Scenario: Valid move in an available column
- **WHEN** a human player submits column 3 and column 3 has available space
- **THEN** the piece is placed, `CurrentPlayer` advances, the updated state is saved to session, and the board view re-renders with the new piece

#### Scenario: Move in a full column
- **WHEN** a human player submits a column that is full
- **THEN** the move is rejected, an error message is displayed, and the board state remains unchanged

#### Scenario: Move after game is over
- **WHEN** a player submits a move but the game is already over
- **THEN** the move is rejected and the user is redirected to the game-over view

### Requirement: AI move executes automatically after human move
When the current player after a human move is an AI player, the GameController SHALL automatically execute the AI move using `MinimaxAI` with the configured difficulty before returning the response.

#### Scenario: Human moves, then AI responds
- **WHEN** a human player makes a valid move and the next player is AI
- **THEN** the AI move is computed and applied, session is updated with both moves, and the board view shows both the human and AI pieces

#### Scenario: AI move results in game over
- **WHEN** the AI makes a move that completes a winning line
- **THEN** the game-over state is detected, session is updated, and the user sees the game-over view with the AI as winner

### Requirement: AI vs AI game runs to completion
When both players are AI, the GameController SHALL provide an action that runs the entire game to completion and displays the final result.

#### Scenario: AI vs AI game completes
- **WHEN** the user starts a game where both players are AI
- **THEN** all moves are computed server-side, the final board state is stored in session, and the game-over view is displayed with the result

### Requirement: Game-over detection and display
The GameController SHALL check `IsGameOver` after each move. When the game ends, the controller SHALL redirect to a game-over view showing the winner or draw result.

#### Scenario: Player wins
- **WHEN** a move completes a winning line
- **THEN** the game-over view displays the winning player's name and symbol

#### Scenario: Draw
- **WHEN** the board is full with no winner
- **THEN** the game-over view displays a draw message

### Requirement: Save current game
The GameController SHALL allow saving the current in-progress game to the repository with a user-provided name.

#### Scenario: Save game with a name
- **WHEN** the user enters "My Game" as the save name and submits
- **THEN** the current game state is mapped to `GameStateDto` via `GameStateMapper` and persisted via `IGameRepository`, and the user sees a confirmation message

#### Scenario: Save overwrites existing name
- **WHEN** the user saves with a name that already exists in the repository
- **THEN** the existing save is overwritten with the current game state

### Requirement: Load a saved game
The GameController SHALL list saved games and allow the user to resume one. Loading restores the `GameBrain` from the repository and stores it in session.

#### Scenario: Load and resume a saved game
- **WHEN** the user selects "My Game" from the saved games list
- **THEN** the `GameStateDto` is loaded from `IGameRepository`, converted to `GameBrain` via `GameStateMapper`, stored in session, and the board view displays the restored state

#### Scenario: No saved games exist
- **WHEN** the user visits the load-game page and no saves exist
- **THEN** the page displays a message indicating no saved games are available

### Requirement: Delete a saved game
The GameController SHALL allow deleting a saved game by name.

#### Scenario: Delete an existing save
- **WHEN** the user deletes "My Game"
- **THEN** the save is removed from `IGameRepository` and the saved games list refreshes without it

### Requirement: Anti-forgery protection on all POST actions
All POST actions on the GameController SHALL validate anti-forgery tokens to prevent CSRF attacks.

#### Scenario: POST without anti-forgery token
- **WHEN** a POST request is made to a game action without a valid anti-forgery token
- **THEN** the request is rejected with a 400 status
