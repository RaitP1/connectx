## ADDED Requirements

### Requirement: WebApp project with MVC configuration
The WebApp `Program.cs` SHALL configure an ASP.NET MVC application with Razor views, session support, and routing.

#### Scenario: Application starts and serves pages
- **WHEN** the WebApp is launched
- **THEN** the application starts, listens on a configured port, and serves the home page at the root URL

#### Scenario: MVC routing configured
- **WHEN** a request is made to `/Game/New`
- **THEN** the request is routed to the GameController's New action

### Requirement: EF persistence wired via DI
The WebApp SHALL register EF Core persistence using `AddEfPersistence` from the Infrastructure layer, with SQLite as the database.

#### Scenario: Repositories resolve from DI
- **WHEN** the application starts with EF persistence configured
- **THEN** `IConfigRepository` resolves to `EfConfigRepository` and `IGameRepository` resolves to `EfGameRepository`

#### Scenario: Database created on startup
- **WHEN** the application starts for the first time
- **THEN** the SQLite database is created with the required schema

### Requirement: Session configuration for game state
The WebApp SHALL configure session middleware to store in-progress game state across HTTP requests.

#### Scenario: Session stores game state
- **WHEN** a game is started and a move is made across two separate HTTP requests
- **THEN** the game state persists in session between requests and the second request sees the board with the first move applied

### Requirement: Default configuration seeding on startup
The WebApp SHALL seed default configurations on first launch when the config repository is empty, using the same four presets as ConsoleApp: "Classical" (6x7, connect 4, rectangle), "Connect-3" (4x5, connect 3, rectangle), "Connect-5" (7x9, connect 5, rectangle), "Connect-4 Cylinder" (6x7, connect 4, cylinder).

#### Scenario: First launch seeds presets
- **WHEN** the application starts and no configurations exist
- **THEN** four presets are created with human players and default names/symbols

#### Scenario: Subsequent launches skip seeding
- **WHEN** the application starts and configurations already exist
- **THEN** no additional presets are created

### Requirement: Home page with navigation
The WebApp SHALL have a home page with links to: start a new game, load a saved game, and manage configurations.

#### Scenario: Home page rendered
- **WHEN** the user visits the root URL
- **THEN** the page displays the game title and navigation links for New Game, Load Game, and Configurations

### Requirement: Shared layout with navigation
The WebApp SHALL use a Razor layout with consistent navigation across all pages.

#### Scenario: Navigation present on all pages
- **WHEN** the user visits any page in the application
- **THEN** the page includes a navigation bar with links to Home, New Game, and Configurations

### Requirement: Solution and project references
The WebApp project SHALL reference Application, Infrastructure, and Domain projects. It SHALL use the ASP.NET web SDK.

#### Scenario: Project compiles with all references
- **WHEN** the solution is built
- **THEN** the WebApp project compiles successfully with access to all required types from Domain, Application, and Infrastructure
