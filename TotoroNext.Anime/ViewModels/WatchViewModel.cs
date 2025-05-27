using System.IO;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class WatchViewModel(SearchResult result) : ReactiveObject
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
            .Where(x => x is { Count: > 0})
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => SelectedServer = x.First());

        this.WhenAnyValue(x => x.Sources)
            .Where(x => x is { Count: 1 })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => SelectedSource = x.First());

        this.WhenAnyValue(x => x.SelectedSource)
            .WhereNotNull()
            .Subscribe(Play);
    }

    private void Play(VideoSource source)
    {
        if(MediaPlayer is null)
        {
            return;
        }

        MediaPlayer.Play(source.Url, source.Headers);
    }
}
