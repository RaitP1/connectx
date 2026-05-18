## ADDED Requirements

### Requirement: Save game from web UI
A player SHALL be able to save the current game with a user-provided name. The system SHALL use `GameStateMapper.ToDto()` and `IGameRepository.Save()`.

#### Scenario: Save active game
- **WHEN** a player provides a save name and submits the save form
- **THEN** the game state SHALL be persisted with that name and the player SHALL see a success message

#### Scenario: Save name validation
- **WHEN** a player submits an empty or whitespace-only save name
- **THEN** the save SHALL be rejected with a validation error

### Requirement: Load and resume game from web UI
A player SHALL be able to load a previously saved game and continue playing. The system SHALL use `IGameRepository.Load()` and `GameStateMapper.ToDomain()` to restore the game.

#### Scenario: Resume saved game
- **WHEN** a player selects a saved game and clicks resume
- **THEN** the game SHALL be loaded from the database, the player SHALL be assigned their slot, and play SHALL continue from where it was saved

### Requirement: Delete saved game from web UI
A player SHALL be able to delete a saved game from the game list.

#### Scenario: Delete saved game
- **WHEN** a player clicks delete on a saved game
- **THEN** the game SHALL be removed from the database

### Requirement: List saved games
The system SHALL display all saved games with their name, configuration summary, current player, and save timestamp.

#### Scenario: Games listed with metadata
- **WHEN** a player visits the game list page
- **THEN** each game SHALL display its name, board dimensions, current turn, and when it was saved
