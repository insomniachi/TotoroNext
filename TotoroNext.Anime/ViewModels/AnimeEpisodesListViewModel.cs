using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.Extensions;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

public partial class AnimeEpisodesListViewModel(EpisodesListViewModelNagivationParameters @params,
                                                IPlaybackProgressService playbackProgressService,
                                                IFactory<IAnimeProvider, Guid> providerFactory,
                                                IEvent<NavigateToDataRequest> dataNavRequest) : ObservableObject, IAsyncInitializable
{
    [ObservableProperty]
    public partial AnimeModel Anime { get; set; } = @params.Anime;

    [ObservableProperty]
    public partial List<EpisodeInfo> Episodes { get; set; }

    [ObservableProperty]
    public partial EpisodeInfo? SelectedEpisode { get; set; }

    [ObservableProperty]
    public partial LoadableAction? InitializeAction { get; set; }

    public async Task InitializeAsync()
    {
        InitializeAction = LoadableAction.Create(UpdateEpisodes);
        await InitializeAction.Execute();

        this.WhenAnyValue(x => x.Episodes)
            .Where(x => x is { Count: > 0 })
            .Subscribe(_ => SelectedEpisode = GetNextUp());
    }

    [RelayCommand]
    private async Task WatchEpisode(EpisodeInfo episode)
    {
        var provider = providerFactory.CreateDefault();
        var searchResult = await provider.SearchAndSelectAsync(Anime);

        if (searchResult is null)
        {
            return;
        }

        var episodes = await searchResult.GetEpisodes().ToListAsync();
        var selectedEpisode = episodes.FirstOrDefault(x => (int)x.Number == episode.EpisodeNumber);

        if (selectedEpisode is null)
        {
            return;
        }

        if (episode.Progress is { } info)
        {
            selectedEpisode.StartPosition = TimeSpan.FromSeconds(info.Position);
        }

        dataNavRequest.Publish(new(new WatchViewModelNavigationParameter(searchResult,
                                                                                              Anime,
                                                                                              episodes,
                                                                                              selectedEpisode,
                                                                                              false)));
    }

    private async Task UpdateEpisodes()
    {
        var eps = await Anime.GetEpisodes();
        var progress = playbackProgressService.GetProgress(Anime.Id);

        foreach (var item in progress)
        {
            if (eps.FirstOrDefault(x => x.EpisodeNumber == item.Key) is { } ep)
            {
                ep.Progress = item.Value;
            }
        }

        Episodes = eps;
    }

    private EpisodeInfo? GetNextUp()
    {
        if (Anime is { Tracking.WatchedEpisodes: 0 or null })
        {
            return Episodes.FirstOrDefault();
        }

        return Episodes.FirstOrDefault(x => x.EpisodeNumber == Anime.Tracking!.WatchedEpisodes!.Value + 1);
    }
}
