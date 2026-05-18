using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ConnectX.WebTests;

public sealed class MultiplayerTests : IClassFixture<ConnectXWebFactory>
{
    private readonly ConnectXWebFactory _factory;

    public MultiplayerTests(ConnectXWebFactory factory) => _factory = factory;

    [Fact]
    public async Task JoinGame_AssignsSlot1()
    {
        var player1 = CreateClient();

        var getNew = await player1.GetAsync("/Games/New");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getNew);
        var createForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["configName"] = "Classical",
        };
        var createResponse = await player1.PostAsync("/Games/New", new FormUrlEncodedContent(createForm));
        var playUrl = createResponse.Headers.Location!.ToString();
        var gameId = WebTestHelpers.ExtractQueryParam(playUrl, "gameId");

        var player2 = CreateClient();
        var getList = await player2.GetAsync("/Games/List");
        var joinToken = await WebTestHelpers.ExtractAntiForgeryToken(getList);
        var joinForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = joinToken,
            ["name"] = gameId,
        };
        var joinResponse = await player2.PostAsync("/Games/List?handler=Join", new FormUrlEncodedContent(joinForm));

        Assert.Equal(HttpStatusCode.Redirect, joinResponse.StatusCode);
        Assert.Contains("/Games/Play", joinResponse.Headers.Location?.ToString() ?? "");
    }

    [Fact]
    public async Task PollEndpoint_ReturnsGameState()
    {
        var player1 = CreateClient();

        var getNew = await player1.GetAsync("/Games/New");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getNew);
        var createForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["configName"] = "Classical",
        };
        var createResponse = await player1.PostAsync("/Games/New", new FormUrlEncodedContent(createForm));
        var playUrl = createResponse.Headers.Location!.ToString();
        var gameId = WebTestHelpers.ExtractQueryParam(playUrl, "gameId");

        var pollResponse = await player1.GetAsync($"/Games/Play?handler=Poll&gameId={gameId}");

        pollResponse.EnsureSuccessStatusCode();
        var json = await pollResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("currentPlayer", out _));
        Assert.True(doc.RootElement.TryGetProperty("gameOver", out _));
    }

    [Fact]
    public async Task MakeMove_WrongPlayer_Rejected()
    {
        var player1 = CreateClient();

        var getNew = await player1.GetAsync("/Games/New");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getNew);
        var createForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["configName"] = "Classical",
        };
        var createResponse = await player1.PostAsync("/Games/New", new FormUrlEncodedContent(createForm));
        var playUrl = createResponse.Headers.Location!.ToString();
        var gameId = WebTestHelpers.ExtractQueryParam(playUrl, "gameId");

        var player2 = CreateClient();
        var getList = await player2.GetAsync("/Games/List");
        var joinToken = await WebTestHelpers.ExtractAntiForgeryToken(getList);
        var joinForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = joinToken,
            ["name"] = gameId,
        };
        await player2.PostAsync("/Games/List?handler=Join", new FormUrlEncodedContent(joinForm));

        var getPlay = await player2.GetAsync($"/Games/Play?gameId={gameId}");
        var moveToken = await WebTestHelpers.ExtractAntiForgeryToken(getPlay);
        var moveForm = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = moveToken,
            ["column"] = "3",
            ["gameId"] = gameId,
        };
        var moveResponse = await player2.PostAsync("/Games/Play", new FormUrlEncodedContent(moveForm));

        if (moveResponse.StatusCode == HttpStatusCode.Redirect)
        {
            var redirectUrl = moveResponse.Headers.Location!.ToString();
            var afterMove = await player2.GetAsync(redirectUrl);
            var html = await afterMove.Content.ReadAsStringAsync();
            Assert.Contains("not your turn", html, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            var html = await moveResponse.Content.ReadAsStringAsync();
            Assert.Contains("not your turn", html, StringComparison.OrdinalIgnoreCase);
        }
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
