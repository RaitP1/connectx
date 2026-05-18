using Application.Config.Interfaces;
using ConsoleApp;
using ConsoleApp.UI;
using Infrastructure;
using Infrastructure.Persistence.EF;
using Microsoft.Extensions.DependencyInjection;

var useEf = !args.Contains("--json");

var services = new ServiceCollection();

if (useEf)
{
    var dbDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ConnectX");
    Directory.CreateDirectory(dbDir);
    var dbPath = Path.Combine(dbDir, "ConnectX.db");
    services.AddEfPersistence($"Data Source={dbPath}");
}
else
{
    services.AddJsonPersistence();
}

services.AddScoped<GameUI>();
services.AddScoped<GameController>();

var provider = services.BuildServiceProvider();
using (var scope = provider.CreateScope())
{
    if (useEf)
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

    var configRepo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
    DefaultConfigSeeder.Seed(configRepo);

    var controller = scope.ServiceProvider.GetRequiredService<GameController>();
    controller.Run();
}
