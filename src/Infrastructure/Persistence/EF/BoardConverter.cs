using System.Text.Json;

namespace Infrastructure.Persistence.EF;

internal static class BoardConverter
{
    public static string Serialize(int?[][] board) =>
        JsonSerializer.Serialize(board);

    public static int?[][] Deserialize(string json) =>
        JsonSerializer.Deserialize<int?[][]>(json) ?? [];
}
