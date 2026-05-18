## ADDED Requirements

### Requirement: Shared game state via database
All game state SHALL be stored in the database via `IGameRepository`, not in session. The session SHALL only store the current game ID and the player's slot number (0 or 1).

#### Scenario: Game state persisted after each move
- **WHEN** a player makes a move
- **THEN** the updated game state SHALL be saved to the database immediately

#### Scenario: Both players see the same board
- **WHEN** player 0 makes a move and player 1 refreshes
- **THEN** player 1 SHALL see the board with player 0's move reflected

### Requirement: Turn enforcement
The system SHALL only accept moves from the player whose turn it is, identified by matching the session's player slot to the game's current player.

#### Scenario: Correct player makes a move
- **WHEN** it is player 0's turn and the request comes from the session assigned to player 0
- **THEN** the move SHALL be accepted and processed

#### Scenario: Wrong player attempts a move
- **WHEN** it is player 0's turn and the request comes from the session assigned to player 1
- **THEN** the move SHALL be rejected

### Requirement: Polling for opponent moves
The system SHALL provide a JSON endpoint that returns the current game state. The client SHALL poll this endpoint every 2 seconds when it is not the current player's turn.

#### Scenario: Poll returns updated state
- **WHEN** the client polls and the opponent has made a move since last poll
- **THEN** the response SHALL include the updated board state and current player

#### Scenario: Poll returns no change
- **WHEN** the client polls and no move has been made
- **THEN** the response SHALL indicate no change (same current player)

#### Scenario: Poll detects game over
- **WHEN** the client polls and the game has ended (win or draw)
- **THEN** the response SHALL indicate the game is over with the result

### Requirement: Player slot assignment
When creating a game, the creator SHALL be assigned slot 0. When joining, the joiner SHALL be assigned slot 1. The slot SHALL be stored in the session alongside the game ID.

#### Scenario: Creator gets slot 0
- **WHEN** a player creates a new game
- **THEN** the session SHALL store the game ID and player slot 0

#### Scenario: Joiner gets slot 1
- **WHEN** a player joins an existing game
- **THEN** the session SHALL store the game ID and player slot 1

### Requirement: AI move execution
When it is an AI player's turn, the server SHALL execute the AI move immediately after the human's move (for H2AI) or run all moves to completion (for AI2AI).

#### Scenario: AI responds after human move
- **WHEN** a human player makes a move and the next player is AI
- **THEN** the server SHALL compute and apply the AI move before responding

#### Scenario: AI vs AI game runs to completion
- **WHEN** a game is created with both players as AI
- **THEN** the server SHALL run the game to completion and display the final board with result
