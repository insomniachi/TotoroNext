using System.Reactive.Linq;
using FlyleafLib.MediaPlayer;
using ReactiveUI;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Flyleaf;

internal class MediaPlayer : IMediaPlayer
{
    private readonly Player _player = new();

    public IObservable<TimeSpan> DurationChanged => _player.WhenAnyValue(x => x.Duration).Select(x => new TimeSpan(x));
    public IObservable<TimeSpan> PositionChanged => _player.WhenAnyValue(x => x.CurTime).Select(x => new TimeSpan(x));

    public void Play(Media media)
    {
        foreach (var item in media.Headers)
        {
            _player.Config.Demuxer.FormatOpt[item.Key] = item.Value;
        }
        _player.Open(media.Uri.ToString());
        _player.Play();
    }

    internal Player GetPlayer() => _player;
}
