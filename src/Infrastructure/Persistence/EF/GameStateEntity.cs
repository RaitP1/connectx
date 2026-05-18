using Application.Game.Dto;
using Domain;

namespace Infrastructure.Persistence.EF;

public sealed class GameStateEntity
{
    public string Name { get; set; } = "";
    public string BoardJson { get; set; } = "[]";
    public int CurrentPlayer { get; set; }
    public DateTime SavedAt { get; set; }

    public string ConfigName { get; set; } = "";
    public int ConfigRows { get; set; }
    public int ConfigColumns { get; set; }
    public int ConfigWinCondition { get; set; }
    public string ConfigPlayer1Name { get; set; } = "";
    public string ConfigPlayer1Symbol { get; set; } = "";
    public string ConfigPlayer2Name { get; set; } = "";
    public string ConfigPlayer2Symbol { get; set; } = "";
    public EBoardTopology ConfigTopology { get; set; }
    public bool ConfigPlayer1TypeIsAI { get; set; }
    public EAIDifficulty? ConfigPlayer1TypeDifficulty { get; set; }
    public bool ConfigPlayer2TypeIsAI { get; set; }
    public EAIDifficulty? ConfigPlayer2TypeDifficulty { get; set; }

    public GameStateDto ToDomain() => new(
        Name: Name,
        Config: new GameConfig(
            Name: ConfigName,
            Rows: ConfigRows,
            Columns: ConfigColumns,
            WinCondition: ConfigWinCondition,
            Player1Name: ConfigPlayer1Name,
            Player1Symbol: ConfigPlayer1Symbol,
            Player2Name: ConfigPlayer2Name,
            Player2Symbol: ConfigPlayer2Symbol,
            Topology: ConfigTopology,
            Player1Type: new PlayerType(ConfigPlayer1TypeIsAI, ConfigPlayer1TypeDifficulty),
            Player2Type: new PlayerType(ConfigPlayer2TypeIsAI, ConfigPlayer2TypeDifficulty)),
        Board: BoardConverter.Deserialize(BoardJson),
        CurrentPlayer: CurrentPlayer,
        SavedAt: SavedAt);

    public static GameStateEntity FromDomain(GameStateDto state) => new()
    {
        Name = state.Name,
        BoardJson = BoardConverter.Serialize(state.Board),
        CurrentPlayer = state.CurrentPlayer,
        SavedAt = state.SavedAt,
        ConfigName = state.Config.Name,
        ConfigRows = state.Config.Rows,
        ConfigColumns = state.Config.Columns,
        ConfigWinCondition = state.Config.WinCondition,
        ConfigPlayer1Name = state.Config.Player1Name,
        ConfigPlayer1Symbol = state.Config.Player1Symbol,
        ConfigPlayer2Name = state.Config.Player2Name,
        ConfigPlayer2Symbol = state.Config.Player2Symbol,
        ConfigTopology = state.Config.Topology,
        ConfigPlayer1TypeIsAI = state.Config.Player1Type.IsAI,
        ConfigPlayer1TypeDifficulty = state.Config.Player1Type.Difficulty,
        ConfigPlayer2TypeIsAI = state.Config.Player2Type.IsAI,
        ConfigPlayer2TypeDifficulty = state.Config.Player2Type.Difficulty
    };
}
