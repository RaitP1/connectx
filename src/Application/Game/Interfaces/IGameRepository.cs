using Application.Game.Dto;

namespace Application.Game.Interfaces;

public interface IGameRepository
{
    IReadOnlyList<GameStateDto> List();
    void Save(GameStateDto state);
    GameStateDto? Load(string name);
    void Delete(string name);
}
