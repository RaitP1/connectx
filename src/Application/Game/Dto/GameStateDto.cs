using Domain;

namespace Application.Game.Dto;

public sealed record GameStateDto(
    string Name,
    GameConfig Config,
    int?[][] Board,
    int CurrentPlayer,
    DateTime SavedAt);
