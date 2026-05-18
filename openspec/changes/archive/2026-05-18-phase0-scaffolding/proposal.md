## Why

We need the foundational solution structure for a Connect-X game built with Clean Architecture. Without the skeleton in place — projects, references, shared build settings, and architecture tests — subsequent phases have no structure to build on and no guardrails to prevent dependency violations. This must come first because every later phase (Domain, Application, Infrastructure, ConsoleApp, WebApp) depends on the project layout and dependency rule enforcement established here.

## What Changes

- Create a new .NET solution (`ConnectX.sln`) with five projects: Domain, Application, Infrastructure, ConsoleApp, and ConnectX.Tests
- Establish project references that enforce the Clean Architecture dependency rule (dependencies always point inward)
- Add `Directory.Build.props` for shared build settings: net9.0, nullable enable, warnings as errors, implicit usings
- Add architecture tests that verify the dependency rule at test time (Domain has no project refs, Application refs only Domain, Infrastructure does not ref ConsoleApp)
- Add required NuGet packages (EF Core for Infrastructure, xUnit for Tests)
- Add a `.gitignore` for .NET projects

## Capabilities

### New Capabilities
- `solution-structure`: Solution layout, project files, and inter-project references that enforce the dependency rule
- `build-configuration`: Shared build properties via Directory.Build.props (target framework, nullable, warnings-as-errors)
- `architecture-enforcement`: xUnit tests that verify assembly references conform to Clean Architecture layer rules

### Modified Capabilities

## Impact

- Creates the entire `src/` and `tests/` directory structure from scratch
- Introduces NuGet dependencies: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`, `xunit`, `Microsoft.NET.Test.Sdk`
- All subsequent phases build on this skeleton — incorrect references here would propagate architectural violations
