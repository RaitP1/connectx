namespace Domain;

public sealed class GameBrain
{
    private readonly int?[,] _board;
    private readonly GameConfig _config;
    private readonly Stack<(int Row, int Col)> _moveHistory = new();
    private int _currentPlayer;
    private int? _winner;
    private bool _isDraw;

    public GameConfig Config => _config;
    public int CurrentPlayer => _currentPlayer;
    public int Rows => _config.Rows;
    public int Columns => _config.Columns;
    public bool IsGameOver => _winner is not null || _isDraw;
    public int? Winner => _winner;
    public bool IsDraw => _isDraw;

    public GameBrain(GameConfig config)
    {
        _config = config;
        _board = new int?[config.Rows, config.Columns];
        _currentPlayer = 0;
    }

    private GameBrain(GameConfig config, int?[,] board, int currentPlayer, Stack<(int, int)> history)
    {
        _config = config;
        _board = board;
        _currentPlayer = currentPlayer;
        _moveHistory = history;
    }

    public int? GetCell(int row, int col)
    {
        col = WrapColumn(col);
        return _board[row, col];
    }

    public int WrapColumn(int col)
    {
        if (_config.Topology == EBoardTopology.Cylinder)
            return ((col % Columns) + Columns) % Columns;

        return col;
    }

    public bool MakeMove(int column)
    {
        if (IsGameOver)
            return false;

        column = WrapColumn(column);

        if (column < 0 || column >= Columns)
            return false;

        for (var row = Rows - 1; row >= 0; row--)
        {
            if (_board[row, column] is null)
            {
                _board[row, column] = _currentPlayer;
                _moveHistory.Push((row, column));

                if (CheckWin(row, column))
                    _winner = _currentPlayer;
                else if (IsBoardFull())
                    _isDraw = true;

                _currentPlayer = 1 - _currentPlayer;
                return true;
            }
        }

        return false;
    }

    public bool IsColumnAvailable(int column)
    {
        column = WrapColumn(column);
        return _board[0, column] is null;
    }

    public IReadOnlyList<int> GetAvailableColumns()
    {
        var available = new List<int>();
        for (var col = 0; col < Columns; col++)
        {
            if (IsColumnAvailable(col))
                available.Add(col);
        }
        return available;
    }

    public bool UndoMove()
    {
        if (_moveHistory.Count == 0)
            return false;

        var (row, col) = _moveHistory.Pop();
        _board[row, col] = null;
        _currentPlayer = 1 - _currentPlayer;
        _winner = null;
        _isDraw = false;
        return true;
    }

    public GameBrain Clone()
    {
        var newBoard = new int?[Rows, Columns];
        Array.Copy(_board, newBoard, _board.Length);
        // Double-reverse: Stack enumerates LIFO, so wrapping in two constructors preserves order
        var newHistory = new Stack<(int, int)>(new Stack<(int, int)>(_moveHistory));
        return new GameBrain(_config, newBoard, _currentPlayer, newHistory);
    }

    private bool CheckWin(int row, int col)
    {
        var player = _board[row, col];

        int[][] directions =
        [
            [0, 1],   // horizontal
            [1, 0],   // vertical
            [1, 1],   // diagonal down-right
            [1, -1]   // diagonal up-right
        ];

        foreach (var dir in directions)
        {
            var count = 1;
            count += CountDirection(row, col, dir[0], dir[1], player);
            count += CountDirection(row, col, -dir[0], -dir[1], player);

            // On cylinder, cap to the number of distinct cells per row
            if (_config.Topology == EBoardTopology.Cylinder && dir[1] != 0)
                count = Math.Min(count, Columns);

            if (count >= _config.WinCondition)
                return true;
        }

        return false;
    }

    private int CountDirection(int row, int col, int dRow, int dCol, int? player)
    {
        var count = 0;
        var r = row + dRow;
        var c = col + dCol;

        while (r >= 0 && r < Rows && count < _config.WinCondition - 1)
        {
            var wrappedCol = WrapColumn(c);

            if (_config.Topology == EBoardTopology.Rectangle && (c < 0 || c >= Columns))
                break;

            if (_board[r, wrappedCol] != player)
                break;

            count++;
            r += dRow;
            c += dCol;
        }

        return count;
    }

    private bool IsBoardFull()
    {
        for (var col = 0; col < Columns; col++)
        {
            if (_board[0, col] is null)
                return false;
        }
        return true;
    }
}
