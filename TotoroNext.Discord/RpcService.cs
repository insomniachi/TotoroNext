using System.Reactive.Linq;
using DiscordRPC;
using Microsoft.Extensions.Hosting;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Discord;

internal class RpcService(IEvent<PlaybackProgressEventArgs> playbackProgressEvent,
                          IEvent<PlaybackEndedEventArgs> plabackEndedEvent) : IHostedService
{
    private readonly DiscordRpcClient _client = new("997177919052984622");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var initialized = _client.Initialize();
        
        playbackProgressEvent.OnNext()
            .Where((e, idx) => idx % 20 == 0)
            .Subscribe(e =>
            {
                var now = DateTime.UtcNow;
                _client.Update(p =>
                {
                    p.Type = ActivityType.Watching;
                    p.Details = e.Anime.Title;
                    p.State = $"Episode {e.Episode.Number}";
                    p.Assets ??= new();
                    p.Assets.LargeImageKey = e.Anime.Image ?? "icon";
                    p.Timestamps = new Timestamps()
                    {
                        Start = now - e.Position,
                        End = now + (e.Duration - e.Position)
                    };
                });
            });

        plabackEndedEvent.OnNext().Subscribe(_ =>
        {
            _client.ClearPresence();
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
