using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Anime.Views;
using TotoroNext.Module;

namespace TotoroNext.Anime;

public class AnimeModule : IModule
{
    public void ConfigureNavigation(NavigationViewContext context)
    {
        context.RegisterForNavigation<DiscoverPage, DiscoverViewModel>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddNavigationViewItem<DiscoverViewModel>("Discover", new SymbolIcon(Symbol.World));
    }
}
