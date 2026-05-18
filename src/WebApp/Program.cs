using Application.Config.Interfaces;
using Domain;
using Infrastructure;
using Infrastructure.Persistence.EF;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.IsEssential = true;
});

var useJson = builder.Configuration["Persistence"] == "Json";

if (useJson)
{
    builder.Services.AddJsonPersistence();
}
else
{
    var dbDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ConnectX");
    Directory.CreateDirectory(dbDir);
    var defaultDb = $"Data Source={Path.Combine(dbDir, "ConnectX.db")}";
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? defaultDb;
    builder.Services.AddEfPersistence(connectionString);
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    if (!useJson)
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

    var configRepo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
    SeedDefaultConfigs(configRepo);
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapRazorPages();

app.Run();

static void SeedDefaultConfigs(IConfigRepository configRepository)
{
    if (configRepository.List().Count > 0)
        return;

    GameConfig[] defaults =
    [
        new("Classical", 6, 7, 4, "Player 1", "X", "Player 2", "O",
            EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false)),
        new("Connect-3", 4, 5, 3, "Player 1", "X", "Player 2", "O",
            EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false)),
        new("Connect-5", 7, 9, 5, "Player 1", "X", "Player 2", "O",
            EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false)),
        new("Connect-4 Cylinder", 6, 7, 4, "Player 1", "X", "Player 2", "O",
            EBoardTopology.Cylinder, new PlayerType(false), new PlayerType(false)),
    ];

    foreach (var config in defaults)
        configRepository.Save(config);
}

public partial class Program;
