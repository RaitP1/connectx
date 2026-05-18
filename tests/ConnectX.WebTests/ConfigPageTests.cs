using System.Net;
using Application.Config.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.WebTests;

public sealed class ConfigPageTests : IClassFixture<ConnectXWebFactory>
{
    private readonly ConnectXWebFactory _factory;

    public ConfigPageTests(ConnectXWebFactory factory) => _factory = factory;

    [Fact]
    public async Task ConfigsIndex_ListsSeededConfigs()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Configs");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Classical", html);
        Assert.Contains("Connect-3", html);
    }

    [Fact]
    public async Task ConfigsCreate_ValidConfig_RedirectsToIndex()
    {
        var client = CreateClient();

        var getResponse = await client.GetAsync("/Configs/Create");
        getResponse.EnsureSuccessStatusCode();
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getResponse);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Config.Name"] = "TestConfig",
            ["Config.Rows"] = "6",
            ["Config.Columns"] = "7",
            ["Config.WinCondition"] = "4",
            ["Config.Topology"] = "Rectangle",
            ["Config.Player1IsAI"] = "false",
            ["Config.Player2IsAI"] = "false",
        };

        var response = await client.PostAsync("/Configs/Create", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.EndsWith("/Configs", response.Headers.Location?.ToString() ?? "");
    }

    [Fact]
    public async Task ConfigsCreate_InvalidWinCondition_ReturnsPage()
    {
        var client = CreateClient();

        var getResponse = await client.GetAsync("/Configs/Create");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getResponse);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Config.Name"] = "BadConfig",
            ["Config.Rows"] = "3",
            ["Config.Columns"] = "3",
            ["Config.WinCondition"] = "10",
            ["Config.Topology"] = "Rectangle",
            ["Config.Player1IsAI"] = "false",
            ["Config.Player2IsAI"] = "false",
        };

        var response = await client.PostAsync("/Configs/Create", new FormUrlEncodedContent(form));

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("win condition", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ConfigsDelete_RemovesConfig()
    {
        var client = CreateClient();

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        var toDelete = new GameConfig("ToDelete", 6, 7, 4, "P1", "X", "P2", "O",
            EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false));
        repo.Save(toDelete);

        var getResponse = await client.GetAsync("/Configs");
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getResponse);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["name"] = "ToDelete",
        };

        var response = await client.PostAsync("/Configs?handler=Delete", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        using var assertScope = _factory.Services.CreateScope();
        var assertRepo = assertScope.ServiceProvider.GetRequiredService<IConfigRepository>();
        Assert.Null(assertRepo.Load("ToDelete"));
    }

    [Fact]
    public async Task ConfigsEdit_UpdatesConfig()
    {
        var client = CreateClient();

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        var original = new GameConfig("EditMe", 6, 7, 4, "P1", "X", "P2", "O",
            EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false));
        repo.Save(original);

        var getResponse = await client.GetAsync("/Configs/Edit?name=EditMe");
        getResponse.EnsureSuccessStatusCode();
        var token = await WebTestHelpers.ExtractAntiForgeryToken(getResponse);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Config.Name"] = "EditMe",
            ["Config.Rows"] = "8",
            ["Config.Columns"] = "9",
            ["Config.WinCondition"] = "5",
            ["Config.Topology"] = "Cylinder",
            ["Config.Player1IsAI"] = "false",
            ["Config.Player2IsAI"] = "false",
        };

        var response = await client.PostAsync("/Configs/Edit?name=EditMe", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        using var assertScope = _factory.Services.CreateScope();
        var assertRepo = assertScope.ServiceProvider.GetRequiredService<IConfigRepository>();
        var updated = assertRepo.Load("EditMe");
        Assert.NotNull(updated);
        Assert.Equal(8, updated.Rows);
        Assert.Equal(9, updated.Columns);
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
