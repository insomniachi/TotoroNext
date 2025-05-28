using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Contracts;

namespace TotoroNext.Anime.Services;

internal class AnimeProviderFactory(IServiceScopeFactory serviceScopeFactory) : IAnimeProviderFactory
{
    public IAnimeProvider GetProvider(Guid id)
    {
        using var scope = serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredKeyedService<IAnimeProvider>(id);
    }
}
