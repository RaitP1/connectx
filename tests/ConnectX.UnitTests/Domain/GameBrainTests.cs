using Domain;

namespace ConnectX.UnitTests.Domain;

public class GameBrainTests
{
    private static GameConfig StandardConfig(EBoardTopology topology = EBoardTopology.Rectangle) => new(
        Rows: 6,
        Columns: 7,
        WinCondition: 4,
        Player1Name: "Player 1",
        Player1Symbol: "X",
        Player2Name: "Player 2",
        Player2Symbol: "O",
        Topology: topology,
        Player1Type: new PlayerType(false),
        Player2Type: new PlayerType(false));

    [Fact]
    public void Constructor_InitializesEmptyBoardWithPlayer0()
    {
        var brain = new GameBrain(StandardConfig());

        Assert.Equal(0, brain.CurrentPlayer);
        for (var r = 0; r < brain.Rows; r++)
        for (var c = 0; c < brain.Columns; c++)
            Assert.Null(brain.GetCell(r, c));
    }

    [Fact]
    public void MakeMove_DropsToBottomRowInEmptyColumn()
    {
        var brain = new GameBrain(StandardConfig());

        brain.MakeMove(3);

        Assert.Equal(0, brain.GetCell(5, 3));
        for (var r = 0; r < 5; r++)
            Assert.Null(brain.GetCell(r, 3));
    }

    [Fact]
    public void MakeMove_StacksPiecesUpward()
    {
        var brain = new GameBrain(StandardConfig());

        brain.MakeMove(3);
        brain.MakeMove(3);
        brain.MakeMove(3);

        Assert.Equal(0, brain.GetCell(5, 3));
        Assert.Equal(1, brain.GetCell(4, 3));
        Assert.Equal(0, brain.GetCell(3, 3));
    }

    [Fact]
    public void MakeMove_SwitchesPlayer()
    {
        var brain = new GameBrain(StandardConfig());

        Assert.Equal(0, brain.CurrentPlayer);
        brain.MakeMove(0);
        Assert.Equal(1, brain.CurrentPlayer);
        brain.MakeMove(1);
        Assert.Equal(0, brain.CurrentPlayer);
    }

    [Fact]
    public void MakeMove_ReturnsFalseForFullColumn()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 2, WinCondition = 2 };
        var brain = new GameBrain(config);

        brain.MakeMove(0);
        brain.MakeMove(0);

        Assert.False(brain.MakeMove(0));
    }

    [Fact]
    public void MakeMove_DoesNotSwitchPlayerOnRejectedMove()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 2, WinCondition = 2 };
        var brain = new GameBrain(config);

        brain.MakeMove(0);
        brain.MakeMove(0);

        var playerBefore = brain.CurrentPlayer;
        brain.MakeMove(0);
        Assert.Equal(playerBefore, brain.CurrentPlayer);
    }

    [Fact]
    public void IsColumnAvailable_ReturnsTrueForEmptyColumn()
    {
        var brain = new GameBrain(StandardConfig());

        Assert.True(brain.IsColumnAvailable(3));
    }

    [Fact]
    public void IsColumnAvailable_ReturnsFalseForFullColumn()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 2, WinCondition = 2 };
        var brain = new GameBrain(config);

        brain.MakeMove(0);
        brain.MakeMove(0);

        Assert.False(brain.IsColumnAvailable(0));
    }

    [Fact]
    public void GetAvailableColumns_ReturnsCorrectColumns()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 3, WinCondition = 2 };
        var brain = new GameBrain(config);

        brain.MakeMove(1);
        brain.MakeMove(1);

        var available = brain.GetAvailableColumns();
        Assert.Equal([0, 2], available);
    }

    [Fact]
    public void WrapColumn_CylinderWrapsPositive()
    {
        var brain = new GameBrain(StandardConfig(EBoardTopology.Cylinder));

        Assert.Equal(2, brain.WrapColumn(9));
    }

    [Fact]
    public void WrapColumn_CylinderWrapsNegative()
    {
        var brain = new GameBrain(StandardConfig(EBoardTopology.Cylinder));

        Assert.Equal(6, brain.WrapColumn(-1));
    }

    [Fact]
    public void WrapColumn_RectangleReturnsIdentity()
    {
        var brain = new GameBrain(StandardConfig(EBoardTopology.Rectangle));

        Assert.Equal(3, brain.WrapColumn(3));
    }

    [Fact]
    public void GetCell_CylinderWrapsColumnAccess()
    {
        var brain = new GameBrain(StandardConfig(EBoardTopology.Cylinder));

        brain.MakeMove(0);

        Assert.Equal(0, brain.GetCell(5, 7));
        Assert.Equal(0, brain.GetCell(5, -7));
    }
}
