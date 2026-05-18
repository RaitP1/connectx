using Application.AI;
using Domain;

namespace ConnectX.UnitTests.Application;

public class MinimaxAITests
{
    private static GameConfig StandardConfig() => new(
        Name: "Test",
        Rows: 6,
        Columns: 7,
        WinCondition: 4,
        Player1Name: "Player 1",
        Player1Symbol: "X",
        Player2Name: "Player 2",
        Player2Symbol: "O",
        Topology: EBoardTopology.Rectangle,
        Player1Type: new PlayerType(true, EAIDifficulty.Medium),
        Player2Type: new PlayerType(false));

    [Fact]
    public void GetMove_ReturnsValidColumn()
    {
        var brain = new GameBrain(StandardConfig());
        var ai = new MinimaxAI(EAIDifficulty.Medium);

        var col = ai.GetMove(brain, 0);

        Assert.Contains(col, brain.GetAvailableColumns());
    }

    [Fact]
    public void GetMove_BlocksOpponentWin()
    {
        var brain = new GameBrain(StandardConfig());

        // p1 has 3 in a row at (5,0)(5,1)(5,2) — must block at col 3
        brain.MakeMove(6); // p0 at (5,6)
        brain.MakeMove(0); // p1 at (5,0)
        brain.MakeMove(5); // p0 at (5,5)
        brain.MakeMove(1); // p1 at (5,1)
        brain.MakeMove(4); // p0 at (5,4)
        brain.MakeMove(2); // p1 at (5,2)

        // p0's turn — p1 threatens col 3 for horizontal win
        var ai = new MinimaxAI(EAIDifficulty.Medium);
        var col = ai.GetMove(brain, 0);

        Assert.Equal(3, col);
    }

    [Fact]
    public void GetMove_TakesWinningMove()
    {
        var brain = new GameBrain(StandardConfig());

        // p0 has 3 in a row at (5,0)(5,1)(5,2), can win at col 3
        brain.MakeMove(0); // p0 at (5,0)
        brain.MakeMove(6); // p1 at (5,6)
        brain.MakeMove(1); // p0 at (5,1)
        brain.MakeMove(5); // p1 at (5,5)
        brain.MakeMove(2); // p0 at (5,2)
        brain.MakeMove(4); // p1 at (5,4)

        // p0's turn — can win at col 3
        var ai = new MinimaxAI(EAIDifficulty.Medium);
        var col = ai.GetMove(brain, 0);

        Assert.Equal(3, col);
    }

    [Fact]
    public void GetMove_ReturnsSingleAvailableColumn()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 2, WinCondition = 4 };
        var brain = new GameBrain(config);

        brain.MakeMove(0); // p0 at (1,0)
        brain.MakeMove(0); // p1 at (0,0)
        brain.MakeMove(1); // p0 at (1,1)

        // Only col 1 row 0 is available
        var ai = new MinimaxAI(EAIDifficulty.Easy);
        var col = ai.GetMove(brain, 1);

        Assert.Equal(1, col);
    }

    [Fact]
    public void GetMove_DoesNotMutateOriginalState()
    {
        var brain = new GameBrain(StandardConfig());
        brain.MakeMove(3);

        var playerBefore = brain.CurrentPlayer;
        var cellBefore = brain.GetCell(5, 3);

        var ai = new MinimaxAI(EAIDifficulty.Hard);
        ai.GetMove(brain, 1);

        Assert.Equal(playerBefore, brain.CurrentPlayer);
        Assert.Equal(cellBefore, brain.GetCell(5, 3));

        for (var r = 0; r < brain.Rows; r++)
        for (var c = 0; c < brain.Columns; c++)
        {
            if (r == 5 && c == 3) continue;
            Assert.Null(brain.GetCell(r, c));
        }
    }
}
