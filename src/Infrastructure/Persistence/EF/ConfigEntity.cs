using Domain;

namespace Infrastructure.Persistence.EF;

public sealed class ConfigEntity
{
    public string Name { get; set; } = "";
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int WinCondition { get; set; }
    public string Player1Name { get; set; } = "";
    public string Player1Symbol { get; set; } = "";
    public string Player2Name { get; set; } = "";
    public string Player2Symbol { get; set; } = "";
    public EBoardTopology Topology { get; set; }
    public bool Player1TypeIsAI { get; set; }
    public EAIDifficulty? Player1TypeDifficulty { get; set; }
    public bool Player2TypeIsAI { get; set; }
    public EAIDifficulty? Player2TypeDifficulty { get; set; }

    public GameConfig ToDomain() => new(
        Name: Name,
        Rows: Rows,
        Columns: Columns,
        WinCondition: WinCondition,
        Player1Name: Player1Name,
        Player1Symbol: Player1Symbol,
        Player2Name: Player2Name,
        Player2Symbol: Player2Symbol,
        Topology: Topology,
        Player1Type: new PlayerType(Player1TypeIsAI, Player1TypeDifficulty),
        Player2Type: new PlayerType(Player2TypeIsAI, Player2TypeDifficulty));

    public static ConfigEntity FromDomain(GameConfig config) => new()
    {
        Name = config.Name,
        Rows = config.Rows,
        Columns = config.Columns,
        WinCondition = config.WinCondition,
        Player1Name = config.Player1Name,
        Player1Symbol = config.Player1Symbol,
        Player2Name = config.Player2Name,
        Player2Symbol = config.Player2Symbol,
        Topology = config.Topology,
        Player1TypeIsAI = config.Player1Type.IsAI,
        Player1TypeDifficulty = config.Player1Type.Difficulty,
        Player2TypeIsAI = config.Player2Type.IsAI,
        Player2TypeDifficulty = config.Player2Type.Difficulty
    };
}
