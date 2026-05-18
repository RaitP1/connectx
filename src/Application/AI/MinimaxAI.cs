using Domain;

namespace Application.AI;

public sealed class MinimaxAI : IAIPlayer
{
    private readonly int _maxDepth;

    public MinimaxAI(EAIDifficulty difficulty)
    {
        _maxDepth = difficulty switch
        {
            EAIDifficulty.Easy => 3,
            EAIDifficulty.Medium => 5,
            EAIDifficulty.Hard => 7,
            _ => 5
        };
    }

    public int GetMove(GameBrain brain, int player)
    {
        var clone = brain.Clone();
        var available = clone.GetAvailableColumns();

        if (available.Count == 1)
            return available[0];

        var bestCol = available[0];
        var bestScore = int.MinValue;

        foreach (var col in available)
        {
            clone.MakeMove(col);
            var score = Minimax(clone, _maxDepth - 1, int.MinValue, int.MaxValue, false, player);
            clone.UndoMove();

            if (score > bestScore)
            {
                bestScore = score;
                bestCol = col;
            }
        }

        return bestCol;
    }

    private static int Minimax(GameBrain brain, int depth, int alpha, int beta, bool isMaximizing, int player)
    {
        if (depth == 0 || brain.IsGameOver)
            return BoardEvaluator.EvaluatePosition(brain, player);

        var available = brain.GetAvailableColumns();

        if (isMaximizing)
        {
            var maxEval = int.MinValue;
            foreach (var col in available)
            {
                brain.MakeMove(col);
                var eval = Minimax(brain, depth - 1, alpha, beta, false, player);
                brain.UndoMove();

                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            var minEval = int.MaxValue;
            foreach (var col in available)
            {
                brain.MakeMove(col);
                var eval = Minimax(brain, depth - 1, alpha, beta, true, player);
                brain.UndoMove();

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }
}
