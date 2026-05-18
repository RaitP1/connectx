## ADDED Requirements

### Requirement: Shared persistence backend
Both the console app and web app SHALL default to EF persistence with the same SQLite database file, enabling games saved in one app to be loaded in the other.

#### Scenario: Console game loaded in web
- **WHEN** a game is saved in the console app using EF persistence
- **THEN** the game SHALL appear in the web app's game list and be resumable

#### Scenario: Web game loaded in console
- **WHEN** a game is saved in the web app
- **THEN** the game SHALL appear in the console app's load game menu and be resumable

### Requirement: Compatible game state format
Both apps SHALL use the same `GameStateDto` and `GameStateMapper` for serialization, ensuring game state is fully interchangeable regardless of which app created it.

#### Scenario: State format compatibility
- **WHEN** a game is saved by either app
- **THEN** the persisted `GameStateDto` SHALL contain identical fields (Name, Config, Board, CurrentPlayer, SavedAt) regardless of the originating app

### Requirement: Console app default persistence switch
The console app SHALL default to `AddEfPersistence()` instead of `AddJsonPersistence()` to enable cross-app continuity out of the box.

#### Scenario: Console uses EF by default
- **WHEN** the console app starts with default configuration
- **THEN** it SHALL use EF persistence with the same database path as the web app
