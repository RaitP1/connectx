using Application.Config.Interfaces;
using ConnectX.TestHelpers.Builders;
using Domain;
using Infrastructure.Persistence.EF;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ConnectX.IntegrationTests.Persistence.EF;

public sealed class EfConfigRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly EfConfigRepository _repo;

    public EfConfigRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _repo = new EfConfigRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public void Implements_IConfigRepository()
    {
        Assert.IsAssignableFrom<IConfigRepository>(_repo);
    }

    [Fact]
    public void Save_PersistsNewConfig()
    {
        var config = ConfigBuilder.Standard("Test");

        _repo.Save(config);

        var loaded = _repo.Load("Test");
        Assert.NotNull(loaded);
        Assert.Equal("Test", loaded.Name);
    }

    [Fact]
    public void Save_OverwritesExistingConfig()
    {
        _repo.Save(ConfigBuilder.Standard("Test"));
        _repo.Save(ConfigBuilder.Standard("Test") with { Rows = 8 });

        var loaded = _repo.Load("Test");
        Assert.NotNull(loaded);
        Assert.Equal(8, loaded.Rows);
    }

    [Fact]
    public void Load_ReturnsNullForNonExistent()
    {
        var result = _repo.Load("Missing");

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
            Player1Name: "Alpha",
            Player1Symbol: "A",
            Player2Name: "Beta",
            Player2Symbol: "B",
            Topology: EBoardTopology.Cylinder,
            Player1Type: new PlayerType(true, EAIDifficulty.Easy),
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
            Player2Type = new PlayerType(true, EAIDifficulty.Medium)
        };
        _repo.Save(config);

        var loaded = _repo.Load("CylinderTest");

        Assert.NotNull(loaded);
        Assert.Equal(EBoardTopology.Cylinder, loaded.Topology);
        Assert.Equal(EAIDifficulty.Medium, loaded.Player2Type.Difficulty);
    }

    [Fact]
    public void Load_PreservesHumanPlayerType()
    {
        var config = ConfigBuilder.Standard("Human");
        _repo.Save(config);

        var loaded = _repo.Load("Human");

        Assert.NotNull(loaded);
        Assert.False(loaded.Player1Type.IsAI);
        Assert.Null(loaded.Player1Type.Difficulty);
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
        _repo.Save(ConfigBuilder.Standard("A"));
        _repo.Save(ConfigBuilder.Standard("B"));
        _repo.Save(ConfigBuilder.Standard("C"));

        var result = _repo.List();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Delete_RemovesExistingConfig()
    {
        _repo.Save(ConfigBuilder.Standard("Old"));

        _repo.Delete("Old");

        Assert.Null(_repo.Load("Old"));
    }

    [Fact]
    public void Delete_DoesNotThrowForNonExistent()
    {
        var exception = Record.Exception(() => _repo.Delete("Ghost"));

        Assert.Null(exception);
    }
}
