## ADDED Requirements

### Requirement: Storage directory resolution
The system SHALL provide a `FilesystemHelper` in `Infrastructure.Persistence.Json` that resolves storage base paths using the user's home directory.

#### Scenario: Config directory path
- **WHEN** the config storage path is requested
- **THEN** it SHALL resolve to `{UserProfile}/ConnectX/config/`

#### Scenario: Savegames directory path
- **WHEN** the savegames storage path is requested
- **THEN** it SHALL resolve to `{UserProfile}/ConnectX/savegames/`

### Requirement: Directory auto-creation
The system SHALL create storage directories if they do not exist when a path is resolved.

#### Scenario: First access creates directory
- **WHEN** a storage path is resolved and the directory does not exist
- **THEN** the system SHALL create the directory (including parent directories)

#### Scenario: Existing directory is not recreated
- **WHEN** a storage path is resolved and the directory already exists
- **THEN** the system SHALL return the path without error

### Requirement: Filename sanitization
The system SHALL sanitize all filenames before any filesystem operation to prevent path traversal and injection.

#### Scenario: Valid characters preserved
- **WHEN** a filename contains only alphanumeric characters, hyphens, and underscores
- **THEN** the sanitized filename SHALL be identical to the input

#### Scenario: Special characters stripped
- **WHEN** a filename contains characters outside `[a-zA-Z0-9_-]`
- **THEN** those characters SHALL be removed from the filename

#### Scenario: Path traversal sequences removed
- **WHEN** a filename contains `..`, `/`, or `\`
- **THEN** those sequences SHALL be removed before any filesystem operation

#### Scenario: Empty filename after sanitization
- **WHEN** a filename is empty or becomes empty after sanitization
- **THEN** the system SHALL throw an `ArgumentException`

### Requirement: Path confinement validation
The system SHALL validate that the fully resolved file path stays within the allowed storage directory.

#### Scenario: Path within allowed directory
- **WHEN** a resolved path is within the expected storage directory
- **THEN** the operation SHALL proceed normally

#### Scenario: Path escapes allowed directory
- **WHEN** a resolved path would escape the allowed storage directory
- **THEN** the system SHALL throw an `InvalidOperationException`
