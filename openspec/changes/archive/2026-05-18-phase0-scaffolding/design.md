## Context

This is a greenfield .NET 9 project. There is no existing solution structure. The roadmap defines a six-phase build of a Connect-X game using Clean Architecture with five projects (Domain, Application, Infrastructure, ConsoleApp, WebApp) and a test project. Phase 0 establishes the skeleton that all subsequent phases build on.

The course requires `TreatWarningsAsErrors` enabled. The architecture must enforce that dependencies only point inward (Presentation -> Infrastructure -> Application -> Domain).

## Goals / Non-Goals

**Goals:**
- Establish a solution structure where `dotnet build` and `dotnet test` succeed on empty projects
- Enforce the dependency rule via automated architecture tests from day one
- Centralize build configuration so all projects share the same framework and compiler settings
- Install NuGet packages needed by Infrastructure (EF Core) and Tests (xUnit) so later phases can use them immediately

**Non-Goals:**
- No game logic, UI, persistence, or business code — that belongs to phases 1-6
- No CI/CD pipeline setup
- No WebApp project yet (deferred to phase 6)
- No EF Core DbContext, migrations, or persistence code

## Decisions

### 1. Use `Directory.Build.props` for shared settings
All five projects target the same framework (net9.0) and share the same compiler options. A single `Directory.Build.props` at the repo root avoids duplication and ensures consistency.

**Alternative considered:** Per-project settings in each `.csproj`. Rejected because it creates drift risk and requires updating multiple files for framework upgrades.

### 2. Architecture tests via reflection on assembly references
Use `typeof(SomeType).Assembly.GetReferencedAssemblies()` in xUnit tests to verify that each layer only references permitted layers. This is zero-dependency (no extra NuGet like NetArchTest) and runs as part of the normal test suite.

**Alternative considered:** NetArchTest or ArchUnitNET. Rejected for phase 0 to avoid an extra dependency when simple reflection checks are sufficient for assembly-level rules.

### 3. ConsoleApp references Application + Infrastructure (not Domain directly)
ConsoleApp acts as a composition root. It references Infrastructure (which transitively brings Domain via Application). A direct ConsoleApp -> Domain reference is unnecessary but harmless for DI registration if needed. The roadmap specifies ConsoleApp -> Application + Infrastructure.

### 4. Test project references Domain + Application + Infrastructure
The test project needs access to all layers to run architecture tests (inspecting assembly references) and to support unit/integration tests in later phases. It does not reference ConsoleApp or WebApp.

## Risks / Trade-offs

- **[Risk] Empty projects have no types for reflection** → Architecture tests will reference the assembly by project reference. Even empty assemblies can be loaded; tests will assert on `GetReferencedAssemblies()`. Each project will contain a minimal marker class if needed to ensure the assembly is emitted.
- **[Risk] `TreatWarningsAsErrors` may block builds on generated code warnings** → Mitigated by using `<NoWarn>` for specific generated code warning codes if they arise in later phases. For phase 0, empty projects produce no warnings.
- **[Trade-off] No WebApp project yet** → Keeps phase 0 focused. Architecture tests will be updated in phase 6 when WebApp is added.
