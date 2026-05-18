using System.Text.RegularExpressions;

namespace Infrastructure.Persistence.Json;

public sealed partial class FilesystemHelper
{
    private readonly string _baseDir;

    public FilesystemHelper(string? baseDir = null)
    {
        _baseDir = baseDir
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ConnectX");
    }

    public string GetConfigDirectory()
    {
        var path = Path.Combine(_baseDir, "config");
        Directory.CreateDirectory(path);
        return path;
    }

    public string GetSavegamesDirectory()
    {
        var path = Path.Combine(_baseDir, "savegames");
        Directory.CreateDirectory(path);
        return path;
    }

    public static string SanitizeFilename(string name)
    {
        var cleaned = name.Replace("..", "").Replace("/", "").Replace("\\", "");
        cleaned = AllowedCharsRegex().Replace(cleaned, "");

        if (string.IsNullOrEmpty(cleaned))
            throw new ArgumentException("Filename is empty after sanitization.", nameof(name));

        return cleaned;
    }

    public string ResolvePath(string directory, string filename)
    {
        var sanitized = SanitizeFilename(filename);
        var fullPath = Path.GetFullPath(Path.Combine(directory, $"{sanitized}.json"));
        var normalizedDir = Path.GetFullPath(directory) + Path.DirectorySeparatorChar;

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (!fullPath.StartsWith(normalizedDir, comparison))
            throw new InvalidOperationException($"Path '{fullPath}' escapes the allowed directory '{normalizedDir}'.");

        return fullPath;
    }

    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex AllowedCharsRegex();
}
