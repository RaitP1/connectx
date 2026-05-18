using System.ComponentModel.DataAnnotations;
using Domain;

namespace WebApp.Models;

public sealed class ConfigFormModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = "";

    [Range(1, 100)]
    public int Rows { get; init; } = 6;

    [Range(1, 100)]
    public int Columns { get; init; } = 7;

    [Range(1, 100)]
    public int WinCondition { get; init; } = 4;

    [Required]
    [StringLength(50)]
    public string Player1Name { get; init; } = "Player 1";

    [Required]
    [StringLength(2)]
    public string Player1Symbol { get; init; } = "X";

    [Required]
    [StringLength(50)]
    public string Player2Name { get; init; } = "Player 2";

    [Required]
    [StringLength(2)]
    public string Player2Symbol { get; init; } = "O";

    public EBoardTopology Topology { get; init; } = EBoardTopology.Rectangle;

    public string Player1Type { get; init; } = "Human";

    public string Player2Type { get; init; } = "Human";

    public GameConfig ToGameConfig() =>
        new(Name, Rows, Columns, WinCondition,
            Player1Name, Player1Symbol, Player2Name, Player2Symbol,
            Topology, ParsePlayerType(Player1Type), ParsePlayerType(Player2Type));

    public static ConfigFormModel FromGameConfig(GameConfig config) =>
        new()
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
            Player1Type = FormatPlayerType(config.Player1Type),
            Player2Type = FormatPlayerType(config.Player2Type),
        };

    private static PlayerType ParsePlayerType(string value) =>
        value switch
        {
            "Easy AI" => new PlayerType(true, EAIDifficulty.Easy),
            "Medium AI" => new PlayerType(true, EAIDifficulty.Medium),
            "Hard AI" => new PlayerType(true, EAIDifficulty.Hard),
            _ => new PlayerType(false),
        };

    private static string FormatPlayerType(PlayerType type) =>
        type.IsAI
            ? $"{type.Difficulty} AI"
            : "Human";
}
