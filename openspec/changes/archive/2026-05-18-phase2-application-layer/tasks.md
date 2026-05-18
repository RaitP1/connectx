## 1. Domain — Internal Restoration Constructor

- [x] 1.1 Add `[InternalsVisibleTo("Application")]` attribute to Domain assembly (in DomainMarker.cs or AssemblyInfo)
- [x] 1.2 Change `GameBrain` private constructor to `internal` for state restoration (config, board, currentPlayer, history)
- [x] 1.3 Write unit test verifying Application assembly can access internal constructor (compilation test — if it builds, it works)

## 2. Game State DTO

- [x] 2.1 Create `Application/Game/Dto/GameStateDto.cs` as an immutable record with properties: Name, Config, Board (`int?[][]`), CurrentPlayer, SavedAt
- [x] 2.2 Write unit test verifying DTO is a record with expected properties

## 3. Game State Mapper

- [x] 3.1 Create `Application/Game/Mapping/GameStateMapper.cs` with static `ToDto(GameBrain brain, string name)` method (2D to jagged array conversion, captures config, current player, UTC timestamp)
- [x] 3.2 Implement static `ToDomain(GameStateDto dto)` method (jagged to 2D array conversion, reconstruct GameBrain via internal constructor)
- [x] 3.3 Write unit tests for `ToDto`: board state captured, current player captured, config captured, name set, SavedAt is recent
- [x] 3.4 Write unit tests for `ToDomain`: board state restored, current player restored, config restored, cylinder topology preserved
- [x] 3.5 Write unit tests for round-trip fidelity: brain -> DTO -> brain produces identical observable state (cells, current player, game-over, winner)
- [x] 3.6 Write unit test for round-trip with mid-game state (several moves played, no winner yet)

## 4. Repository Interfaces

- [x] 4.1 Create `Application/Config/Interfaces/IConfigRepository.cs` with methods: `List()`, `Save(GameConfig)`, `Load(string name)`, `Delete(string name)`
- [x] 4.2 Create `Application/Game/Interfaces/IGameRepository.cs` with methods: `List()`, `Save(GameStateDto)`, `Load(string name)`, `Delete(string name)`

## 5. Dependency Injection

- [x] 5.1 Add `Microsoft.Extensions.DependencyInjection.Abstractions` NuGet package to Application project
- [x] 5.2 Create `Application/ServiceCollectionExtensions.cs` with `AddApplicationServices(this IServiceCollection)` extension method
- [x] 5.3 Write unit test verifying `AddApplicationServices` returns the same `IServiceCollection` for chaining

## 6. Verification

- [x] 6.1 Run `dotnet build` — solution compiles with no warnings (TreatWarningsAsErrors)
- [x] 6.2 Run `dotnet test` — all existing architecture tests still pass
- [x] 6.3 Run `dotnet test` — all new unit tests pass
- [x] 6.4 Verify test coverage >= 80% for new Application code
