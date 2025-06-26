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

public partial class AnimeDetailsViewModel(AnimeModel anime,
                                           IFactory<IMetadataService, Guid> metaFactory,
                                           IFactory<IAnimeProvider, Guid> providerFactory,
                                           IFactory<ITrackingService, Guid> trackerFactory,
                                           IEvent<NavigateToDataRequest> dataNavRequest) : ObservableObject, IAsyncInitializable
{
    private readonly IMetadataService _metadataService = metaFactory.CreateDefault();
    private readonly IAnimeProvider _provider = providerFactory.CreateDefault();

    [ObservableProperty]
    public partial AnimeModel Anime { get; set; } = anime;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueWatchingCommand))]
    public partial List<EpisodeInfo> Episodes { get; set; } = [];

    [ObservableProperty]
    public partial ListItemStatus Status { get; set; } = anime.Tracking!.Status!.Value;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueWatchingCommand))]
    public partial double Progress { get; set; } = anime.Tracking!.WatchedEpisodes!.Value;

    [ObservableProperty]
    public partial double Score { get; set; } = anime.Tracking!.Score!.Value;

    [ObservableProperty]
    public partial DateTimeOffset? StartDate { get; set; } = anime.Tracking!.StartDate;

    [ObservableProperty]
    public partial DateTimeOffset? FinishDate { get; set; } = anime.Tracking!.FinishDate;


    public ListItemStatus[] Statuses { get; } = [.. Enum.GetValues<ListItemStatus>()];

    public async Task InitializeAsync()
    {
        Anime = await _metadataService.GetAnimeAsync(Anime.Id) ?? Anime;
        Episodes = await Anime.GetEpisodes();

        this.WhenAnyValue(x => x.Status, x => x.Progress, x => x.Score, x => x.StartDate, x => x.FinishDate)
            .Skip(1)
            .Select(x => new Tracking 
            {
                Status = x.Item1,
                WatchedEpisodes = (int)x.Item2,
                Score = (int)x.Item3,
                StartDate = x.Item4?.DateTime,
                FinishDate = x.Item5?.DateTime
            })
            .SelectMany(tracking =>
            {
                var tasks = trackerFactory.CreateAll()
                                          .Select(tracker => new ValueTuple<ITrackingService, long?>(tracker, Anime.ExternalIds.GetId(tracker.ServiceName)))
                                          .Where(x => x.Item2 is not null)
                                          .Select(x => x.Item1.Update(x.Item2!.Value, tracking));

                return Task.WhenAll(tasks);
            })
            .Subscribe();
    }

    [RelayCommand]
    private async Task WatchEpisode(EpisodeInfo episode)
    {
        var searchResult = await _provider.SearchAndSelectAsync(Anime);

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

        dataNavRequest.Publish(new(new WatchViewModelNavigationParameter(searchResult, Anime, episodes, selectedEpisode, false)));
    }

    [RelayCommand(CanExecute = nameof(CanContinueWatching))]
    private async Task ContinueWatching()
    {
        if (Anime is { Tracking.WatchedEpisodes: 0 or null })
        {
            await WatchEpisode(Episodes.First());
            return;
        }

        await WatchEpisode(Episodes.First(x => x.EpisodeNumber == Anime.Tracking!.WatchedEpisodes!.Value + 1));
    }

    private bool CanContinueWatching()
    {
        if(Episodes is null or { Count : 0 })
        {
            return false;
        }

        if(Progress is not > 0)
        {
            return true;
        }

        return Progress < Episodes.Max(x => x.EpisodeNumber);
    }
}
