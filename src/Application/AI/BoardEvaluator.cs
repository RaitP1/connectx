using Domain;

namespace Application.AI;

public static class BoardEvaluator
{
    public static int EvaluatePosition(GameBrain brain, int player)
    {
        var opponent = 1 - player;

        if (brain.Winner == player) return 10000;
        if (brain.Winner == opponent) return -10000;

        var score = 0;

        for (var row = 0; row < brain.Rows; row++)
        {
            for (var col = 0; col < brain.Columns; col++)
            {
                if (brain.GetCell(row, col) == player)
                    score += EvaluateCell(brain, row, col, player);
                else if (brain.GetCell(row, col) == opponent)
                    score -= EvaluateCell(brain, row, col, opponent);
            }
        }

        return score;
    }

    private static int EvaluateCell(GameBrain brain, int row, int col, int player)
    {
        var score = 0;
        int[][] directions = [[0, 1], [1, 0], [1, 1], [1, -1]];

        foreach (var dir in directions)
        {
            var count = 1;
            count += CountDirection(brain, row, col, dir[0], dir[1], player);
            count += CountDirection(brain, row, col, -dir[0], -dir[1], player);

            if (count >= 2)
                score += count * count;
        }

        return score;
    }

    private static int CountDirection(GameBrain brain, int row, int col, int dRow, int dCol, int player)
    {
        var count = 0;
        var r = row + dRow;
        var c = col + dCol;

        while (r >= 0 && r < brain.Rows && count < brain.Config.WinCondition - 1)
        {
            var wrappedCol = brain.WrapColumn(c);

            if (brain.Config.Topology == EBoardTopology.Rectangle && (c < 0 || c >= brain.Columns))
                break;

            if (brain.GetCell(r, wrappedCol) != player)
                break;

            count++;
            r += dRow;
            c += dCol;
        }

        return count;
    }
}
