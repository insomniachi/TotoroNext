using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.UserInteractions;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Anime.Views;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime;

public class Module : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddMainNavigationViewItem<UserListPage, UserListViewModel>("My List", new SymbolIcon(Symbol.Library))
                .AddMainNavigationViewItem<SearchProviderPage, SearchProviderViewModel>("Watch Now", new FontIcon { Glyph = "\uE7C5" })
                .AddMainNavigationViewItem<SearchMetadataProviderPage, SearchMetadataProviderViewModel>("Search Metadata", new FontIcon { Glyph = "\uEDE4" })
                .AddDataViewMap<WatchPage, WatchViewModel, WatchViewModelNavigationParameter>();

        services.RegisterEvent<PlaybackProgressEventArgs>()
                .RegisterEvent<PlaybackEndedEventArgs>();

        services.AddSelectionUserInteraction<SelectProviderResult, SearchResult>()
                .AddSelectionUserInteraction<SelectAnimeResult, AnimeModel>();
    }
}
