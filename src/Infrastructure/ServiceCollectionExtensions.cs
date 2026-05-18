using Application.Config.Interfaces;
using Application.Game.Interfaces;
using Infrastructure.Persistence.Json;
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
}
