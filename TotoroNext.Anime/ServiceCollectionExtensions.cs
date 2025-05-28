using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Contracts;

namespace TotoroNext.Anime;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnimeServices(this IServiceCollection services)
    {
        services.AddSingleton<IAnimeProviderFactory, Services.AnimeProviderFactory>();

        return services;
    }
}
