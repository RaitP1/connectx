using Application.Config.Interfaces;
using Application.Game.Interfaces;
using Infrastructure.Persistence.EF;
using Infrastructure.Persistence.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonPersistence(this IServiceCollection services)
    {
        services.AddSingleton<FilesystemHelper>();
        services.AddSingleton<IConfigRepository, JsonConfigRepository>();
        services.AddSingleton<IGameRepository, JsonGameRepository>();
        return services;
    }

    public static IServiceCollection AddEfPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddScoped<IConfigRepository, EfConfigRepository>();
        services.AddScoped<IGameRepository, EfGameRepository>();
        return services;
    }
}
