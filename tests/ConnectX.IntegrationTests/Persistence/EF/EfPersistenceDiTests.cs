using Application.Config.Interfaces;
using Application.Game.Interfaces;
using Infrastructure;
using Infrastructure.Persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectX.IntegrationTests.Persistence.EF;

public sealed class EfPersistenceDiTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"connectx-test-{Guid.NewGuid()}.db");
    private readonly ServiceProvider _provider;

    public EfPersistenceDiTests()
    {
        var services = new ServiceCollection();
        services.AddEfPersistence($"Data Source={_dbPath}");
        _provider = services.BuildServiceProvider();

        using var scope = _provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }

    public void Dispose()
    {
        _provider.Dispose();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Fact]
    public void ConfigRepository_ResolvesToEfImplementation()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();

        Assert.IsType<EfConfigRepository>(repo);
    }

    [Fact]
    public void GameRepository_ResolvesToEfImplementation()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        Assert.IsType<EfGameRepository>(repo);
    }

    [Fact]
    public void DatabaseFileCreated()
    {
        Assert.True(File.Exists(_dbPath));
    }
}
