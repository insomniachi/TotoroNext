using System.Reactive.Linq;
using Microsoft.Extensions.Hosting;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using Uno.Disposables;

namespace TotoroNext.Anime.Abstractions;

public class TrackingUpdater(IFactory<ITrackingService, Guid> factory,
                             IEvent<PlaybackProgressEventArgs> playbackProgressEvent) : IHostedService
{
    private readonly SerialDisposable _subscription = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        playbackProgressEvent.OnNext()
            .Where(e => (e.Anime.Tracking?.WatchedEpisodes ?? 0) < e.Episode.Number)
            .Where(e => e.Duration - e.Position < TimeSpan.FromMinutes(2))
            .SelectMany(e =>
            {
                if(e.Anime.Tracking is null)
                {
                    e.Anime.Tracking = new Tracking
                    {
                        Status = ListItemStatus.Watching,
                        StartDate = DateTime.Now,
                    };
                }

                var tracking = e.Anime.Tracking;
                
                tracking.WatchedEpisodes = (int)e.Episode.Number;
                tracking.Status = e.Anime.TotalEpisodes == e.Episode.Number ? ListItemStatus.Completed : ListItemStatus.Watching;

                var tasks = factory.CreateAll()
                                   .Select(service => new Tuple<ITrackingService, long?>(service, e.Anime.ExternalIds.GetId(service.ServiceName)))
                                   .Where(x => x.Item2 is not null)
                                   .Select(tuple => tuple.Item1.Update(tuple.Item2!.Value, tracking));

                return Task.WhenAll(tasks);
            })
            .Subscribe()
            .DisposeWith(_subscription);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_subscription.IsDisposed)
        {
            return Task.CompletedTask;
        }

        _subscription.Dispose();

        return Task.CompletedTask;
    }

    protected virtual long GetId(AnimeModel anime) => anime.Id;
}
