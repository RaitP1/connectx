## ADDED Requirements

### Requirement: JSON config repository implements IConfigRepository
The system SHALL provide `JsonConfigRepository` in `Infrastructure.Persistence.Json` that implements `IConfigRepository` using JSON file storage.

#### Scenario: Repository is registered as IConfigRepository
- **WHEN** `AddJsonPersistence()` is called on the service collection
- **THEN** `JsonConfigRepository` SHALL be resolvable as `IConfigRepository`

### Requirement: Save config as JSON file
The system SHALL serialize a `GameConfig` to a JSON file named `{sanitized-name}.json` in the config storage directory.

#### Scenario: Save new config
- **WHEN** `Save(config)` is called with a config that has no existing file
- **THEN** a new JSON file SHALL be created with the serialized config

#### Scenario: Save overwrites existing config
- **WHEN** `Save(config)` is called with a config whose name matches an existing file
- **THEN** the existing file SHALL be overwritten with the new config

#### Scenario: Serialization format
- **WHEN** a config is saved
- **THEN** the JSON SHALL be indented for human readability and enum values SHALL be serialized as strings

### Requirement: Load config from JSON file
The system SHALL deserialize a `GameConfig` from the named JSON file in the config storage directory.

#### Scenario: Load existing config
- **WHEN** `Load(name)` is called with a name matching an existing file
- **THEN** the deserialized `GameConfig` SHALL be returned

#### Scenario: Load non-existent config
- **WHEN** `Load(name)` is called with a name that has no matching file
- **THEN** `null` SHALL be returned

### Requirement: List all saved configs
The system SHALL return all saved configs by reading all `.json` files in the config storage directory.

#### Scenario: List with saved configs
- **WHEN** `List()` is called and config files exist
- **THEN** all configs SHALL be deserialized and returned as `IReadOnlyList<GameConfig>`

#### Scenario: List with empty directory
- **WHEN** `List()` is called and no config files exist
- **THEN** an empty list SHALL be returned

### Requirement: Delete saved config
The system SHALL delete the JSON file for the named config.

#### Scenario: Delete existing config
- **WHEN** `Delete(name)` is called with a name matching an existing file
- **THEN** the file SHALL be removed from the config directory

#### Scenario: Delete non-existent config
- **WHEN** `Delete(name)` is called with a name that has no matching file
- **THEN** the operation SHALL complete without error

### Requirement: Config round-trip fidelity
A config saved and then loaded SHALL be identical to the original.

#### Scenario: Round-trip preserves all fields
- **WHEN** a `GameConfig` is saved and then loaded by name
- **THEN** all properties (Rows, Columns, WinCondition, player names, symbols, Topology, PlayerTypes) SHALL match the original

#### Scenario: Round-trip preserves enum values
- **WHEN** a config with `Cylinder` topology and `Hard` AI difficulty is saved and loaded
- **THEN** the enum values SHALL deserialize correctly
