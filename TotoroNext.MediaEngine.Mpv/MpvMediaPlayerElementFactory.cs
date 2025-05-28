using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.MediaEngine.Mpv;

internal class MpvMediaPlayerElementFactory(IModuleSettings<ModuleSettings> settings) : IMediaPlayerElementFactory
{
    public UIElement CreateElement(IMediaPlayer player)
    {
        return null!;
    }

    public IMediaPlayer CreatePlayer() => new MpvMediaPlayer(settings.Value);
}
