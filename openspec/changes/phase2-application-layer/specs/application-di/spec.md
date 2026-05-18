## ADDED Requirements

### Requirement: Application service registration extension method
The system SHALL provide a `ServiceCollectionExtensions` class in the `Application` namespace with a static method `AddApplicationServices(this IServiceCollection services)` that registers Application-layer services.

#### Scenario: Extension method is available
- **WHEN** a caller imports the Application namespace and has `Microsoft.Extensions.DependencyInjection`
- **THEN** `AddApplicationServices()` SHALL be available as an extension method on `IServiceCollection`

#### Scenario: Method returns the service collection
- **WHEN** `AddApplicationServices()` is called
- **THEN** it SHALL return the same `IServiceCollection` instance for chaining

### Requirement: Application project depends on DI abstractions
The Application project SHALL reference `Microsoft.Extensions.DependencyInjection.Abstractions` for the `IServiceCollection` type.

#### Scenario: NuGet reference exists
- **WHEN** the Application project file is inspected
- **THEN** it SHALL contain a PackageReference to `Microsoft.Extensions.DependencyInjection.Abstractions`
