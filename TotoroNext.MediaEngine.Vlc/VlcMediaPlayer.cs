using LibVLCSharp.Shared;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

internal class VlcMediaPlayer(LibVLC libVLC) : IMediaPlayer
{
    private readonly MediaPlayer _mediaPlayer = new(libVLC);

    public void Play(Uri uri, IDictionary<string, string> headers)
    {
        var media = new Media(libVLC, uri);
        foreach (var header in headers)
        {
            media.AddOption($":http-header={header.Key}:{header.Value}");
        }
        _mediaPlayer.Play();
    }

    internal MediaPlayer GetPlayer() => _mediaPlayer;
}
