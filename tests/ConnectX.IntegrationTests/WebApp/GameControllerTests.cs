using System.Net;
using System.Text.RegularExpressions;
using Application.Game.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.IntegrationTests.WebApp;

public sealed partial class GameControllerTests : IClassFixture<ConnectXWebFactory>, IDisposable
{
    private readonly ConnectXWebFactory _factory;
    private readonly HttpClient _client;

    public GameControllerTests(ConnectXWebFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task New_ReturnsConfigList()
    {
        var response = await _client.GetAsync("/Game/New");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Classical", html);
    }

    [Fact]
    public async Task Start_RedirectsToPlay()
    {
        var token = await GetAntiForgeryToken("/Game/New");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["configName"] = "Classical",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Game/Start", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Game/Play", response.Headers.Location?.ToString() ?? "");
    }

    [Fact]
    public async Task Move_PlacesPieceAndRedirects()
    {
        await StartGame("Classical");

        var token = await GetAntiForgeryToken("/Game/Play");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["column"] = "3",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Game/Move", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task Save_PersistsGameState()
    {
        await StartGame("Classical");

        var token = await GetAntiForgeryToken("/Game/Save");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["saveName"] = "TestSave",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Game/Save", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        var saved = repo.Load("TestSave");
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task Load_ListsSavedGames()
    {
        await StartGame("Classical");
        await SaveGame("LoadTest");

        var response = await _client.GetAsync("/Game/Load");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("LoadTest", html);
    }

    [Fact]
    public async Task Resume_RestoresGameAndRedirects()
    {
        await StartGame("Classical");
        await SaveGame("ResumeTest");

        var token = await GetAntiForgeryToken("/Game/Load");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["saveName"] = "ResumeTest",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Game/Resume", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Game/Play", response.Headers.Location?.ToString() ?? "");
    }

    [Fact]
    public async Task DeleteSave_RemovesSavedGame()
    {
        await StartGame("Classical");
        await SaveGame("DeleteTest");

        var token = await GetAntiForgeryToken("/Game/Load");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["saveName"] = "DeleteTest",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Game/DeleteSave", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        Assert.Null(repo.Load("DeleteTest"));
    }

    private async Task StartGame(string configName)
    {
        var token = await GetAntiForgeryToken("/Game/New");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["configName"] = configName,
            ["__RequestVerificationToken"] = token,
        });
        await _client.PostAsync("/Game/Start", content);
    }

    private async Task SaveGame(string saveName)
    {
        var token = await GetAntiForgeryToken("/Game/Save");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["saveName"] = saveName,
            ["__RequestVerificationToken"] = token,
        });
        await _client.PostAsync("/Game/Save", content);
    }

    private async Task<string> GetAntiForgeryToken(string url)
    {
        var response = await _client.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();
        var match = AntiForgeryRegex().Match(html);
        return match.Success ? match.Groups[1].Value : "";
    }

    [GeneratedRegex("""__RequestVerificationToken[^>]*value="([^"]+)""")]
    private static partial Regex AntiForgeryRegex();
}
