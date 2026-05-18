using ConnectX.TestHelpers.Builders;
using Domain;
using Infrastructure.Persistence.Json;

namespace ConnectX.IntegrationTests.Persistence.Json;

public sealed class JsonConfigRepositoryTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), $"connectx-test-{Guid.NewGuid()}");
    private readonly JsonConfigRepository _repo;

    public JsonConfigRepositoryTests()
    {
        _repo = new JsonConfigRepository(new FilesystemHelper(_tempDir));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public void Save_CreatesJsonFile()
    {
        var config = ConfigBuilder.Standard("Classical");

        _repo.Save(config);

        var files = Directory.GetFiles(Path.Combine(_tempDir, "config"), "*.json");
        Assert.Single(files);
    }

    [Fact]
    public void Save_OverwritesExistingConfig()
    {
        var config = ConfigBuilder.Standard("Classical");
        _repo.Save(config);
        var updated = config with { Rows = 8 };

        _repo.Save(updated);

        var loaded = _repo.Load("Classical");
        Assert.NotNull(loaded);
        Assert.Equal(8, loaded.Rows);
    }

    [Fact]
    public void Load_ReturnsNullForNonExistent()
    {
        var result = _repo.Load("does-not-exist");

        Assert.Null(result);
    }

    [Fact]
    public void Load_RoundTripsAllFields()
    {
        var config = new GameConfig(
            Name: "Full-Test",
            Rows: 9,
            Columns: 7,
            WinCondition: 5,
            Player1Name: "Alice",
            Player1Symbol: "A",
            Player2Name: "Bob",
            Player2Symbol: "B",
            Topology: EBoardTopology.Cylinder,
            Player1Type: new PlayerType(false),
            Player2Type: new PlayerType(true, EAIDifficulty.Hard));
        _repo.Save(config);

        var loaded = _repo.Load("Full-Test");

        Assert.NotNull(loaded);
        Assert.Equal(config, loaded);
    }

    [Fact]
    public void Load_PreservesEnumValues()
    {
        var config = ConfigBuilder.Cylinder("CylinderTest") with
        {
            Player2Type = new PlayerType(true, EAIDifficulty.Hard)
        };
        _repo.Save(config);

        var loaded = _repo.Load("CylinderTest");

        Assert.NotNull(loaded);
        Assert.Equal(EBoardTopology.Cylinder, loaded.Topology);
        Assert.Equal(EAIDifficulty.Hard, loaded.Player2Type.Difficulty);
    }

    [Fact]
    public void List_ReturnsEmptyWhenNoConfigs()
    {
        var result = _repo.List();

        Assert.Empty(result);
    }

    [Fact]
    public void List_ReturnsAllSavedConfigs()
    {
        _repo.Save(ConfigBuilder.Standard("Config1"));
        _repo.Save(ConfigBuilder.Standard("Config2"));
        _repo.Save(ConfigBuilder.Standard("Config3"));

        var result = _repo.List();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Delete_RemovesExistingConfig()
    {
        _repo.Save(ConfigBuilder.Standard("ToDelete"));

        _repo.Delete("ToDelete");

        Assert.Null(_repo.Load("ToDelete"));
    }

    [Fact]
    public void Delete_DoesNotThrowForNonExistent()
    {
        var exception = Record.Exception(() => _repo.Delete("does-not-exist"));

        Assert.Null(exception);
    }

    [Fact]
    public void Save_SanitizesFilename()
    {
        var config = ConfigBuilder.Standard("My Config!@#");

        _repo.Save(config);

        var loaded = _repo.Load("My Config!@#");
        Assert.NotNull(loaded);
    }

    [Fact]
    public void List_SkipsCorruptJsonFiles()
    {
        _repo.Save(ConfigBuilder.Standard("Valid"));
        var corruptPath = Path.Combine(_tempDir, "config", "corrupt.json");
        File.WriteAllText(corruptPath, "not valid json {{{");

        var result = _repo.List();

        Assert.Single(result);
        Assert.Equal("Valid", result[0].Name);
    }
}
