using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

public partial class AnimeDetailsViewModel(AnimeModel anime,
                                           IFactory<IMetadataService, Guid> metaFactory,
                                           IFactory<ITrackingService, Guid> trackerFactory) : ObservableObject, IAsyncInitializable, INavigatorHost
{
    private readonly IMetadataService _metadataService = metaFactory.CreateDefault();

    [ObservableProperty]
    public partial AnimeModel Anime { get; set; } = anime;

    [ObservableProperty]
    public partial ListItemStatus? Status { get; set; } = anime.Tracking?.Status;

    [ObservableProperty]
    public partial double Progress { get; set; } = anime.Tracking?.WatchedEpisodes ?? double.NaN;

    [ObservableProperty]
    public partial double Score { get; set; } = anime.Tracking?.Score ?? double.NaN;

    [ObservableProperty]
    public partial DateTimeOffset? StartDate { get; set; } = anime.Tracking?.StartDate;

    [ObservableProperty]
    public partial DateTimeOffset? FinishDate { get; set; } = anime.Tracking?.FinishDate;

    [ObservableProperty]
    public partial LoadableAction InitializeAction { get; set; }

    public ListItemStatus[] Statuses { get; } = [.. Enum.GetValues<ListItemStatus>()];

    public INavigator? Navigator { get; set; }

    public async Task InitializeAsync()
    {
        Anime = await _metadataService.GetAnimeAsync(Anime.Id) ?? Anime;

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
}
