using System.Text.Json;
using Application.Game.Dto;
using Application.Game.Interfaces;

namespace Infrastructure.Persistence.Json;

public sealed class JsonGameRepository : IGameRepository
{
    private readonly FilesystemHelper _fs;

    public JsonGameRepository(FilesystemHelper fs)
    {
        _fs = fs;
    }

    public IReadOnlyList<GameStateDto> List()
    {
        var dir = _fs.GetSavegamesDirectory();
        var files = Directory.GetFiles(dir, "*.json");
        var states = new List<GameStateDto>();

        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var state = JsonSerializer.Deserialize<GameStateDto>(json, JsonPersistenceOptions.Default);
                if (state is not null)
                    states.Add(state);
            }
            catch (JsonException)
            {
            }
        }

        return states.AsReadOnly();
    }

    public void Save(GameStateDto state)
    {
        var dir = _fs.GetSavegamesDirectory();
        var path = _fs.ResolvePath(dir, state.Name);
        var json = JsonSerializer.Serialize(state, JsonPersistenceOptions.Default);
        File.WriteAllText(path, json);
    }

    public GameStateDto? Load(string name)
    {
        var dir = _fs.GetSavegamesDirectory();
        var path = _fs.ResolvePath(dir, name);

        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameStateDto>(json, JsonPersistenceOptions.Default);
    }

    public void Delete(string name)
    {
        var dir = _fs.GetSavegamesDirectory();
        var path = _fs.ResolvePath(dir, name);

        if (File.Exists(path))
            File.Delete(path);
    }
}
