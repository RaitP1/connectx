using ConnectX.TestHelpers.Builders;
using Domain;
using Infrastructure.Persistence.Json;

namespace ConnectX.IntegrationTests.Persistence.Json;

public sealed class JsonGameRepositoryTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), $"connectx-test-{Guid.NewGuid()}");
    private readonly JsonGameRepository _repo;

    public JsonGameRepositoryTests()
    {
        _repo = new JsonGameRepository(new FilesystemHelper(_tempDir));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public void Save_CreatesJsonFile()
    {
        var state = GameStateBuilder.FromConfig(ConfigBuilder.Standard(), "game-1");

        _repo.Save(state);

        var files = Directory.GetFiles(Path.Combine(_tempDir, "savegames"), "*.json");
        Assert.Single(files);
    }

    [Fact]
    public void Save_OverwritesExistingState()
    {
        var config = ConfigBuilder.Standard();
        var state = GameStateBuilder.FromConfig(config, "game-1");
        _repo.Save(state);
        var updated = state with { CurrentPlayer = 1 };

        _repo.Save(updated);

        var loaded = _repo.Load("game-1");
        Assert.NotNull(loaded);
        Assert.Equal(1, loaded.CurrentPlayer);
    }

    [Fact]
    public void Load_ReturnsNullForNonExistent()
    {
        var result = _repo.Load("does-not-exist");

        Assert.Null(result);
    }

    [Fact]
    public void Load_RoundTripsBoard()
    {
        var config = ConfigBuilder.Standard();
        var state = GameStateBuilder.WithMoves(config, "board-test");
        _repo.Save(state);

        var loaded = _repo.Load("board-test");

        Assert.NotNull(loaded);
        for (var r = 0; r < config.Rows; r++)
        for (var c = 0; c < config.Columns; c++)
            Assert.Equal(state.Board[r][c], loaded.Board[r][c]);
    }

    [Fact]
    public void Load_RoundTripsConfigWithinState()
    {
        var config = ConfigBuilder.Cylinder("CylConfig") with
        {
            Player2Type = new PlayerType(true, EAIDifficulty.Hard)
        };
        var state = GameStateBuilder.FromConfig(config, "config-test");
        _repo.Save(state);

        var loaded = _repo.Load("config-test");

        Assert.NotNull(loaded);
        Assert.Equal(config, loaded.Config);
    }

    [Fact]
    public void Load_RoundTripsCurrentPlayerAndTimestamp()
    {
        var config = ConfigBuilder.Standard();
        var savedAt = new DateTime(2026, 5, 18, 12, 0, 0, DateTimeKind.Utc);
        var state = GameStateBuilder.FromConfig(config, "time-test") with
        {
            CurrentPlayer = 1,
            SavedAt = savedAt
        };
        _repo.Save(state);

        var loaded = _repo.Load("time-test");

        Assert.NotNull(loaded);
        Assert.Equal(1, loaded.CurrentPlayer);
        Assert.Equal(savedAt, loaded.SavedAt);
    }

    [Fact]
    public void List_ReturnsEmptyWhenNoGames()
    {
        var result = _repo.List();

        Assert.Empty(result);
    }

    [Fact]
    public void List_ReturnsAllSavedGames()
    {
        var config = ConfigBuilder.Standard();
        _repo.Save(GameStateBuilder.FromConfig(config, "save-1"));
        _repo.Save(GameStateBuilder.FromConfig(config, "save-2"));

        var result = _repo.List();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Delete_RemovesExistingGame()
    {
        var config = ConfigBuilder.Standard();
        _repo.Save(GameStateBuilder.FromConfig(config, "to-delete"));

        _repo.Delete("to-delete");

        Assert.Null(_repo.Load("to-delete"));
    }

    [Fact]
    public void Delete_DoesNotThrowForNonExistent()
    {
        var exception = Record.Exception(() => _repo.Delete("does-not-exist"));

        Assert.Null(exception);
    }

    [Fact]
    public void List_SkipsCorruptJsonFiles()
    {
        var config = ConfigBuilder.Standard();
        _repo.Save(GameStateBuilder.FromConfig(config, "valid-save"));
        var corruptPath = Path.Combine(_tempDir, "savegames", "corrupt.json");
        File.WriteAllText(corruptPath, "not valid json {{{");

        var result = _repo.List();

        Assert.Single(result);
        Assert.Equal("valid-save", result[0].Name);
    }
}
