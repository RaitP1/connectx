namespace Domain;

public sealed record GameConfig(
    int Rows,
    int Columns,
    int WinCondition,
    string Player1Name,
    string Player1Symbol,
    string Player2Name,
    string Player2Symbol,
    EBoardTopology Topology,
    PlayerType Player1Type,
    PlayerType Player2Type)
{
    public bool IsValid()
    {
        if (Rows <= 0 || Columns <= 0)
            return false;

        if (WinCondition > Rows && WinCondition > Columns)
            return false;

        if (string.IsNullOrWhiteSpace(Player1Name) || string.IsNullOrWhiteSpace(Player2Name))
            return false;

        if (string.IsNullOrWhiteSpace(Player1Symbol) || string.IsNullOrWhiteSpace(Player2Symbol))
            return false;

        if (Player1Symbol == Player2Symbol)
            return false;

        return true;
    }
}
