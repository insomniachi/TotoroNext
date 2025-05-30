using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Anime.Views;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime;

public class Module : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDataViewMap<WatchPage, WatchViewModel, SearchResult>();

        services.AddMainNavigationViewItem<SearchProviderPage, SearchProviderViewModel>("Search", new SymbolIcon(Symbol.Find))
                .RegisterEvent<AnimeSelectedEvent>()
                .RegisterEvent<EpisodeSelectedEvent>()
                .RegisterEvent<PlaybackDurationChangedEvent>()
                .RegisterEvent<PlaybackPositionChangedEvent>();
    }
}
