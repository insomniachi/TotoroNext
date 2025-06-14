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
public sealed partial class WatchViewModel(WatchViewModelNavigationParameter navigationParameter,
                                    IEvent<PlaybackProgressEventArgs> playbackProgressEvent,
                                    IEvent<PlaybackEndedEventArgs> playbackEndedEvent,
                                    IFactory<IMediaSegmentsProvider, Guid> segmentsFactory,
                                    IFactory<IMediaPlayer, Guid> mediaPlayerFactory) : ReactiveObject, IInitializable, IDisposable
{
    private TimeSpan _duration;

    public IMediaPlayer? MediaPlayer { get; } = mediaPlayerFactory.CreateDefault();

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

    public void Dispose()
    {
        playbackEndedEvent.Publish(new());
    }

    private void InitializePublishers()
    {
        if(MediaPlayer is null)
        {
            return;
        }

        MediaPlayer
            .PositionChanged
            .Where(_ => Anime is not null && SelectedEpisode is not null)
            .Subscribe(position => playbackProgressEvent.Publish(new(Anime!, SelectedEpisode!, _duration, position)));

        MediaPlayer
            .DurationChanged
            .Subscribe(duration => _duration = duration);
    }

    private async Task Play(VideoSource source)
    {
        if (MediaPlayer is null || SelectedEpisode is null)
        {
            return;
        }

        IEnumerable<string?> parts = [ProviderResult.Title, $"Episode {SelectedEpisode.Number}", source.Title];
        var title = string.Join(" - ", parts.Where(x => !string.IsNullOrEmpty(x)));

        var duration = MediaHelper.GetDuration(source.Url, source.Headers);
        List<MediaSegment> segments = [];

        if (Anime is not null && segmentsFactory.CreateDefault() is { } segmentsProvider)
        {
            segments.AddRange(await segmentsProvider.GetSegments(Anime.MalId, SelectedEpisode.Number, duration.TotalSeconds));
        }

        var metadata = new MediaMetadata(title, source.Headers, segments);

        MediaPlayer.Play(new Media(source.Url, metadata));
    }
}
