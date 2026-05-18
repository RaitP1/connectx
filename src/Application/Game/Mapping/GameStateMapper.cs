using Application.Game.Dto;
using Domain;

namespace Application.Game.Mapping;

public static class GameStateMapper
{
    public static GameStateDto ToDto(GameBrain brain, string name)
    {
        var board = new int?[brain.Rows][];
        for (var r = 0; r < brain.Rows; r++)
        {
            board[r] = new int?[brain.Columns];
            for (var c = 0; c < brain.Columns; c++)
                board[r][c] = brain.GetCell(r, c);
        }

        return new GameStateDto(
            Name: name,
            Config: brain.Config,
            Board: board,
            CurrentPlayer: brain.CurrentPlayer,
            SavedAt: DateTime.UtcNow);
    }

    public static GameBrain ToDomain(GameStateDto dto)
    {
        var rows = dto.Config.Rows;
        var cols = dto.Config.Columns;

        if (dto.Board.Length != rows || dto.Board.Any(row => row.Length != cols))
            throw new ArgumentException(
                $"Board dimensions do not match config {rows}x{cols}.", nameof(dto));

        var board = new int?[rows, cols];

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < cols; c++)
            board[r, c] = dto.Board[r][c];

        return new GameBrain(
            dto.Config,
            board,
            dto.CurrentPlayer,
            new Stack<(int, int)>());
    }
}
