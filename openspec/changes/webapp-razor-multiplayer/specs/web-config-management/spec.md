## ADDED Requirements

### Requirement: List configurations
The system SHALL display all saved game configurations with their properties (name, board size, win condition, topology, player types).

#### Scenario: Configurations listed
- **WHEN** a player visits the configurations page
- **THEN** all configurations SHALL be displayed with their name, rows, columns, win condition, and topology

#### Scenario: No configurations exist
- **WHEN** no configurations are saved (edge case after all deleted)
- **THEN** the page SHALL display a message and a link to create a new configuration

### Requirement: Create configuration
The system SHALL allow creating a new configuration with name, rows (3-20), columns (3-20), win condition (3-10), board topology (rectangle/cylinder), and player types for both players.

#### Scenario: Create valid configuration
- **WHEN** a player submits a valid configuration form
- **THEN** the configuration SHALL be saved and the player SHALL be redirected to the configuration list

#### Scenario: Validation rejects invalid config
- **WHEN** a player submits a configuration where win condition exceeds both board dimensions
- **THEN** the form SHALL display a validation error and not save the configuration

#### Scenario: Duplicate name rejected
- **WHEN** a player submits a configuration with a name that already exists
- **THEN** the form SHALL display an error indicating the name is taken

### Requirement: Edit configuration
The system SHALL allow editing an existing configuration's properties.

#### Scenario: Edit configuration
- **WHEN** a player modifies a configuration's board size and submits
- **THEN** the updated configuration SHALL be saved and reflected in the list

#### Scenario: Edit preserves name
- **WHEN** a player edits a configuration
- **THEN** the configuration name SHALL remain the same (name is the identifier)

### Requirement: Delete configuration
The system SHALL allow deleting a saved configuration.

#### Scenario: Delete configuration
- **WHEN** a player clicks delete on a configuration
- **THEN** the configuration SHALL be removed and no longer appear in the list
