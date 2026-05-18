using Application.Game.Dto;
using Application.Game.Mapping;
using Domain;

namespace ConnectX.UnitTests.Application.Mapping;

public sealed class GameStateMapperTests
{
    private static GameConfig StandardConfig(EBoardTopology topology = EBoardTopology.Rectangle) => new(
        Name: "Test",
        Rows: 6,
        Columns: 7,
        WinCondition: 4,
        Player1Name: "Player 1",
        Player1Symbol: "X",
        Player2Name: "Player 2",
        Player2Symbol: "O",
        Topology: topology,
        Player1Type: new PlayerType(false),
        Player2Type: new PlayerType(true, EAIDifficulty.Hard));

    [Fact]
    public void ToDto_CapturesBoardState()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(3);
        brain.MakeMove(4);

        var dto = GameStateMapper.ToDto(brain, "test-save");

        Assert.Equal(0, dto.Board[5][3]);
        Assert.Equal(1, dto.Board[5][4]);
        Assert.Null(dto.Board[0][0]);
    }

    [Fact]
    public void ToDto_CapturesCurrentPlayer()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(0);

        var dto = GameStateMapper.ToDto(brain, "test");

        Assert.Equal(1, dto.CurrentPlayer);
    }

    [Fact]
    public void ToDto_CapturesConfig()
    {
        var config = StandardConfig();
        var brain = new GameBrain(config);

        var dto = GameStateMapper.ToDto(brain, "test");

        Assert.Equal(config, dto.Config);
    }

    [Fact]
    public void ToDto_SetsName()
    {
        var brain = new GameBrain(StandardConfig());

        var dto = GameStateMapper.ToDto(brain, "my-save");

        Assert.Equal("my-save", dto.Name);
    }

    [Fact]
    public void ToDto_SetsSavedAtToRecentTime()
    {
        var brain = new GameBrain(StandardConfig());
        var before = DateTime.UtcNow;

        var dto = GameStateMapper.ToDto(brain, "test");

        var after = DateTime.UtcNow;
        Assert.InRange(dto.SavedAt, before, after);
    }

    [Fact]
    public void ToDto_ConvertsBoardToJaggedArray()
    {
        var brain = new GameBrain(StandardConfig());

        var dto = GameStateMapper.ToDto(brain, "test");

        Assert.Equal(6, dto.Board.Length);
        foreach (var row in dto.Board)
            Assert.Equal(7, row.Length);
    }

    [Fact]
    public void ToDomain_RestoresBoardState()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(3);
        brain.MakeMove(4);
        var dto = GameStateMapper.ToDto(brain, "test");

        var restored = GameStateMapper.ToDomain(dto);

        Assert.Equal(0, restored.GetCell(5, 3));
        Assert.Equal(1, restored.GetCell(5, 4));
        Assert.Null(restored.GetCell(0, 0));
    }

    [Fact]
    public void ToDomain_RestoresCurrentPlayer()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(0);
        var dto = GameStateMapper.ToDto(brain, "test");

        var restored = GameStateMapper.ToDomain(dto);

        Assert.Equal(1, restored.CurrentPlayer);
    }

    [Fact]
    public void ToDomain_RestoresConfig()
    {
        var config = StandardConfig();
        var brain = new GameBrain(config);
        var dto = GameStateMapper.ToDto(brain, "test");

        var restored = GameStateMapper.ToDomain(dto);

        Assert.Equal(config, restored.Config);
    }

    [Fact]
    public void ToDomain_RestoresCylinderTopology()
    {
        var config = StandardConfig(EBoardTopology.Cylinder);
        var brain = new GameBrain(config);
        brain.MakeMove(6);
        var dto = GameStateMapper.ToDto(brain, "test");

        var restored = GameStateMapper.ToDomain(dto);

        Assert.Equal(EBoardTopology.Cylinder, restored.Config.Topology);
        Assert.Equal(0, restored.GetCell(5, 6));
    }

    [Fact]
    public void RoundTrip_PreservesAllCells()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(0);
        brain.MakeMove(1);
        brain.MakeMove(2);

        var restored = GameStateMapper.ToDomain(GameStateMapper.ToDto(brain, "test"));

        for (var r = 0; r < brain.Rows; r++)
        for (var c = 0; c < brain.Columns; c++)
            Assert.Equal(brain.GetCell(r, c), restored.GetCell(r, c));
    }

    [Fact]
    public void RoundTrip_PreservesCurrentPlayer()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(0);

        var restored = GameStateMapper.ToDomain(GameStateMapper.ToDto(brain, "test"));

        Assert.Equal(brain.CurrentPlayer, restored.CurrentPlayer);
    }

    [Fact]
    public void RoundTrip_PreservesGameOverState()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(0); brain.MakeMove(1);
        brain.MakeMove(0); brain.MakeMove(1);
        brain.MakeMove(0); brain.MakeMove(1);
        brain.MakeMove(0); // player 0 wins vertical

        var restored = GameStateMapper.ToDomain(GameStateMapper.ToDto(brain, "test"));

        Assert.Equal(brain.IsGameOver, restored.IsGameOver);
        Assert.Equal(brain.Winner, restored.Winner);
    }

    [Fact]
    public void ToDomain_ThrowsOnMismatchedBoardDimensions()
    {
        var config = StandardConfig();
        var badDto = new GameStateDto(
            Name: "bad",
            Config: config,
            Board: new int?[3][], // 3 rows instead of 6
            CurrentPlayer: 0,
            SavedAt: DateTime.UtcNow);
        for (var i = 0; i < 3; i++)
            badDto.Board[i] = new int?[7];

        Assert.Throws<ArgumentException>(() => GameStateMapper.ToDomain(badDto));
    }

    [Fact]
    public void ToDomain_ThrowsOnMismatchedColumnCount()
    {
        var config = StandardConfig();
        var board = new int?[6][];
        for (var i = 0; i < 6; i++)
            board[i] = new int?[5]; // 5 cols instead of 7

        var badDto = new GameStateDto("bad", config, board, 0, DateTime.UtcNow);

        Assert.Throws<ArgumentException>(() => GameStateMapper.ToDomain(badDto));
    }

    [Fact]
    public void RoundTrip_MidGameState_PreservesAllState()
    {
        var brain = new GameBrain(StandardConfig(EBoardTopology.Cylinder));
        brain.MakeMove(0); brain.MakeMove(6);
        brain.MakeMove(3); brain.MakeMove(2);
        brain.MakeMove(5);

        var restored = GameStateMapper.ToDomain(GameStateMapper.ToDto(brain, "mid-game"));

        Assert.False(restored.IsGameOver);
        Assert.Equal(brain.CurrentPlayer, restored.CurrentPlayer);
        for (var r = 0; r < brain.Rows; r++)
        for (var c = 0; c < brain.Columns; c++)
            Assert.Equal(brain.GetCell(r, c), restored.GetCell(r, c));
    }
}
