using Application.Config.Interfaces;
using ConsoleApp;
using ConsoleApp.UI;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddJsonPersistence();
// services.AddEfPersistence("Data Source=ConnectX.db");

services.AddSingleton<GameUI>();
services.AddSingleton<GameController>();

var provider = services.BuildServiceProvider();
//    using (var scope = provider.CreateScope())
//        scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.EF.AppDbContext>().Database.EnsureCreated();

var configRepo = provider.GetRequiredService<IConfigRepository>();
DefaultConfigSeeder.Seed(configRepo);

var controller = provider.GetRequiredService<GameController>();
controller.Run();
