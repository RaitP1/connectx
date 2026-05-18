## Why

The Domain layer has a working game engine and the Application layer has AI, but there are no contracts for persistence or mapping between Domain and external representations. Without repository interfaces, Infrastructure cannot implement save/load. Without DTOs and mappers, no layer can serialize or deserialize game state. This is the bridge that unlocks Phases 3-6.

## What Changes

- Add `IConfigRepository` interface for CRUD operations on game configurations
- Add `IGameRepository` interface for CRUD operations on saved game states
- Add `GameStateDto` as the serialization-friendly snapshot of a game in progress (jagged array board, config, current player, timestamp)
- Add `GameStateMapper` to convert between `GameBrain` and `GameStateDto` in both directions
- Add `ServiceCollectionExtensions` to register Application-layer services via DI
- Add unit tests for mapper round-trip fidelity (board state, topology, player types, current player)

## Capabilities

### New Capabilities
- `config-repository`: Repository interface for listing, saving, loading, and deleting game configurations
- `game-repository`: Repository interface for listing, saving, loading, and deleting saved game states
- `game-state-dto`: Data transfer object representing a serializable snapshot of game state
- `game-state-mapping`: Bidirectional mapping between GameBrain (domain) and GameStateDto (application boundary)
- `application-di`: Dependency injection registration for Application-layer services

### Modified Capabilities

## Impact

- `src/Application/Config/Interfaces/` -- new `IConfigRepository.cs`
- `src/Application/Game/Interfaces/` -- new `IGameRepository.cs`
- `src/Application/Game/Dto/` -- new `GameStateDto.cs`
- `src/Application/Game/Mapping/` -- new `GameStateMapper.cs`
- `src/Application/` -- new `ServiceCollectionExtensions.cs`
- `tests/` -- new mapper unit tests
- No changes to Domain, Infrastructure, or ConsoleApp
- New NuGet dependency: `Microsoft.Extensions.DependencyInjection.Abstractions` for Application project
