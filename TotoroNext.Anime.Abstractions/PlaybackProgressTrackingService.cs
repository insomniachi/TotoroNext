using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Abstractions;

public class PlaybackProgressTrackingService(IEvent<PlaybackProgressEventArgs> progressEvent,
                                             IEvent<TrackingUpdateEventArgs> trackingUpdateEvent) : IHostedService
{
    private readonly string _file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", $"progress.json");
    private Dictionary<string, ProgressInfo> _progress = [];

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _progress = JsonSerializer.Deserialize<Dictionary<string, ProgressInfo>>(File.ReadAllText(_file)) ?? [];

        progressEvent.OnNext()
            .Subscribe(e =>
            {
                var key = $"{e.Anime.Id}_{e.Episode.Number}";
                if (_progress.TryGetValue(key, out var info))
                {
                    info.Position = e.Position.TotalSeconds;
                }
                else
                {   
                    _progress[key] = new ProgressInfo
                    {
                        Position = e.Position.TotalSeconds,
                    };
                }
            });

        trackingUpdateEvent.OnNext()
            .Subscribe(e =>
            {
                var key = $"{e.Anime.Id}_{e.Episode.Number}";
                if (_progress.TryGetValue(key, out var info))
                {
                    info.IsCompleted = e.Anime.TotalEpisodes == e.Episode.Number;
                }
            });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var completed = _progress.Where(x => x.Value.IsCompleted).Select(x => x.Key);
        _progress.RemoveKeys(completed);

        
        File.WriteAllText(_file, JsonSerializer.Serialize(_progress));
        return Task.CompletedTask;
    }
}

internal class ProgressInfo
{
    public double Position { get; set; }

    [JsonIgnore]
    public bool IsCompleted { get; set; }
}
