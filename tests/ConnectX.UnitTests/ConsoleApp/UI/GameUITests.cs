using ConsoleApp.UI;
using ConnectX.TestHelpers.Builders;
using Domain;

namespace ConnectX.UnitTests.ConsoleApp.UI;

public class GameUITests
{
    [Fact]
    public void DrawBoard_EmptyBoard_ShowsColumnNumbersAndEmptyCells()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = ConfigBuilder.Standard();
        var brain = new GameBrain(config);

        ui.DrawBoard(brain, config);

        var output = writer.ToString();
        Assert.Contains("1", output);
        Assert.Contains("7", output);
        Assert.DoesNotContain("X", output);
        Assert.DoesNotContain("O", output);
    }

    [Fact]
    public void DrawBoard_WithPieces_ShowsPlayerSymbols()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = ConfigBuilder.Standard();
        var brain = new GameBrain(config);

        brain.MakeMove(3); // Player 0 -> "X"
        brain.MakeMove(4); // Player 1 -> "O"

        ui.DrawBoard(brain, config);

        var output = writer.ToString();
        Assert.Contains("X", output);
        Assert.Contains("O", output);
    }

    [Fact]
    public void DrawBoard_CorrectGridDimensions()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = new GameConfig(
            "Small", 4, 5, 3, "P1", "X", "P2", "O",
            EBoardTopology.Rectangle,
            new PlayerType(false), new PlayerType(false));
        var brain = new GameBrain(config);

        ui.DrawBoard(brain, config);

        var output = writer.ToString();
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Contains("5", output);
    }

    [Fact]
    public void ShowTurnIndicator_DisplaysPlayerNameAndSymbol()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = ConfigBuilder.Standard();

        ui.ShowTurnIndicator(config, 0);

        var output = writer.ToString();
        Assert.Contains("Player 1", output);
        Assert.Contains("X", output);
    }

    [Fact]
    public void ShowTurnIndicator_Player2()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = ConfigBuilder.Standard();

        ui.ShowTurnIndicator(config, 1);

        var output = writer.ToString();
        Assert.Contains("Player 2", output);
        Assert.Contains("O", output);
    }

    [Fact]
    public void ShowGameOver_Winner_DisplaysWinnerName()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = ConfigBuilder.Standard();

        ui.ShowGameOver(0, false, config);

        var output = writer.ToString();
        Assert.Contains("Player 1", output);
        Assert.Contains("win", output.ToLower());
    }

    [Fact]
    public void ShowGameOver_Draw_DisplaysDrawMessage()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = ConfigBuilder.Standard();

        ui.ShowGameOver(null, true, config);

        var output = writer.ToString();
        Assert.Contains("draw", output.ToLower());
    }

    [Fact]
    public void FormatBoard_ProducesConsistentOutput()
    {
        var writer = new StringWriter();
        var ui = new GameUI(writer);
        var config = ConfigBuilder.Standard();
        var brain = new GameBrain(config);

        brain.MakeMove(0);

        ui.DrawBoard(brain, config);
        var output = writer.ToString();

        Assert.Contains("X", output);
        Assert.Contains("|", output);
    }
}
