using System.IO;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class WatchViewModel(SearchResult result,
                                    IEventAggregator eventAggregator) : ReactiveObject
{
    public IMediaPlayer? MediaPlayer { get; set; }

    [Reactive]
    public partial SearchResult Anime { get; set; }

    [Reactive]
    public partial Episode SelectedEpisode { get; set; }

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
        this.WhenAnyValue(x => x.Anime)
            .WhereNotNull()
            .SelectMany(anime => anime.GetEpisodes().ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);

    public void Initialize()
    {
        InitializeOAPH();

        Anime = result;

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
            .Subscribe(Play);

        InitializePublishers();
    }

    private void InitializePublishers()
    {
        this.WhenAnyValue(x => x.Anime)
            .WhereNotNull()
            .Subscribe(anime => eventAggregator.GetObserver<AnimeSelectedEvent>().OnNext(new(anime)));

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

    private void Play(VideoSource source)
    {
        if(MediaPlayer is null)
        {
            return;
        }

        IEnumerable<string?> parts = [Anime.Title, $"Episode {SelectedEpisode.Number}", source.Title];
        var title = string.Join(" - ", parts.Where(x => !string.IsNullOrEmpty(x)));

        MediaPlayer.Play(new Media(title, source.Url, source.Headers));
    }
}
