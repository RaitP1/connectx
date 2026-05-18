using Application.Game.Dto;
using Domain;

namespace ConnectX.TestHelpers.Builders;

public static class GameStateBuilder
{
    public static GameStateDto FromConfig(GameConfig config, string name = "save-1")
    {
        var board = new int?[config.Rows][];
        for (var i = 0; i < config.Rows; i++)
            board[i] = new int?[config.Columns];

        return new GameStateDto(name, config, board, 0, DateTime.UtcNow);
    }

    public static GameStateDto WithMoves(GameConfig config, string name = "save-1")
    {
        var board = new int?[config.Rows][];
        for (var i = 0; i < config.Rows; i++)
            board[i] = new int?[config.Columns];

        board[config.Rows - 1][0] = 0;
        board[config.Rows - 1][1] = 1;
        board[config.Rows - 2][0] = 0;

        return new GameStateDto(name, config, board, 1, DateTime.UtcNow);
    }
}
