using Domain;

namespace Application.Config.Interfaces;

public interface IConfigRepository
{
    IReadOnlyList<GameConfig> List();
    void Save(GameConfig config);
    GameConfig? Load(string name);
    void Delete(string name);
}
