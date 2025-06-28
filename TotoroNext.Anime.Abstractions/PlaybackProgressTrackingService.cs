using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Abstractions;

public class PlaybackProgressTrackingService(IEvent<PlaybackProgressEventArgs> progressEvent,
                                             IEvent<TrackingUpdateEventArgs> trackingUpdateEvent) : IPlaybackProgressService
{
    private readonly string _file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", $"progress.json");
    private Dictionary<string, ProgressInfo> _progress = [];

    public Dictionary<float, ProgressInfo> GetProgress(long id)
    {
        var keys = _progress.Keys.Where(x => x.StartsWith($"{id}_")).ToList();
        var result = new Dictionary<float, ProgressInfo>();
        foreach (var key in keys)
        {
            var parts = key.Split('_');
            if (parts.Length < 2 || !float.TryParse(parts[1], out var episodeNumber))
            {
                continue;
            }

            if (_progress.TryGetValue(key, out var info))
            {
                result[episodeNumber] = info;
            }
        }

        return result;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (File.Exists(_file))
        {
            _progress = JsonSerializer.Deserialize<Dictionary<string, ProgressInfo>>(File.ReadAllText(_file)) ?? [];
        }

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
                        Total = e.Duration.TotalSeconds,
                    };
                }
            });

        trackingUpdateEvent.OnNext()
            .Subscribe(e =>
            {
                var key = $"{e.Anime.Id}_{e.Episode.Number}";
                if (_progress.TryGetValue(key, out var info))
                {
                    info.IsCompleted = true;
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

public class ProgressInfo
{
    public double Position { get; set; }

    public double Total { get; set; }

    [JsonIgnore]
    public bool IsCompleted { get; set; }
}
