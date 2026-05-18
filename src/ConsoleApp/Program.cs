using Application.Config.Interfaces;
using ConsoleApp;
using ConsoleApp.UI;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddJsonPersistence();
services.AddSingleton<GameUI>();
services.AddSingleton<GameController>();

var provider = services.BuildServiceProvider();

var configRepo = provider.GetRequiredService<IConfigRepository>();
DefaultConfigSeeder.Seed(configRepo);

var controller = provider.GetRequiredService<GameController>();
controller.Run();
