using Domain;

namespace ConnectX.UnitTests.Domain;

public class GameConfigTests
{
    private static GameConfig CreateValid() => new(
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
    public void IsValid_ReturnsTrueForStandardConfig()
    {
        var config = CreateValid();

        Assert.True(config.IsValid());
    }

    [Theory]
    [InlineData(0, 7)]
    [InlineData(6, 0)]
    [InlineData(-1, 7)]
    [InlineData(6, -1)]
    public void IsValid_ReturnsFalseForInvalidDimensions(int rows, int columns)
    {
        var config = CreateValid() with { Rows = rows, Columns = columns };

        Assert.False(config.IsValid());
    }

    [Fact]
    public void IsValid_ReturnsFalseWhenWinConditionExceedsBothDimensions()
    {
        var config = CreateValid() with { Rows = 3, Columns = 3, WinCondition = 5 };

        Assert.False(config.IsValid());
    }

    [Fact]
    public void IsValid_ReturnsTrueWhenWinConditionFitsAtLeastOneDimension()
    {
        var config = CreateValid() with { Rows = 3, Columns = 7, WinCondition = 5 };

        Assert.True(config.IsValid());
    }

    [Theory]
    [InlineData("", "O")]
    [InlineData("X", "")]
    [InlineData("  ", "O")]
    public void IsValid_ReturnsFalseForEmptyOrWhitespaceSymbols(string sym1, string sym2)
    {
        var config = CreateValid() with { Player1Symbol = sym1, Player2Symbol = sym2 };

        Assert.False(config.IsValid());
    }

    [Fact]
    public void IsValid_ReturnsFalseForDuplicateSymbols()
    {
        var config = CreateValid() with { Player1Symbol = "X", Player2Symbol = "X" };

        Assert.False(config.IsValid());
    }

    [Theory]
    [InlineData("", "Player 2")]
    [InlineData("Player 1", "")]
    [InlineData("  ", "Player 2")]
    [InlineData("Player 1", "  ")]
    public void IsValid_ReturnsFalseForEmptyOrWhitespaceNames(string name1, string name2)
    {
        var config = CreateValid() with { Player1Name = name1, Player2Name = name2 };

        Assert.False(config.IsValid());
    }
}
