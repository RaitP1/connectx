using Domain;

namespace ConnectX.UnitTests.Domain;

public class GameBrainWinTests
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

    private static void MakeMoves(GameBrain brain, params int[] columns)
    {
        foreach (var col in columns)
            brain.MakeMove(col);
    }

    [Fact]
    public void HorizontalWin_RectangleBoard()
    {
        var brain = new GameBrain(StandardConfig());

        // Player 0: cols 0,1,2,3 — Player 1: cols 0,1,2 (stacking below)
        MakeMoves(brain, 0, 0, 1, 1, 2, 2, 3);

        Assert.True(brain.IsGameOver);
        Assert.Equal(0, brain.Winner);
    }

    [Fact]
    public void VerticalWin()
    {
        var brain = new GameBrain(StandardConfig());

        // Player 0: col 0 x4, Player 1: col 1 x3
        MakeMoves(brain, 0, 1, 0, 1, 0, 1, 0);

        Assert.True(brain.IsGameOver);
        Assert.Equal(0, brain.Winner);
    }

    [Fact]
    public void DiagonalDownRightWin()
    {
        var brain = new GameBrain(StandardConfig());

        // Build diagonal for p0: (5,0)(4,1)(3,2)(2,3)
        // Scatter p1 moves to avoid accidental horizontal wins
        brain.MakeMove(0); // p0 at (5,0)
        brain.MakeMove(1); // p1 at (5,1)
        brain.MakeMove(1); // p0 at (4,1)
        brain.MakeMove(2); // p1 at (5,2)
        brain.MakeMove(2); // p0 at (4,2)
        brain.MakeMove(6); // p1 at (5,6) — scattered
        brain.MakeMove(2); // p0 at (3,2)
        brain.MakeMove(3); // p1 at (5,3)
        brain.MakeMove(3); // p0 at (4,3)
        brain.MakeMove(3); // p1 at (3,3)
        brain.MakeMove(3); // p0 at (2,3) — diagonal (5,0)(4,1)(3,2)(2,3) win

        Assert.True(brain.IsGameOver);
        Assert.Equal(0, brain.Winner);
    }

    [Fact]
    public void DiagonalUpRightWin()
    {
        var brain = new GameBrain(StandardConfig());

        // Build bottom-left to top-right diagonal for player 0
        // Need p0 at (5,3), (4,2), (3,1), (2,0)
        brain.MakeMove(3); // p0 at (5,3)
        brain.MakeMove(2); // p1 at (5,2)
        brain.MakeMove(2); // p0 at (4,2)
        brain.MakeMove(1); // p1 at (5,1)
        brain.MakeMove(1); // p0 at (4,1)
        brain.MakeMove(0); // p1 at (5,0)
        brain.MakeMove(1); // p0 at (3,1)
        brain.MakeMove(0); // p1 at (4,0)
        brain.MakeMove(0); // p0 at (3,0)
        brain.MakeMove(6); // p1 at (5,6)
        brain.MakeMove(0); // p0 at (2,0) — diagonal (5,3)(4,2)(3,1)(2,0) win

        Assert.True(brain.IsGameOver);
        Assert.Equal(0, brain.Winner);
    }

    [Fact]
    public void CylinderHorizontalWrapWin()
    {
        var brain = new GameBrain(StandardConfig(EBoardTopology.Cylinder));

        // Player 0 at cols 5,6,0,1 (wrapping around) — player 1 stacks elsewhere
        brain.MakeMove(5); // p0
        brain.MakeMove(2); // p1
        brain.MakeMove(6); // p0
        brain.MakeMove(2); // p1
        brain.MakeMove(0); // p0
        brain.MakeMove(3); // p1
        brain.MakeMove(1); // p0 — horizontal wrap win

        Assert.True(brain.IsGameOver);
        Assert.Equal(0, brain.Winner);
    }

    [Fact]
    public void CylinderDiagonalWrapWin()
    {
        // 7 cols cylinder, winCondition=4
        // p0 diagonal: (5,5), (4,6), (3,0), (2,1) — wraps columns
        var brain = new GameBrain(StandardConfig(EBoardTopology.Cylinder));

        brain.MakeMove(5); // p0 at (5,5)
        brain.MakeMove(6); // p1 at (5,6)
        brain.MakeMove(6); // p0 at (4,6)
        brain.MakeMove(3); // p1 at (5,3) — scattered
        brain.MakeMove(0); // p0 at (5,0)
        brain.MakeMove(0); // p1 at (4,0)
        brain.MakeMove(0); // p0 at (3,0)
        brain.MakeMove(1); // p1 at (5,1)
        brain.MakeMove(1); // p0 at (4,1)
        brain.MakeMove(1); // p1 at (3,1)
        brain.MakeMove(1); // p0 at (2,1) — diagonal (5,5)(4,6)(3,0)(2,1) win

        Assert.True(brain.IsGameOver);
        Assert.Equal(0, brain.Winner);
    }

    [Fact]
    public void DrawDetection_FullBoardNoWinner()
    {
        // Use a 2x2 board with winCondition=3 (impossible to win)
        var config = StandardConfig() with { Rows = 2, Columns = 2, WinCondition = 3 };
        var brain = new GameBrain(config);

        brain.MakeMove(0); // p0 at (1,0)
        brain.MakeMove(1); // p1 at (1,1)
        brain.MakeMove(0); // p0 at (0,0)
        brain.MakeMove(1); // p1 at (0,1)

        Assert.True(brain.IsGameOver);
        Assert.True(brain.IsDraw);
        Assert.Null(brain.Winner);
    }

    [Fact]
    public void WinOnLastMove_IsWinNotDraw()
    {
        // 2x2 board winCondition=2: player 0 can win vertically in col 0
        var config = StandardConfig() with { Rows = 2, Columns = 2, WinCondition = 2 };
        var brain = new GameBrain(config);

        brain.MakeMove(0); // p0 at (1,0)
        brain.MakeMove(1); // p1 at (1,1)
        brain.MakeMove(0); // p0 at (0,0) — vertical win, also fills toward full board

        Assert.True(brain.IsGameOver);
        Assert.False(brain.IsDraw);
        Assert.Equal(0, brain.Winner);
    }

    [Fact]
    public void NoWinDetectedBeforeWinConditionMet()
    {
        var brain = new GameBrain(StandardConfig());

        brain.MakeMove(0); // p0
        brain.MakeMove(1); // p1
        brain.MakeMove(0); // p0
        brain.MakeMove(1); // p1
        brain.MakeMove(0); // p0 — only 3 vertical in col 0

        Assert.False(brain.IsGameOver);
        Assert.Null(brain.Winner);
    }

    [Fact]
    public void CylinderNarrowBoard_NoFalseWin()
    {
        // 4-column cylinder, winCondition=5, 6 rows
        // All 4 bottom cells filled by p0 — NOT a horizontal win (only 4, need 5)
        var config = StandardConfig(EBoardTopology.Cylinder)
            with { Columns = 4, WinCondition = 5 };
        var brain = new GameBrain(config);

        brain.MakeMove(0); // p0 at (5,0)
        brain.MakeMove(0); // p1 at (4,0)
        brain.MakeMove(1); // p0 at (5,1)
        brain.MakeMove(1); // p1 at (4,1)
        brain.MakeMove(2); // p0 at (5,2)
        brain.MakeMove(2); // p1 at (4,2)
        brain.MakeMove(3); // p0 at (5,3) — 4 in a row horizontally on 4-col cylinder

        Assert.False(brain.IsGameOver);
    }

    [Fact]
    public void MoveRejectedAfterGameOver()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 3, WinCondition = 2 };
        var brain = new GameBrain(config);

        brain.MakeMove(0); // p0 at (1,0)
        brain.MakeMove(2); // p1 at (1,2)
        brain.MakeMove(0); // p0 at (0,0) — vertical win

        Assert.False(brain.MakeMove(1));
    }
}
