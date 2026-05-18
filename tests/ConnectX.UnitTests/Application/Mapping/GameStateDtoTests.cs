using Application.Game.Dto;
using Domain;

namespace ConnectX.UnitTests.Application.Mapping;

public sealed class GameStateDtoTests
{
    [Fact]
    public void GameStateDto_IsRecord()
    {
        var type = typeof(GameStateDto);
        var cloneMethod = type.GetMethod("<Clone>$");
        Assert.NotNull(cloneMethod);
    }

    [Fact]
    public void GameStateDto_HasExpectedProperties()
    {
        var type = typeof(GameStateDto);

        Assert.NotNull(type.GetProperty("Name"));
        Assert.NotNull(type.GetProperty("Config"));
        Assert.NotNull(type.GetProperty("Board"));
        Assert.NotNull(type.GetProperty("CurrentPlayer"));
        Assert.NotNull(type.GetProperty("SavedAt"));
    }

    [Fact]
    public void GameStateDto_PropertyTypes_AreCorrect()
    {
        Assert.Equal(typeof(string), typeof(GameStateDto).GetProperty("Name")!.PropertyType);
        Assert.Equal(typeof(GameConfig), typeof(GameStateDto).GetProperty("Config")!.PropertyType);
        Assert.Equal(typeof(int?[][]), typeof(GameStateDto).GetProperty("Board")!.PropertyType);
        Assert.Equal(typeof(int), typeof(GameStateDto).GetProperty("CurrentPlayer")!.PropertyType);
        Assert.Equal(typeof(DateTime), typeof(GameStateDto).GetProperty("SavedAt")!.PropertyType);
    }
}
