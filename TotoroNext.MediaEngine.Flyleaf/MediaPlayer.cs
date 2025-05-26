using FlyleafLib.MediaPlayer;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Flyleaf;

internal class MediaPlayer : IMediaPlayer
{
    private readonly Player _player = new();

    public void Play(Uri uri, IDictionary<string, string> headers)
    {
        foreach (var item in headers)
        {
            _player.Config.Demuxer.FormatOpt[item.Key] = item.Value;
        }
        _player.Open(uri.ToString());
        _player.Play();
    }

    internal Player GetPlayer() => _player;
}
