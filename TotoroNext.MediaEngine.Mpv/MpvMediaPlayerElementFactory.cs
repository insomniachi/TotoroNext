using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Mpv;

internal class MpvMediaPlayerElementFactory : IMediaPlayerElementFactory
{
    public UIElement CreateElement(IMediaPlayer player)
    {
        return null!;
    }

    public IMediaPlayer CreatePlayer() => new MpvMediaPlayer();
}
