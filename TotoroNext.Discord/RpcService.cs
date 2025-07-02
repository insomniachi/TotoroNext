using CommunityToolkit.Mvvm.Messaging;
using DiscordRPC;
using Microsoft.Extensions.Hosting;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Discord;

internal class RpcService(IModuleSettings<Settings> settings,
                          IMessenger messenger) : IHostedService, IRecipient<PlaybackState>, IRecipient<PlaybackEnded>
{
    private readonly DiscordRpcClient _client = new("997177919052984622");
    private readonly Settings _settings = settings.Value;

    public void Receive(PlaybackState message)
    {
        if(!_settings.IsEnabled)
        {
            return;
        }

        var now = DateTime.UtcNow;
        _client.Update(p =>
        {
            p.Type = ActivityType.Watching;
            p.Details = message.Anime.Title;
            p.State = $"Episode {message.Episode.Number}";
            p.Assets ??= new();
            p.Assets.LargeImageKey = message.Anime.Image ?? "icon";
            p.Timestamps = new Timestamps()
            {
                Start = now - message.Position,
                End = now + (message.Duration - message.Position)
            };
        });
    }

    public void Receive(PlaybackEnded message)
    {
        _client.ClearPresence();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Initialize();
        messenger.Register<PlaybackState>(this);
        messenger.Register<PlaybackEnded>(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _client.Deinitialize();
        messenger.Unregister<PlaybackState>(this);
        messenger.Unregister<PlaybackEnded>(this);
        return Task.CompletedTask;
    }
}
