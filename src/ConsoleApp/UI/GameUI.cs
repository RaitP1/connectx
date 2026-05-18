using Domain;

namespace ConsoleApp.UI;

public sealed class GameUI
{
    private readonly TextWriter _writer;

    public GameUI(TextWriter? writer = null)
    {
        _writer = writer ?? Console.Out;
    }

    public void DrawBoard(GameBrain brain, GameConfig config)
    {
        DrawBoardInternal(brain, config, ghostRow: -1, ghostCol: -1, ghostSymbol: "");
    }

    public void AnimateDrop(GameBrain brain, int column)
    {
        var landingRow = FindLandingRow(brain, column);
        if (landingRow < 0) return;

        var symbol = brain.CurrentPlayer == 0
            ? brain.Config.Player1Symbol
            : brain.Config.Player2Symbol;

        for (var row = 0; row <= landingRow; row++)
        {
            Console.Clear();
            ShowColumnSelector(column, brain.Columns);
            DrawBoardInternal(brain, brain.Config, row, column, symbol);
            ShowTurnIndicator(brain.Config, brain.CurrentPlayer);
            Thread.Sleep(50);
        }
    }

    private static int FindLandingRow(GameBrain brain, int column)
    {
        for (var row = brain.Rows - 1; row >= 0; row--)
        {
            if (brain.GetCell(row, column) is null)
                return row;
        }

        return -1;
    }

    private void DrawBoardInternal(GameBrain brain, GameConfig config,
        int ghostRow, int ghostCol, string ghostSymbol)
    {
        _writer.Write("  ");
        for (var c = 0; c < brain.Columns; c++)
            _writer.Write($" {c + 1}  ");
        _writer.WriteLine();

        _writer.Write("  ");
        _writer.WriteLine(new string('-', brain.Columns * 4 + 1));

        for (var r = 0; r < brain.Rows; r++)
        {
            _writer.Write("  ");
            for (var c = 0; c < brain.Columns; c++)
            {
                string symbol;
                if (r == ghostRow && c == ghostCol && brain.GetCell(r, c) is null)
                {
                    symbol = ghostSymbol;
                }
                else
                {
                    var cell = brain.GetCell(r, c);
                    symbol = cell switch
                    {
                        0 => config.Player1Symbol,
                        1 => config.Player2Symbol,
                        _ => " "
                    };
                }

                _writer.Write($"| {symbol} ");
            }
            _writer.WriteLine("|");

            _writer.Write("  ");
            _writer.WriteLine(new string('-', brain.Columns * 4 + 1));
        }
    }

    public void ShowTurnIndicator(GameConfig config, int currentPlayer)
    {
        var name = currentPlayer == 0 ? config.Player1Name : config.Player2Name;
        var symbol = currentPlayer == 0 ? config.Player1Symbol : config.Player2Symbol;
        _writer.WriteLine($"{name}'s turn ({symbol})");
    }

    public void ShowGameOver(int? winner, bool isDraw, GameConfig config)
    {
        if (isDraw)
        {
            _writer.WriteLine("Game over — it's a draw!");
            return;
        }

        if (winner is not null)
        {
            var name = winner == 0 ? config.Player1Name : config.Player2Name;
            _writer.WriteLine($"{name} wins!");
        }
    }

    public void ShowColumnSelector(int selectedColumn, int totalColumns)
    {
        _writer.Write("  ");
        for (var c = 0; c < totalColumns; c++)
        {
            if (c == selectedColumn)
                _writer.Write(" v  ");
            else
                _writer.Write("    ");
        }
        _writer.WriteLine();
    }

    public PlayerInput GetPlayerMove(GameBrain brain)
    {
        var selectedColumn = 0;
        var available = brain.GetAvailableColumns();
        if (available.Count > 0)
            selectedColumn = available[0];

        while (true)
        {
            Console.Clear();
            ShowColumnSelector(selectedColumn, brain.Columns);
            DrawBoard(brain, brain.Config);
            ShowTurnIndicator(brain.Config, brain.CurrentPlayer);
            _writer.WriteLine("[←/→] Move  [Enter] Drop  [S] Save  [Q] Quit");

            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    selectedColumn = ((selectedColumn - 1) % brain.Columns + brain.Columns) % brain.Columns;
                    break;
                case ConsoleKey.RightArrow:
                    selectedColumn = (selectedColumn + 1) % brain.Columns;
                    break;
                case ConsoleKey.Enter:
                    if (brain.IsColumnAvailable(selectedColumn))
                        return new PlayerInput(EPlayerAction.Move, selectedColumn);
                    break;
                case ConsoleKey.S:
                    return new PlayerInput(EPlayerAction.Save);
                case ConsoleKey.Q:
                    return new PlayerInput(EPlayerAction.Quit);
            }
        }
    }
}
