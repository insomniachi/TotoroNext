using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using Flurl;
using Flurl.Http;

namespace TotoroNext.MediaEngine.Vlc;

internal class HttpInterface
{
    public const string Password = "totoro";
    private readonly string _api;
    private readonly ReplaySubject<TimeSpan> _durationChanged = new();
    private readonly ReplaySubject<TimeSpan> _timeChanged = new();

    public IObservable<TimeSpan> DurationChanged { get; }
    public IObservable<TimeSpan> PositionChanged { get; }

    public HttpInterface(Process process)
    {
        var host = "127.0.0.1";
        var port = "8080";
        _api = $"http://{host}:{port}";

        Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            .Where(_ => !process.HasExited)
            .SelectMany(_ => GetStatus())
            .Where(s => s is not null)
            .Subscribe(status =>
            {
                _durationChanged.OnNext(TimeSpan.FromSeconds(status!.Length));
                _timeChanged.OnNext(TimeSpan.FromSeconds(status!.Time));
            });

        DurationChanged = _durationChanged.DistinctUntilChanged();
        PositionChanged = _timeChanged.DistinctUntilChanged();
    }


    public async Task<VlcStatus?> GetStatus()
    {
        IFlurlResponse? result = null;
        
        try
        {
            result = await _api
                .AppendPathSegment("/requests/status.json")
                .WithBasicAuth("", Password).GetAsync();
        }
        catch { }

        if(result is null)
        {
            return null;
        }

        if (result.StatusCode >= 300)
        {
            return null;
        }

        return await result.GetJsonAsync<VlcStatus>();
    }

    public async Task SeekTo(TimeSpan timeSpan)
    {
        _ = await _api
             .AppendPathSegment("/requets/status.json")
             .SetQueryParam("command", "seek")
             .SetQueryParam("val", timeSpan.TotalSeconds)
             .WithBasicAuth("", Password)
             .GetAsync();
    }

    public async Task SetVolume(int percent)
    {
        _ = await _api
         .AppendPathSegment("/requets/status.json")
         .SetQueryParam("command", "volume")
         .SetQueryParam("val", $"{percent}%")
         .WithBasicAuth("", Password)
         .GetAsync();
    }
}


internal class VlcStatus
{
    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("information")]
    public Information Information { get; set; } = new Information();
}

internal class Information
{
    [JsonPropertyName("category")]
    public Category Category { get; set; } = new();
}

internal class Category
{
    [JsonPropertyName("meta")]
    public Meta Meta { get; set; } = new();
}

internal class Meta
{
    [JsonPropertyName("filename")]
    public string FileName { get; set; } = string.Empty;
}
