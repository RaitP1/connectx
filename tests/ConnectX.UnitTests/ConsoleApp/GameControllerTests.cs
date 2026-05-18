using Application.Config.Interfaces;
using Application.Game.Dto;
using Application.Game.Interfaces;
using ConsoleApp;
using ConnectX.TestHelpers.Builders;
using Domain;

namespace ConnectX.UnitTests.ConsoleApp;

public class InMemoryConfigRepository : IConfigRepository
{
    private readonly Dictionary<string, GameConfig> _configs = new();

    public IReadOnlyList<GameConfig> List() => _configs.Values.ToList();
    public void Save(GameConfig config) => _configs[config.Name] = config;
    public GameConfig? Load(string name) => _configs.GetValueOrDefault(name);
    public void Delete(string name) => _configs.Remove(name);
}

public class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<string, GameStateDto> _games = new();

    public IReadOnlyList<GameStateDto> List() => _games.Values.ToList();
    public void Save(GameStateDto state) => _games[state.Name] = state;
    public GameStateDto? Load(string name) => _games.GetValueOrDefault(name);
    public void Delete(string name) => _games.Remove(name);
}

public class DefaultConfigSeederTests
{
    [Fact]
    public void SeedDefaults_WhenEmpty_CreatesFourPresets()
    {
        var repo = new InMemoryConfigRepository();

        DefaultConfigSeeder.Seed(repo);

        var configs = repo.List();
        Assert.Equal(4, configs.Count);
        Assert.Contains(configs, c => c.Name == "Classical");
        Assert.Contains(configs, c => c.Name == "Connect-3");
        Assert.Contains(configs, c => c.Name == "Connect-5");
        Assert.Contains(configs, c => c.Name == "Connect-4 Cylinder");
    }

    [Fact]
    public void SeedDefaults_WhenNotEmpty_DoesNotSeed()
    {
        var repo = new InMemoryConfigRepository();
        repo.Save(ConfigBuilder.Standard("Existing"));

        DefaultConfigSeeder.Seed(repo);

        var configs = repo.List();
        Assert.Single(configs);
        Assert.Equal("Existing", configs[0].Name);
    }

    [Fact]
    public void Classical_Is7x6Connect4Rectangle()
    {
        var repo = new InMemoryConfigRepository();
        DefaultConfigSeeder.Seed(repo);

        var classical = repo.Load("Classical");
        Assert.NotNull(classical);
        Assert.Equal(6, classical.Rows);
        Assert.Equal(7, classical.Columns);
        Assert.Equal(4, classical.WinCondition);
        Assert.Equal(EBoardTopology.Rectangle, classical.Topology);
    }

    [Fact]
    public void Connect3_Is5x4Connect3Rectangle()
    {
        var repo = new InMemoryConfigRepository();
        DefaultConfigSeeder.Seed(repo);

        var config = repo.Load("Connect-3");
        Assert.NotNull(config);
        Assert.Equal(4, config.Rows);
        Assert.Equal(5, config.Columns);
        Assert.Equal(3, config.WinCondition);
        Assert.Equal(EBoardTopology.Rectangle, config.Topology);
    }

    [Fact]
    public void Connect5_Is9x7Connect5Rectangle()
    {
        var repo = new InMemoryConfigRepository();
        DefaultConfigSeeder.Seed(repo);

        var config = repo.Load("Connect-5");
        Assert.NotNull(config);
        Assert.Equal(7, config.Rows);
        Assert.Equal(9, config.Columns);
        Assert.Equal(5, config.WinCondition);
        Assert.Equal(EBoardTopology.Rectangle, config.Topology);
    }

    [Fact]
    public void Connect4Cylinder_Is7x6Connect4Cylinder()
    {
        var repo = new InMemoryConfigRepository();
        DefaultConfigSeeder.Seed(repo);

        var config = repo.Load("Connect-4 Cylinder");
        Assert.NotNull(config);
        Assert.Equal(6, config.Rows);
        Assert.Equal(7, config.Columns);
        Assert.Equal(4, config.WinCondition);
        Assert.Equal(EBoardTopology.Cylinder, config.Topology);
    }

    [Fact]
    public void AllPresets_UseHumanPlayersWithDefaultNamesAndSymbols()
    {
        var repo = new InMemoryConfigRepository();
        DefaultConfigSeeder.Seed(repo);

        foreach (var config in repo.List())
        {
            Assert.False(config.Player1Type.IsAI);
            Assert.False(config.Player2Type.IsAI);
            Assert.Equal("Player 1", config.Player1Name);
            Assert.Equal("Player 2", config.Player2Name);
            Assert.Equal("X", config.Player1Symbol);
            Assert.Equal("O", config.Player2Symbol);
        }
    }
}

public class SettingsMutationTests
{
    [Fact]
    public void ChangeBoardWidth_ReturnsNewConfig_OriginalUnchanged()
    {
        var original = ConfigBuilder.Standard();

        var updated = original with { Columns = 9 };

        Assert.Equal(7, original.Columns);
        Assert.Equal(9, updated.Columns);
    }

    [Fact]
    public void ChangeBoardHeight_ReturnsNewConfig_OriginalUnchanged()
    {
        var original = ConfigBuilder.Standard();

        var updated = original with { Rows = 8 };

        Assert.Equal(6, original.Rows);
        Assert.Equal(8, updated.Rows);
    }

    [Fact]
    public void ChangeWinCondition_ReturnsNewConfig()
    {
        var original = ConfigBuilder.Standard();

        var updated = original with { WinCondition = 5 };

        Assert.Equal(4, original.WinCondition);
        Assert.Equal(5, updated.WinCondition);
    }

    [Fact]
    public void ToggleTopology_FromRectangleToCylinder()
    {
        var original = ConfigBuilder.Standard();
        Assert.Equal(EBoardTopology.Rectangle, original.Topology);

        var updated = original with { Topology = EBoardTopology.Cylinder };

        Assert.Equal(EBoardTopology.Rectangle, original.Topology);
        Assert.Equal(EBoardTopology.Cylinder, updated.Topology);
    }

    [Fact]
    public void ConfigurePlayerAsAI_WithDifficulty()
    {
        var original = ConfigBuilder.Standard();

        var updated = original with { Player1Type = new PlayerType(true, EAIDifficulty.Hard) };

        Assert.False(original.Player1Type.IsAI);
        Assert.True(updated.Player1Type.IsAI);
        Assert.Equal(EAIDifficulty.Hard, updated.Player1Type.Difficulty);
    }

    [Fact]
    public void ConfigurePlayerAsHuman()
    {
        var original = ConfigBuilder.WithAI();
        Assert.True(original.Player2Type.IsAI);

        var updated = original with { Player2Type = new PlayerType(false) };

        Assert.False(updated.Player2Type.IsAI);
    }
}
