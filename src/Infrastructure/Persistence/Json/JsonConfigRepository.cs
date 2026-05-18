using System.Text.Json;
using Application.Config.Interfaces;
using Domain;

namespace Infrastructure.Persistence.Json;

public sealed class JsonConfigRepository : IConfigRepository
{
    private readonly FilesystemHelper _fs;

    public JsonConfigRepository(FilesystemHelper fs)
    {
        _fs = fs;
    }

    public IReadOnlyList<GameConfig> List()
    {
        var dir = _fs.GetConfigDirectory();
        var files = Directory.GetFiles(dir, "*.json");
        var configs = new List<GameConfig>();

        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var config = JsonSerializer.Deserialize<GameConfig>(json, JsonPersistenceOptions.Default);
                if (config is not null)
                    configs.Add(config);
            }
            catch (JsonException)
            {
            }
        }

        return configs.AsReadOnly();
    }

    public void Save(GameConfig config)
    {
        var dir = _fs.GetConfigDirectory();
        var path = _fs.ResolvePath(dir, config.Name);
        var json = JsonSerializer.Serialize(config, JsonPersistenceOptions.Default);
        File.WriteAllText(path, json);
    }

    public GameConfig? Load(string name)
    {
        var dir = _fs.GetConfigDirectory();
        var path = _fs.ResolvePath(dir, name);

        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameConfig>(json, JsonPersistenceOptions.Default);
    }

    public void Delete(string name)
    {
        var dir = _fs.GetConfigDirectory();
        var path = _fs.ResolvePath(dir, name);

        if (File.Exists(path))
            File.Delete(path);
    }
}
