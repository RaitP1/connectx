## ADDED Requirements

### Requirement: Domain has no project references
An architecture test SHALL verify that the Domain assembly does not reference any other ConnectX project assemblies.

#### Scenario: Domain references no ConnectX projects
- **WHEN** the architecture test for Domain isolation runs
- **THEN** the Domain assembly's referenced assemblies contain none of: Application, Infrastructure, ConsoleApp, WebApp

### Requirement: Application references only Domain
An architecture test SHALL verify that the Application assembly references only the Domain assembly among ConnectX projects.

#### Scenario: Application depends only on Domain
- **WHEN** the architecture test for Application dependencies runs
- **THEN** the Application assembly's referenced ConnectX assemblies contain only Domain

### Requirement: Infrastructure does not reference presentation layers
An architecture test SHALL verify that the Infrastructure assembly does not reference ConsoleApp or WebApp.

#### Scenario: Infrastructure does not reference ConsoleApp
- **WHEN** the architecture test for Infrastructure isolation runs
- **THEN** the Infrastructure assembly's referenced assemblies do not contain ConsoleApp

#### Scenario: Infrastructure does not reference WebApp
- **WHEN** the architecture test for Infrastructure isolation runs
- **THEN** the Infrastructure assembly's referenced assemblies do not contain WebApp

### Requirement: All architecture tests pass on empty skeleton
All three architecture tests SHALL pass when run on the empty scaffolded solution.

#### Scenario: Architecture tests green
- **WHEN** `dotnet test` is run on the solution
- **THEN** all architecture tests pass with zero failures
