## ADDED Requirements

### Requirement: Shared build properties via Directory.Build.props
A `Directory.Build.props` file SHALL exist at the repository root and SHALL apply to all projects in the solution.

#### Scenario: Directory.Build.props exists at root
- **WHEN** the repository root is listed
- **THEN** a `Directory.Build.props` file is present

### Requirement: Target framework is net9.0
All projects SHALL target `net9.0` via the shared `Directory.Build.props`.

#### Scenario: Framework set in shared props
- **WHEN** `Directory.Build.props` is inspected
- **THEN** it contains `<TargetFramework>net9.0</TargetFramework>`

#### Scenario: Individual projects do not override framework
- **WHEN** any project's `.csproj` file is inspected
- **THEN** it does not contain a `TargetFramework` element (inherited from shared props)

### Requirement: Nullable reference types enabled
All projects SHALL have nullable reference types enabled via `<Nullable>enable</Nullable>` in `Directory.Build.props`.

#### Scenario: Nullable enabled in shared props
- **WHEN** `Directory.Build.props` is inspected
- **THEN** it contains `<Nullable>enable</Nullable>`

### Requirement: Warnings treated as errors
All projects SHALL treat warnings as errors via `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `Directory.Build.props`.

#### Scenario: TreatWarningsAsErrors set in shared props
- **WHEN** `Directory.Build.props` is inspected
- **THEN** it contains `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`

### Requirement: Implicit usings enabled
All projects SHALL have implicit usings enabled via `<ImplicitUsings>enable</ImplicitUsings>` in `Directory.Build.props`.

#### Scenario: ImplicitUsings enabled in shared props
- **WHEN** `Directory.Build.props` is inspected
- **THEN** it contains `<ImplicitUsings>enable</ImplicitUsings>`
