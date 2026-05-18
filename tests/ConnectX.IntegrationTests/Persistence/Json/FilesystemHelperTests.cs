using Infrastructure.Persistence.Json;

namespace ConnectX.IntegrationTests.Persistence.Json;

public sealed class FilesystemHelperTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), $"connectx-test-{Guid.NewGuid()}");

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public void GetConfigDirectory_CreatesDirectoryIfNotExists()
    {
        var helper = new FilesystemHelper(_tempDir);

        var path = helper.GetConfigDirectory();

        Assert.True(Directory.Exists(path));
        Assert.Equal(Path.Combine(_tempDir, "config"), path);
    }

    [Fact]
    public void GetSavegamesDirectory_CreatesDirectoryIfNotExists()
    {
        var helper = new FilesystemHelper(_tempDir);

        var path = helper.GetSavegamesDirectory();

        Assert.True(Directory.Exists(path));
        Assert.Equal(Path.Combine(_tempDir, "savegames"), path);
    }

    [Fact]
    public void GetConfigDirectory_ReturnsExistingDirectoryWithoutError()
    {
        var helper = new FilesystemHelper(_tempDir);
        helper.GetConfigDirectory();

        var path = helper.GetConfigDirectory();

        Assert.True(Directory.Exists(path));
    }

    [Theory]
    [InlineData("valid-name", "valid-name")]
    [InlineData("test_123", "test_123")]
    [InlineData("ABC", "ABC")]
    public void SanitizeFilename_PreservesValidCharacters(string input, string expected)
    {
        Assert.Equal(expected, FilesystemHelper.SanitizeFilename(input));
    }

    [Theory]
    [InlineData("hello world", "helloworld")]
    [InlineData("test@#$%", "test")]
    [InlineData("my.file.name", "myfilename")]
    public void SanitizeFilename_StripsInvalidCharacters(string input, string expected)
    {
        Assert.Equal(expected, FilesystemHelper.SanitizeFilename(input));
    }

    [Theory]
    [InlineData("../../../etc/passwd")]
    [InlineData("..\\windows\\system32")]
    [InlineData("test/../secret")]
    public void SanitizeFilename_RemovesPathTraversalSequences(string input)
    {
        var sanitized = FilesystemHelper.SanitizeFilename(input);

        Assert.DoesNotContain("..", sanitized);
        Assert.DoesNotContain("/", sanitized);
        Assert.DoesNotContain("\\", sanitized);
    }

    [Theory]
    [InlineData("")]
    [InlineData("@#$%^")]
    [InlineData("../../..")]
    public void SanitizeFilename_ThrowsForEmptyResultAfterSanitization(string input)
    {
        Assert.Throws<ArgumentException>(() => FilesystemHelper.SanitizeFilename(input));
    }

    [Fact]
    public void ResolvePath_StaysWithinAllowedDirectory()
    {
        var helper = new FilesystemHelper(_tempDir);
        var configDir = helper.GetConfigDirectory();

        var path = helper.ResolvePath(configDir, "test-file");

        Assert.StartsWith(configDir, path);
    }

    [Fact]
    public void ResolvePath_SanitizationPreventsDirectoryEscape()
    {
        var helper = new FilesystemHelper(_tempDir);
        var configDir = helper.GetConfigDirectory();

        var path = helper.ResolvePath(configDir, "../escape");

        Assert.StartsWith(configDir, path);
        Assert.EndsWith("escape.json", path);
    }
}
