using Application.Config.Interfaces;
using Domain;

namespace WebApp;

public static class DefaultConfigSeeder
{
    public static void Seed(IConfigRepository configRepository)
    {
        if (configRepository.List().Count > 0)
            return;

        var defaults = new[]
        {
            new GameConfig("Classical", 6, 7, 4, "Player 1", "X", "Player 2", "O",
                EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false)),

            new GameConfig("Connect-3", 4, 5, 3, "Player 1", "X", "Player 2", "O",
                EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false)),

            new GameConfig("Connect-5", 7, 9, 5, "Player 1", "X", "Player 2", "O",
                EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false)),

            new GameConfig("Connect-4 Cylinder", 6, 7, 4, "Player 1", "X", "Player 2", "O",
                EBoardTopology.Cylinder, new PlayerType(false), new PlayerType(false)),
        };

        foreach (var config in defaults)
            configRepository.Save(config);
    }
}
