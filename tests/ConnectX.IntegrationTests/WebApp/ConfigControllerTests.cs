using System.Net;
using System.Text.RegularExpressions;
using Application.Config.Interfaces;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.IntegrationTests.WebApp;

public sealed partial class ConfigControllerTests : IClassFixture<ConnectXWebFactory>, IDisposable
{
    private readonly ConnectXWebFactory _factory;
    private readonly HttpClient _client;

    public ConfigControllerTests(ConnectXWebFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task Index_ReturnsConfigList()
    {
        SeedConfig("ListTest");

        var response = await _client.GetAsync("/Config");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("ListTest", html);
    }

    [Fact]
    public async Task Create_Get_ReturnsForm()
    {
        var response = await _client.GetAsync("/Config/Create");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Name", html);
        Assert.Contains("Rows", html);
        Assert.Contains("Topology", html);
    }

    [Fact]
    public async Task Create_Post_SavesConfigAndRedirects()
    {
        var token = await GetAntiForgeryToken("/Config/Create");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Name"] = "WebTest",
            ["Rows"] = "6",
            ["Columns"] = "7",
            ["WinCondition"] = "4",
            ["Player1Name"] = "P1",
            ["Player1Symbol"] = "X",
            ["Player2Name"] = "P2",
            ["Player2Symbol"] = "O",
            ["Topology"] = "Rectangle",
            ["Player1Type"] = "Human",
            ["Player2Type"] = "Human",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Config/Create", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        var config = repo.Load("WebTest");
        Assert.NotNull(config);
        Assert.Equal(6, config.Rows);
    }

    [Fact]
    public async Task Create_Post_InvalidConfig_ReturnsForm()
    {
        var token = await GetAntiForgeryToken("/Config/Create");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Name"] = "BadConfig",
            ["Rows"] = "3",
            ["Columns"] = "3",
            ["WinCondition"] = "99",
            ["Player1Name"] = "P1",
            ["Player1Symbol"] = "X",
            ["Player2Name"] = "P2",
            ["Player2Symbol"] = "X",
            ["Topology"] = "Rectangle",
            ["Player1Type"] = "Human",
            ["Player2Type"] = "Human",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Config/Create", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid configuration", html);
    }

    [Fact]
    public async Task Edit_Get_LoadsExistingConfig()
    {
        SeedConfig("EditTest");

        var response = await _client.GetAsync("/Config/Edit/EditTest");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("EditTest", html);
    }

    [Fact]
    public async Task Edit_Post_UpdatesConfig()
    {
        SeedConfig("EditSave");

        var token = await GetAntiForgeryToken("/Config/Edit/EditSave");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Name"] = "EditSave",
            ["Rows"] = "8",
            ["Columns"] = "9",
            ["WinCondition"] = "5",
            ["Player1Name"] = "P1",
            ["Player1Symbol"] = "X",
            ["Player2Name"] = "P2",
            ["Player2Symbol"] = "O",
            ["Topology"] = "Cylinder",
            ["Player1Type"] = "Human",
            ["Player2Type"] = "Easy AI",
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Config/Edit/EditSave", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        var config = repo.Load("EditSave");
        Assert.NotNull(config);
        Assert.Equal(8, config.Rows);
        Assert.Equal(EBoardTopology.Cylinder, config.Topology);
    }

    [Fact]
    public async Task Delete_RemovesConfig()
    {
        SeedConfig("DeleteMe");

        var token = await GetAntiForgeryToken("/Config");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
        });

        var response = await _client.PostAsync("/Config/Delete/DeleteMe", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        Assert.Null(repo.Load("DeleteMe"));
    }

    private void SeedConfig(string name)
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
        repo.Save(new GameConfig(name, 6, 7, 4, "P1", "X", "P2", "O",
            EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false)));
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
