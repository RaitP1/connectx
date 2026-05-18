## ADDED Requirements

### Requirement: List all configurations
The ConfigController SHALL display all saved configurations with their key properties (name, dimensions, win condition, topology, player types).

#### Scenario: Configurations exist
- **WHEN** the user visits the configurations page and configurations are saved
- **THEN** all configurations are listed showing name, rows x columns, win condition, topology, and player types

#### Scenario: No configurations exist
- **WHEN** the user visits the configurations page and the repository is empty
- **THEN** the page displays a message indicating no configurations exist and provides a link to create one

### Requirement: Create a new configuration
The ConfigController SHALL present a form for creating a new `GameConfig` with all fields: name, rows, columns, win condition, player 1 name/symbol/type, player 2 name/symbol/type, and topology. The form SHALL validate input using `GameConfig.IsValid()` before saving.

#### Scenario: Valid configuration created
- **WHEN** the user fills in all fields with valid values and submits
- **THEN** a new `GameConfig` is created, saved via `IConfigRepository`, and the user is redirected to the configurations list

#### Scenario: Invalid configuration rejected
- **WHEN** the user submits a configuration where win condition exceeds board dimensions
- **THEN** the form re-displays with a validation error message and the user's input preserved

#### Scenario: Duplicate name
- **WHEN** the user creates a configuration with a name that already exists
- **THEN** the existing configuration is overwritten (upsert behavior matching `IConfigRepository.Save`)

### Requirement: Edit an existing configuration
The ConfigController SHALL allow editing a saved configuration by loading it into the form, accepting changes, and saving the updated version.

#### Scenario: Load configuration for editing
- **WHEN** the user clicks edit on the "Classical" configuration
- **THEN** the edit form displays with all fields pre-populated from the existing configuration

#### Scenario: Save edited configuration
- **WHEN** the user modifies the rows from 6 to 8 and submits
- **THEN** the updated configuration is saved via `IConfigRepository` and the user is redirected to the configurations list

### Requirement: Delete a configuration
The ConfigController SHALL allow deleting a configuration by name with a confirmation step.

#### Scenario: Delete an existing configuration
- **WHEN** the user confirms deletion of "Connect-3"
- **THEN** the configuration is removed via `IConfigRepository.Delete` and the configurations list refreshes without it

#### Scenario: Delete a non-existent configuration
- **WHEN** a delete request is made for a name that does not exist
- **THEN** the operation completes without error (no-op per repository contract) and the user is redirected to the list

### Requirement: Form includes all GameConfig fields
The create and edit forms SHALL include inputs for: name (text), rows (number), columns (number), win condition (number), player 1 name (text), player 1 symbol (text), player 2 name (text), player 2 symbol (text), topology (dropdown: Rectangle/Cylinder), player 1 type (dropdown: Human/Easy AI/Medium AI/Hard AI), player 2 type (dropdown: Human/Easy AI/Medium AI/Hard AI).

#### Scenario: All fields rendered on create form
- **WHEN** the user visits the create configuration page
- **THEN** the form contains labeled inputs for all 11 GameConfig fields with sensible defaults

#### Scenario: Topology dropdown options
- **WHEN** the user opens the topology dropdown
- **THEN** the options are "Rectangle" and "Cylinder"

#### Scenario: Player type dropdown options
- **WHEN** the user opens a player type dropdown
- **THEN** the options are "Human", "Easy AI", "Medium AI", and "Hard AI"

### Requirement: Anti-forgery protection on all POST actions
All POST actions on the ConfigController SHALL validate anti-forgery tokens.

#### Scenario: POST without anti-forgery token
- **WHEN** a POST request is made to a config action without a valid anti-forgery token
- **THEN** the request is rejected with a 400 status
