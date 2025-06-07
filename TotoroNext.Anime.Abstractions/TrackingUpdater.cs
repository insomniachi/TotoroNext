using System.Reactive.Linq;
using Microsoft.Extensions.Hosting;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Abstractions;

public abstract class TrackingUpdater(ITrackingService trackingService,
                                      IEventAggregator eventAggregator) : IHostedService
{
    private AnimeModel? _anime;
    private Episode? _episode;
    private TimeSpan? _duration;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        eventAggregator.GetObservable<TrackableAnimeSelectedEvent>().Subscribe(e => _anime = e.Item);

        eventAggregator.GetObservable<EpisodeSelectedEvent>()
                       .Where(_ => _anime is not null)
                       .Where(e => e.Item.Number > (_anime!.Tracking?.WatchedEpisodes ?? 0))
                       .Subscribe(e => _episode = e.Item);

        eventAggregator.GetObservable<PlaybackDurationChangedEvent>()
                       .Where(_ => _episode is not null)
                       .Subscribe(e => _duration = e.Duration);

        eventAggregator.GetObservable<PlaybackPositionChangedEvent>()
                       .Where(_ => _duration is not null)
                       .Where(e => _duration - e.Position < TimeSpan.FromMinutes(2))
                       .FirstAsync()
                       .SelectMany(_ =>
                       {
                           var tracking = new Tracking
                           {
                               WatchedEpisodes = (int?)_episode?.Number
                           };
                           return trackingService.Update(_anime!.Id, tracking);
                       })
                       .Subscribe();


        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
