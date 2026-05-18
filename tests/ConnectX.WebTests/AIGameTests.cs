using System.Net;
using Application.Config.Interfaces;
using Application.Game.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.WebTests;

public sealed class AIGameTests : IClassFixture<ConnectXWebFactory>
{
    private readonly ConnectXWebFactory _factory;

    public AIGameTests(ConnectXWebFactory factory) => _factory = factory;

    [Fact]
    public async Task HumanVsAI_MoveTriggersAIResponse()
    {
        SeedAIConfig("H2AI-Config", aiPlayer2: true);

        var client = CreateClient();
        var getNew = await client.GetAsync("/Games/New");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getNew);

        var createForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["configName"] = "H2AI-Config",
        };
        var createResponse = await client.PostAsync("/Games/New", new FormUrlEncodedContent(createForm));
        var playUrl = createResponse.Headers.Location!.ToString();
        var gameId = WebTestHelpers.ExtractQueryParam(playUrl, "gameId");

        var getPlay = await client.GetAsync(playUrl);
        var moveToken = await WebTestHelpers.ExtractAntiForgeryToken(getPlay);
        var moveForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = moveToken,
            ["column"] = "3",
            ["gameId"] = gameId,
        };
        var moveResponse = await client.PostAsync("/Games/Play", new FormUrlEncodedContent(moveForm));

        if (moveResponse.StatusCode == HttpStatusCode.Redirect)
        {
            var redirectUrl = moveResponse.Headers.Location!.ToString();
            var afterMove = await client.GetAsync(redirectUrl);
            var html = await afterMove.Content.ReadAsStringAsync();
            Assert.Contains("board-table", html);
        }
        else
        {
            var html = await moveResponse.Content.ReadAsStringAsync();
            Assert.Contains("board-table", html);
        }

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        var game = repo.Load(gameId);
        Assert.NotNull(game);
        var totalPieces = game.Board.SelectMany(r => r).Count(c => c.HasValue);
        Assert.True(totalPieces >= 2, "AI should have responded with a move");
    }

    [Fact]
    public async Task AIvsAI_RunsToCompletion()
    {
        SeedAIConfig("AI2AI-Config", aiPlayer1: true, aiPlayer2: true);

        var client = CreateClient();
        var getNew = await client.GetAsync("/Games/New");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getNew);

        var createForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["configName"] = "AI2AI-Config",
        };
        var createResponse = await client.PostAsync("/Games/New", new FormUrlEncodedContent(createForm));
        var playUrl = createResponse.Headers.Location!.ToString();
        var gameId = WebTestHelpers.ExtractQueryParam(playUrl, "gameId");

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();
        var game = repo.Load(gameId);
        Assert.NotNull(game);

        var brain = Application.Game.Mapping.GameStateMapper.ToDomain(game);
        Assert.True(brain.IsGameOver, "AI vs AI game should run to completion on creation");
    }

    private void SeedAIConfig(string name, bool aiPlayer1 = false, bool aiPlayer2 = false)
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        if (repo.Load(name) is not null) return;

        var config = new GameConfig(name, 6, 7, 4, "Player 1", "X", "Player 2", "O",
            EBoardTopology.Rectangle,
            new PlayerType(aiPlayer1, aiPlayer1 ? EAIDifficulty.Easy : null),
            new PlayerType(aiPlayer2, aiPlayer2 ? EAIDifficulty.Easy : null));
        repo.Save(config);
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
