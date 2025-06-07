using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using JetBrains.Annotations;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class WatchViewModel(WatchViewModelNavigationParameter navigationParameter,
                                    IEventAggregator eventAggregator,
                                    IFactory<IMediaSegmentsProvider, Guid> segmentsFactory) : ReactiveObject, IInitializable
{
    public IMediaPlayer? MediaPlayer { get; set; }

    [Reactive]
    public partial SearchResult ProviderResult { get; set; }

    [Reactive]
    public partial AnimeModel? Anime { get; set; }

    [Reactive]
    public partial Episode? SelectedEpisode { get; set; }

    [Reactive]
    public partial VideoServer SelectedServer { get; set; }

    [Reactive]
    public partial VideoSource SelectedSource { get; set; }

    [ObservableAsProperty(PropertyName = "Servers")]
    private IObservable<List<VideoServer>> ServersObservable() =>
        this.WhenAnyValue(x => x.SelectedEpisode)
            .WhereNotNull()
            .SelectMany(ep => ep.GetServersAsync().ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);

    [ObservableAsProperty(PropertyName = "Sources")]
    private IObservable<List<VideoSource>> StreamObservable() =>
        this.WhenAnyValue(x => x.SelectedServer)
            .WhereNotNull()
            .SelectMany(server => server.Extract().ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);

    [ObservableAsProperty(PropertyName = "Episodes")]
    private IObservable<List<Episode>> EpisodesObservable() =>
        this.WhenAnyValue(x => x.ProviderResult)
            .WhereNotNull()
            .SelectMany(anime => anime.GetEpisodes().ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);

    public void Initialize()
    {
        InitializeOAPH();

        (ProviderResult, Anime) = navigationParameter;

        this.WhenAnyValue(x => x.Episodes)
            .WhereNotNull()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(eps =>
            {
                var watched = (Anime?.Tracking?.WatchedEpisodes ?? 0) + 1;
                SelectedEpisode = eps.FirstOrDefault(x => x.Number == watched);
            });

        this.WhenAnyValue(x => x.Servers)
            .Where(x => x is { Count: > 0 })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => SelectedServer = x.First());

        this.WhenAnyValue(x => x.Sources)
            .Where(x => x is { Count: 1 })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => SelectedSource = x.First());

        this.WhenAnyValue(x => x.SelectedSource)
            .WhereNotNull()
            .SelectMany(x => Play(x).ToObservable())
            .Subscribe();

        InitializePublishers();
    }

    private void InitializePublishers()
    {
        this.WhenAnyValue(x => x.Anime)
            .WhereNotNull()
            .Subscribe(anime => eventAggregator.GetObserver<TrackableAnimeSelectedEvent>().OnNext(new(anime)));

        this.WhenAnyValue(x => x.ProviderResult)
            .WhereNotNull()
            .Subscribe(result => eventAggregator.GetObserver<AnimeSelectedEvent>().OnNext(new(result)));

        this.WhenAnyValue(x => x.SelectedEpisode)
            .WhereNotNull()
            .Subscribe(ep => eventAggregator.GetObserver<EpisodeSelectedEvent>().OnNext(new(ep)));

        this.WhenAnyValue(x => x.MediaPlayer)
            .WhereNotNull()
            .SelectMany(x => x.DurationChanged)
            .Subscribe(duration => eventAggregator.GetObserver<PlaybackDurationChangedEvent>().OnNext(new(duration)));

        this.WhenAnyValue(x => x.MediaPlayer)
            .WhereNotNull()
            .SelectMany(x => x.PositionChanged)
            .Subscribe(position => eventAggregator.GetObserver<PlaybackPositionChangedEvent>().OnNext(new(position)));
    }

    private async Task Play(VideoSource source)
    {
        if(MediaPlayer is null || SelectedEpisode is null)
        {
            return;
        }

        IEnumerable<string?> parts = [ProviderResult.Title, $"Episode {SelectedEpisode.Number}", source.Title];
        var title = string.Join(" - ", parts.Where(x => !string.IsNullOrEmpty(x)));

        var duration = MediaHelper.GetDuration(source.Url, source.Headers);
        List<MediaSegment> segments = [];

        if(Anime is not null)
        {
            var segmentsProvider = segmentsFactory.Create(new Guid("5ccd59c9-7fd1-485e-b542-e4b8cfaf5655"));
            segments.AddRange(await segmentsProvider.GetSegments(Anime.MalId, SelectedEpisode.Number, duration.TotalSeconds));
        }

        var metadata = new MediaMetadata(title, source.Headers, segments);

        MediaPlayer.Play(new Media(source.Url, metadata));
    }
}
