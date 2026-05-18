using System.Net;
using Application.Game.Interfaces;
using ConnectX.TestHelpers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.WebTests;

public sealed class GameLifecycleTests : IClassFixture<ConnectXWebFactory>
{
    private readonly ConnectXWebFactory _factory;

    public GameLifecycleTests(ConnectXWebFactory factory) => _factory = factory;

    [Fact]
    public async Task NewGame_ListsConfigs()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Games/New");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Classical", html);
    }

    [Fact]
    public async Task CreateGame_RedirectsToPlay()
    {
        var client = CreateClient();

        var getResponse = await client.GetAsync("/Games/New");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getResponse);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["configName"] = "Classical",
        };

        var response = await client.PostAsync("/Games/New", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Games/Play", response.Headers.Location?.ToString() ?? "");
    }

    [Fact]
    public async Task GamesList_ShowsSavedGames()
    {
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        var config = ConfigBuilder.Standard("ListTest");
        repo.Save(GameStateBuilder.FromConfig(config, "visible-game"));

        var response = await client.GetAsync("/Games/List");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("visible-game", html);
    }

    [Fact]
    public async Task DeleteGame_RemovesFromList()
    {
        var client = CreateClient();

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        var config = ConfigBuilder.Standard("DelTest");
        repo.Save(GameStateBuilder.FromConfig(config, "delete-me"));

        var getResponse = await client.GetAsync("/Games/List");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getResponse);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["name"] = "delete-me",
        };

        var response = await client.PostAsync("/Games/List?handler=Delete", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        using var assertScope = _factory.Services.CreateScope();
        var assertRepo = assertScope.ServiceProvider.GetRequiredService<IGameRepository>();
        Assert.Null(assertRepo.Load("delete-me"));
    }

    [Fact]
    public async Task PlayPage_ShowsBoard()
    {
        var client = CreateClient();

        var getNew = await client.GetAsync("/Games/New");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getNew);
        var createForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["configName"] = "Classical",
        };
        var createResponse = await client.PostAsync("/Games/New", new FormUrlEncodedContent(createForm));
        var playUrl = createResponse.Headers.Location!.ToString();

        var playResponse = await client.GetAsync(playUrl);

        playResponse.EnsureSuccessStatusCode();
        var html = await playResponse.Content.ReadAsStringAsync();
        Assert.Contains("board-table", html);
    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
        });
    }
}
