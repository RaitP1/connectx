## ADDED Requirements

### Requirement: Create new game
A player SHALL be able to create a new game by selecting a configuration. The system SHALL create a `GameStateDto`, persist it via `IGameRepository`, assign the creator as player 0, and store the game ID and player slot in the session.

#### Scenario: Create game from config
- **WHEN** a player selects a configuration and submits the new game form
- **THEN** a new game SHALL be created in the database, the player SHALL be assigned slot 0, and the player SHALL be redirected to the game play page

#### Scenario: Create game with AI opponent
- **WHEN** a player creates a game where player 2 is AI
- **THEN** the game SHALL be created and the player SHALL be the only human participant (no join needed)

### Requirement: Join existing game
A player SHALL be able to join a game that is waiting for a second player. The system SHALL assign the joiner as player 1 and store the game ID and player slot in their session.

#### Scenario: Join a waiting game
- **WHEN** a player clicks join on a game that has only one human player
- **THEN** the player SHALL be assigned slot 1 and redirected to the game play page

#### Scenario: Join a full game
- **WHEN** a player attempts to join a game that already has two human players
- **THEN** the join SHALL be rejected and the player SHALL see an error message

### Requirement: List active games
The system SHALL display a list of all saved games, showing game name, configuration, status (waiting for player, in progress, completed), and available actions (join, resume, delete).

#### Scenario: Games listed with status
- **WHEN** a player visits the load/lobby page
- **THEN** all saved games SHALL be listed with their name, config details, and current status

#### Scenario: Empty game list
- **WHEN** no saved games exist
- **THEN** the page SHALL display a message indicating no games are available

### Requirement: Delete game
A player SHALL be able to delete a saved game from the lobby.

#### Scenario: Delete a game
- **WHEN** a player clicks delete on a saved game and confirms
- **THEN** the game SHALL be removed from the database and no longer appear in the list
