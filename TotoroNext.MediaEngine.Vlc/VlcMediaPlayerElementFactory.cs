using LibVLCSharp.Shared;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

internal class VlcMediaPlayerElementFactory : IMediaPlayerElementFactory
{
    private LibVLC? _libVlc;

    public UIElement CreateElement(IMediaPlayer player)
    {
        if(player is not VlcMediaPlayer p)
        {
            throw new ArgumentException("Player must be of type VlcMediaPlayer", nameof(player));
        }

        var element = new LibVLCSharp.Uno.MediaPlayerElement
        {
            MediaPlayer = p.GetPlayer()
        };

        element.Initialized += (s, e) =>
        {
            _libVlc = new LibVLC(enableDebugLogs: true, e.SwapChainOptions);
        };

        return element;
    }

    public IMediaPlayer CreatePlayer() => new VlcMediaPlayer(_libVlc ?? new LibVLC(enableDebugLogs: true));
}
