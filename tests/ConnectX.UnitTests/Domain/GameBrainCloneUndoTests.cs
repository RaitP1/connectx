using Domain;

namespace ConnectX.UnitTests.Domain;

public class GameBrainCloneUndoTests
{
    private static GameConfig StandardConfig() => new(
        Rows: 6,
        Columns: 7,
        WinCondition: 4,
        Player1Name: "Player 1",
        Player1Symbol: "X",
        Player2Name: "Player 2",
        Player2Symbol: "O",
        Topology: EBoardTopology.Rectangle,
        Player1Type: new PlayerType(false),
        Player2Type: new PlayerType(false));

    [Fact]
    public void UndoMove_RestoresCell()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(3);

        brain.UndoMove();

        Assert.Null(brain.GetCell(5, 3));
    }

    [Fact]
    public void UndoMove_RestoresCurrentPlayer()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(3);

        Assert.Equal(1, brain.CurrentPlayer);
        brain.UndoMove();
        Assert.Equal(0, brain.CurrentPlayer);
    }

    [Fact]
    public void UndoMove_NoOpOnEmptyHistory()
    {
        var brain = new GameBrain(StandardConfig());

        var result = brain.UndoMove();

        Assert.False(result);
        Assert.Equal(0, brain.CurrentPlayer);
    }

    [Fact]
    public void Clone_ProducesIndependentCopy()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(3);

        var clone = brain.Clone();
        clone.MakeMove(4);

        Assert.Null(brain.GetCell(5, 4));
        Assert.NotNull(clone.GetCell(5, 4));
    }

    [Fact]
    public void Clone_PreservesState()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(3);
        brain.MakeMove(2);

        var clone = brain.Clone();

        Assert.Equal(brain.CurrentPlayer, clone.CurrentPlayer);
        Assert.Equal(brain.GetCell(5, 3), clone.GetCell(5, 3));
        Assert.Equal(brain.GetCell(5, 2), clone.GetCell(5, 2));
        Assert.Equal(brain.Config, clone.Config);
    }

    [Fact]
    public void UndoMove_AfterWin_RestoresPlayableState()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 4, WinCondition = 2 };
        var brain = new GameBrain(config);

        brain.MakeMove(0); // p0 at (1,0)
        brain.MakeMove(2); // p1 at (1,2)
        brain.MakeMove(0); // p0 at (0,0) — vertical win

        Assert.True(brain.IsGameOver);

        brain.UndoMove();

        Assert.False(brain.IsGameOver);
        Assert.Null(brain.Winner);
        Assert.True(brain.MakeMove(1));
    }
}
