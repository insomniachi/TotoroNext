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
        services.AddSingleton<IPlaybackProgressService, PlaybackProgressTrackingService>();

        // main navigation
        services.AddNavigationViewItem<UserListPage, UserListViewModel>("My List", new SymbolIcon(Symbol.Library))
                .AddNavigationViewItem<SearchMetadataProviderPage, SearchMetadataProviderViewModel>("Search", new FontIcon { Glyph = "\uE721" })
                .AddDataViewMap<WatchPage, WatchViewModel, WatchViewModelNavigationParameter>();

        // Pane navigation
        services.AddDataViewMap<AnimeDetailsView, AnimeDetailsViewModel, AnimeModel>()
                .AddDataViewMap<UserListFilterView, UserListFilterViewModel, UserListFilter>()
                .AddDataViewMap<AnimeEpisodesListView, AnimeEpisodesListViewModel, EpisodesListViewModelNagivationParameters>()
                .AddDataViewMap<AnimeGridView, AnimeGridViewModel, List<AnimeModel>>()
                .AddDataViewMap<AnimeOverridesView, AnimeOverridesViewModel, OverridesViewModelNavigationParameters>();

        services.AddSelectionUserInteraction<SelectProviderResult, SearchResult>()
                .AddSelectionUserInteraction<SelectAnimeResult, AnimeModel>()
                .AddSelectionUserInteraction<SelectServerResult, VideoServer>();

        services.AddHostedService<TrackingUpdater>()
                .AddHostedService(sp => sp.GetRequiredService<IPlaybackProgressService>());
    }
}
