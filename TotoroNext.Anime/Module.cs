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
        // main navigation
        services.AddNavigationViewItem<UserListPage, UserListViewModel>("My List", new SymbolIcon(Symbol.Library))
                .AddNavigationViewItem<SearchProviderPage, SearchProviderViewModel>("Watch Now", new FontIcon { Glyph = "\uE7C5" })
                .AddNavigationViewItem<SearchMetadataProviderPage, SearchMetadataProviderViewModel>("Search Metadata", new FontIcon { Glyph = "\uEDE4" })
                .AddDataViewMap<WatchPage, WatchViewModel, WatchViewModelNavigationParameter>();

        // Pane navigation
        services.AddDataViewMap<AnimeDetailsView, AnimeDetailsViewModel, AnimeModel>()
                .AddDataViewMap<UserListFilterView, UserListFilterViewModel, UserListFilter>();

        services.RegisterEvent<PlaybackProgressEventArgs>()
                .RegisterEvent<PlaybackEndedEventArgs>()
                .RegisterEvent<TrackingUpdateEventArgs>();

        services.AddSelectionUserInteraction<SelectProviderResult, SearchResult>()
                .AddSelectionUserInteraction<SelectAnimeResult, AnimeModel>()
                .AddSelectionUserInteraction<SelectServerResult, VideoServer>();

        services.AddHostedService<TrackingUpdater>()
                .AddHostedService<PlaybackProgressTrackingService>();
    }
}
