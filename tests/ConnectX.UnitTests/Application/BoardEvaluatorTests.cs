using Application.AI;
using Domain;

namespace ConnectX.UnitTests.Application;

public class BoardEvaluatorTests
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
    public void EvaluatePosition_MaxForWinningPlayer()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 4, WinCondition = 2 };
        var brain = new GameBrain(config);
        brain.MakeMove(0); // p0
        brain.MakeMove(2); // p1
        brain.MakeMove(0); // p0 wins vertically

        var score = BoardEvaluator.EvaluatePosition(brain, 0);
        Assert.True(score > 0);
    }

    [Fact]
    public void EvaluatePosition_MinForLosingPlayer()
    {
        var config = StandardConfig() with { Rows = 2, Columns = 4, WinCondition = 2 };
        var brain = new GameBrain(config);
        brain.MakeMove(0); // p0
        brain.MakeMove(2); // p1
        brain.MakeMove(0); // p0 wins vertically

        var score = BoardEvaluator.EvaluatePosition(brain, 1);
        Assert.True(score < 0);
    }

    [Fact]
    public void EvaluatePosition_NearZeroForEmptyBoard()
    {
        var brain = new GameBrain(StandardConfig());

        var score = BoardEvaluator.EvaluatePosition(brain, 0);
        Assert.True(Math.Abs(score) <= 10);
    }
}
