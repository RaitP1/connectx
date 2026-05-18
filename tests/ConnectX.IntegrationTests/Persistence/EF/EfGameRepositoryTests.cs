using Application.Game.Interfaces;
using ConnectX.TestHelpers.Builders;
using Domain;
using Infrastructure.Persistence.EF;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ConnectX.IntegrationTests.Persistence.EF;

public sealed class EfGameRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly EfGameRepository _repo;

    public EfGameRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _repo = new EfGameRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public void Implements_IGameRepository()
    {
        Assert.IsAssignableFrom<IGameRepository>(_repo);
    }

    [Fact]
    public void Save_PersistsNewGameState()
    {
        var state = GameStateBuilder.FromConfig(ConfigBuilder.Standard(), "Game1");

        _repo.Save(state);

        var loaded = _repo.Load("Game1");
        Assert.NotNull(loaded);
        Assert.Equal("Game1", loaded.Name);
    }

    [Fact]
    public void Save_OverwritesExistingGameState()
    {
        var config = ConfigBuilder.Standard();
        _repo.Save(GameStateBuilder.FromConfig(config, "Game1"));
        var updated = GameStateBuilder.FromConfig(config, "Game1") with { CurrentPlayer = 1 };

        _repo.Save(updated);

        var loaded = _repo.Load("Game1");
        Assert.NotNull(loaded);
        Assert.Equal(1, loaded.CurrentPlayer);
    }

    [Fact]
    public void Load_ReturnsNullForNonExistent()
    {
        var result = _repo.Load("Missing");

        Assert.Null(result);
    }

    [Fact]
    public void Load_RoundTripsBoard()
    {
        var config = ConfigBuilder.Standard();
        var state = GameStateBuilder.WithMoves(config, "WithMoves");

        _repo.Save(state);

        var loaded = _repo.Load("WithMoves");
        Assert.NotNull(loaded);
        Assert.Equal(state.Board.Length, loaded.Board.Length);
        for (var r = 0; r < state.Board.Length; r++)
        {
            Assert.Equal(state.Board[r].Length, loaded.Board[r].Length);
            for (var c = 0; c < state.Board[r].Length; c++)
            {
                Assert.Equal(state.Board[r][c], loaded.Board[r][c]);
            }
        }
    }

    [Fact]
    public void Load_RoundTripsEmptyBoard()
    {
        var config = ConfigBuilder.Standard();
        var state = GameStateBuilder.FromConfig(config, "Empty");

        _repo.Save(state);

        var loaded = _repo.Load("Empty");
        Assert.NotNull(loaded);
        Assert.Equal(config.Rows, loaded.Board.Length);
        Assert.All(loaded.Board, row => Assert.All(row, cell => Assert.Null(cell)));
    }

    [Fact]
    public void Load_RoundTripsEmbeddedConfig()
    {
        var config = new GameConfig(
            Name: "CylinderAI",
            Rows: 9,
            Columns: 7,
            WinCondition: 5,
            Player1Name: "Alpha",
            Player1Symbol: "A",
            Player2Name: "Beta",
            Player2Symbol: "B",
            Topology: EBoardTopology.Cylinder,
            Player1Type: new PlayerType(true, EAIDifficulty.Easy),
            Player2Type: new PlayerType(true, EAIDifficulty.Hard));
        var state = GameStateBuilder.FromConfig(config, "ConfigTest");

        _repo.Save(state);

        var loaded = _repo.Load("ConfigTest");
        Assert.NotNull(loaded);
        Assert.Equal(config, loaded.Config);
    }

    [Fact]
    public void Load_PreservesSavedAtTimestamp()
    {
        var timestamp = new DateTime(2025, 6, 15, 14, 30, 0, DateTimeKind.Utc);
        var state = GameStateBuilder.FromConfig(ConfigBuilder.Standard(), "Timed") with
        {
            SavedAt = timestamp
        };

        _repo.Save(state);

        var loaded = _repo.Load("Timed");
        Assert.NotNull(loaded);
        Assert.Equal(timestamp, loaded.SavedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void List_ReturnsEmptyWhenNoStates()
    {
        var result = _repo.List();

        Assert.Empty(result);
    }

    [Fact]
    public void List_ReturnsAllSavedStates()
    {
        var config = ConfigBuilder.Standard();
        _repo.Save(GameStateBuilder.FromConfig(config, "A"));
        _repo.Save(GameStateBuilder.FromConfig(config, "B"));
        _repo.Save(GameStateBuilder.FromConfig(config, "C"));

        var result = _repo.List();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Delete_RemovesExistingState()
    {
        _repo.Save(GameStateBuilder.FromConfig(ConfigBuilder.Standard(), "OldGame"));

        _repo.Delete("OldGame");

        Assert.Null(_repo.Load("OldGame"));
    }

    [Fact]
    public void Delete_DoesNotThrowForNonExistent()
    {
        var exception = Record.Exception(() => _repo.Delete("Ghost"));

        Assert.Null(exception);
    }
}
