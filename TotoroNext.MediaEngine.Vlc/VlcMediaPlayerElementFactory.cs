using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

internal class VlcMediaPlayerElementFactory : IMediaPlayerElementFactory
{
    public UIElement CreateElement(IMediaPlayer player)
    {
        return null!;
    }

    public IMediaPlayer CreatePlayer() => new VlcMediaPlayer();
}
