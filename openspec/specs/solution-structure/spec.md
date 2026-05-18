## ADDED Requirements

### Requirement: Solution contains all required projects
The solution file (`ConnectX.sln`) SHALL contain exactly five projects: Domain, Application, Infrastructure, ConsoleApp, and ConnectX.Tests.

#### Scenario: All projects present in solution
- **WHEN** `dotnet sln list` is run
- **THEN** the output lists Domain, Application, Infrastructure, ConsoleApp, and ConnectX.Tests

### Requirement: Projects use correct SDK types
Domain, Application, and Infrastructure SHALL be class libraries. ConsoleApp SHALL be a console application. ConnectX.Tests SHALL be an xUnit test project.

#### Scenario: Class library projects have no entry point
- **WHEN** Domain, Application, or Infrastructure is built
- **THEN** each produces a `.dll` with `OutputType` of `Library`

#### Scenario: ConsoleApp is executable
- **WHEN** ConsoleApp is built
- **THEN** it produces an executable with `OutputType` of `Exe`

### Requirement: Project references enforce the dependency rule
Project references SHALL follow the Clean Architecture dependency rule: Application references Domain; Infrastructure references Application; ConsoleApp references Application and Infrastructure; ConnectX.Tests references Domain, Application, and Infrastructure.

#### Scenario: Application references only Domain
- **WHEN** Application's project file is inspected
- **THEN** it contains exactly one `ProjectReference` pointing to Domain

#### Scenario: Infrastructure references only Application
- **WHEN** Infrastructure's project file is inspected
- **THEN** it contains exactly one `ProjectReference` pointing to Application

#### Scenario: ConsoleApp references Application and Infrastructure
- **WHEN** ConsoleApp's project file is inspected
- **THEN** it contains `ProjectReference` entries for Application and Infrastructure

#### Scenario: Test project references Domain, Application, and Infrastructure
- **WHEN** ConnectX.Tests' project file is inspected
- **THEN** it contains `ProjectReference` entries for Domain, Application, and Infrastructure

### Requirement: Solution builds successfully
The entire solution SHALL build without errors on the empty skeleton.

#### Scenario: Clean build succeeds
- **WHEN** `dotnet build` is run on the solution
- **THEN** the build succeeds with exit code 0 and zero errors

### Requirement: NuGet packages are installed in correct projects
Infrastructure SHALL reference `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design`. ConnectX.Tests SHALL reference `xunit` and `Microsoft.NET.Test.Sdk`.

#### Scenario: Infrastructure has EF Core packages
- **WHEN** Infrastructure's project file is inspected
- **THEN** it contains `PackageReference` entries for `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design`

#### Scenario: Test project has xUnit packages
- **WHEN** ConnectX.Tests' project file is inspected
- **THEN** it contains `PackageReference` entries for `xunit` and `Microsoft.NET.Test.Sdk`

### Requirement: .gitignore excludes .NET build artifacts
A `.gitignore` file SHALL exist at the repository root and SHALL exclude standard .NET build artifacts (bin/, obj/, *.user, etc.).

#### Scenario: .gitignore present
- **WHEN** the repository root is listed
- **THEN** a `.gitignore` file exists

#### Scenario: Build output excluded
- **WHEN** the solution is built and `git status` is checked
- **THEN** no `bin/` or `obj/` directories appear as untracked files
