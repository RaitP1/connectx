# ConnectX

A Connect Four game with console and web interfaces, supporting multiplayer, AI opponents, and cross-app game continuity.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Running

### Web App (Razor Pages)

```bash
dotnet run --project src/WebApp
```

Opens at `http://localhost:5000`. Features: game lobby, multiplayer (two browsers), AI opponents, config management, piece-drop animations.

### Console App

```bash
dotnet run --project src/ConsoleApp
```

Arrow keys to navigate menus and place pieces.

### Persistence Modes

Both apps default to **EF/SQLite** with a shared database at `~/ConnectX/ConnectX.db`, so games saved in one app appear in the other.

To use **JSON file** persistence instead:

```bash
# Console
dotnet run --project src/ConsoleApp -- --json

# Web
dotnet run --project src/WebApp -- --Persistence=Json
```

JSON files are stored in `~/ConnectX/config/` and `~/ConnectX/savegames/`.

## Running Tests

```bash
dotnet test
```

## Project Structure

```
src/
  Domain/          Core game logic (GameBrain, GameConfig)
  Application/     Services, DTOs, mappers, AI, repository interfaces
  Infrastructure/  EF + JSON persistence implementations
  ConsoleApp/      Console UI
  WebApp/          ASP.NET Core Razor Pages UI

tests/
  ConnectX.UnitTests/          Domain and application logic
  ConnectX.IntegrationTests/   EF and JSON persistence
  ConnectX.WebTests/           Web app integration tests
```
