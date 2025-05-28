using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.Anime.AllAnime;

public class Module : IModule
{
    public void ConfigureNavigation(NavigationViewContext context) { }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKeyedTransient<IAnimeProvider, AnimeProvider>("AllAnime");
    }
}
