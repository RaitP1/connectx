using Domain;

namespace WebApp.Models;

public sealed class PlayViewModel
{
    public required int?[][] Board { get; init; }
    public required int Rows { get; init; }
    public required int Columns { get; init; }
    public required int CurrentPlayer { get; init; }
    public required GameConfig Config { get; init; }
    public required bool IsGameOver { get; init; }
    public required int? Winner { get; init; }
    public required bool IsDraw { get; init; }
    public string? Message { get; init; }
}
