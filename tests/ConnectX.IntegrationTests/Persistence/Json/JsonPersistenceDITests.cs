using Application.Config.Interfaces;
using Application.Game.Interfaces;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.IntegrationTests.Persistence.Json;

public sealed class JsonPersistenceDITests
{
    [Fact]
    public void AddJsonPersistence_RegistersConfigRepository()
    {
        var services = new ServiceCollection();
        services.AddJsonPersistence();
        var provider = services.BuildServiceProvider();

        var repo = provider.GetService<IConfigRepository>();

        Assert.NotNull(repo);
    }

    [Fact]
    public void AddJsonPersistence_RegistersGameRepository()
    {
        var services = new ServiceCollection();
        services.AddJsonPersistence();
        var provider = services.BuildServiceProvider();

        var repo = provider.GetService<IGameRepository>();

        Assert.NotNull(repo);
    }

    [Fact]
    public void AddJsonPersistence_RegistersSingletonLifetime()
    {
        var services = new ServiceCollection();
        services.AddJsonPersistence();
        var provider = services.BuildServiceProvider();

        var repo1 = provider.GetService<IConfigRepository>();
        var repo2 = provider.GetService<IConfigRepository>();

        Assert.Same(repo1, repo2);
    }
}
