using System.Reactive.Linq;
using Microsoft.Extensions.Hosting;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;
using Uno.Disposables;

namespace TotoroNext.Anime.Abstractions;

public abstract class TrackingUpdater(ITrackingService trackingService,
                                      IEvent<PlaybackProgressEventArgs> playbackProgressEvent) : IHostedService
{
    private readonly SerialDisposable _subscription = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        playbackProgressEvent.OnNext()
            .Where(e => (e.Anime.Tracking?.WatchedEpisodes ?? 0) < e.Episode.Number )
            .Where(e => e.Duration - e.Position < TimeSpan.FromMinutes(2))
            .SelectMany(e =>
            {
                if (e.Anime.Tracking is null)
                {
                    e.Anime.Tracking = new Tracking
                    {
                        Status = ListItemStatus.Watching,
                        StartDate = DateTime.Now,
                    };
                }

                var tracking = e.Anime.Tracking;
                tracking.WatchedEpisodes = (int)e.Episode.Number;

                if (e.Anime.TotalEpisodes == e.Episode.Number)
                {
                    tracking.Status = ListItemStatus.Completed;
                }

                return trackingService.Update(e.Anime.Id, tracking);
            })
            .Subscribe()
            .DisposeWith(_subscription);
  
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if(_subscription.IsDisposed)
        {
            return Task.CompletedTask;
        }

        _subscription.Dispose();

        return Task.CompletedTask;
    }
}
