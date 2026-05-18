## ADDED Requirements

### Requirement: JSON persistence DI registration
The system SHALL provide an `AddJsonPersistence()` extension method on `IServiceCollection` in the Infrastructure layer that registers all JSON repository implementations.

#### Scenario: Extension method registers config repository
- **WHEN** `AddJsonPersistence()` is called
- **THEN** `JsonConfigRepository` SHALL be registered as the implementation of `IConfigRepository`

#### Scenario: Extension method registers game repository
- **WHEN** `AddJsonPersistence()` is called
- **THEN** `JsonGameRepository` SHALL be registered as the implementation of `IGameRepository`

#### Scenario: Singleton lifetime
- **WHEN** `AddJsonPersistence()` registers the repositories
- **THEN** both repositories SHALL be registered with singleton lifetime

### Requirement: Single-line persistence swap
The composition root SHALL be able to switch persistence backends by replacing one DI registration call.

#### Scenario: JSON persistence wiring
- **WHEN** the composition root calls `services.AddJsonPersistence()`
- **THEN** all repository resolutions SHALL use JSON file implementations
