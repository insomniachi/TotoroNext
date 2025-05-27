using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Anime.Views;
using TotoroNext.Module;

namespace TotoroNext.Anime;

public class AnimeModule : IModule
{
    public void ConfigureNavigation(NavigationViewContext context)
    {
        context.RegisterForNavigation<SearchProviderPage, SearchProviderViewModel>();
        context.RegisterForNavigation<WatchPage, WatchViewModel, SearchResult>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddNavigationViewItem<SearchProviderViewModel>("Search", new SymbolIcon(Symbol.Find))
                .RegisterEvent<AnimeSelectedEvent>()
                .RegisterEvent<EpisodeSelectedEvent>()
                .RegisterEvent<PlaybackDurationChangedEvent>()
                .RegisterEvent<PlaybackPositionChangedEvent>();
    }
}
