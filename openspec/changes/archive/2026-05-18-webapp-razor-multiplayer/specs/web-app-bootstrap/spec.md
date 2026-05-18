## ADDED Requirements

### Requirement: Razor Pages application host
The web application SHALL be an ASP.NET Core application using Razor Pages (not MVC controllers). `Program.cs` SHALL configure Razor Pages services, session middleware, and EF persistence.

#### Scenario: Application starts with Razor Pages
- **WHEN** the web application launches
- **THEN** Razor Pages are registered, session middleware is active, and EF persistence is configured with SQLite

#### Scenario: Static files served
- **WHEN** a request is made for a CSS or JS file under `/wwwroot/`
- **THEN** the static file middleware SHALL serve the file

### Requirement: Session configuration
The web application SHALL configure session with a 30-minute idle timeout, HttpOnly cookies, SameSite=Strict, and IsEssential=true.

#### Scenario: Session cookie properties
- **WHEN** a new session is created
- **THEN** the session cookie SHALL be HttpOnly, SameSite=Strict, and marked essential

#### Scenario: Session idle timeout
- **WHEN** a session is inactive for more than 30 minutes
- **THEN** the session SHALL expire

### Requirement: EF database initialization
On startup, the web application SHALL ensure the database is created and seed default configurations if none exist.

#### Scenario: First launch creates database and seeds configs
- **WHEN** the web app starts and no database exists
- **THEN** the database SHALL be created and four default configurations SHALL be seeded

#### Scenario: Subsequent launches skip seeding
- **WHEN** the web app starts and configurations already exist
- **THEN** no additional configurations SHALL be created

### Requirement: Shared layout
The web application SHALL provide a shared layout with navigation links to Home, New Game, Load Game, and Configurations.

#### Scenario: Navigation present on all pages
- **WHEN** any page is rendered
- **THEN** the layout SHALL include navigation links to the home page, new game, load game, and configuration management
