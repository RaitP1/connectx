## MODIFIED Requirements

### Requirement: All architecture tests pass on empty skeleton
All architecture tests SHALL pass when run on the solution, including validation of the WebApp project's dependency compliance.

#### Scenario: Architecture tests green
- **WHEN** `dotnet test` is run on the solution
- **THEN** all architecture tests pass with zero failures, including tests that verify WebApp does not violate the dependency rule

#### Scenario: WebApp references only allowed projects
- **WHEN** the architecture test for WebApp dependencies runs
- **THEN** the WebApp assembly's referenced ConnectX assemblies contain only Domain, Application, and Infrastructure
