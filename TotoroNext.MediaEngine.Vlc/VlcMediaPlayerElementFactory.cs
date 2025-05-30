using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

internal class VlcMediaPlayerElementFactory(IModuleSettings<Settings> settings) : IMediaPlayerElementFactory
{
    public UIElement CreateElement(IMediaPlayer player)
    {
        return null!;
    }

    public IMediaPlayer CreatePlayer() => new VlcMediaPlayer(settings.Value);
}
