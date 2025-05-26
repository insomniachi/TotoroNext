using FlyleafLib.Controls.WinUI;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Flyleaf;

public class MediaPlayerElementFactory : IMediaPlayerElementFactory
{
    public UIElement CreateElement(IMediaPlayer player)
    {
        if(player is not MediaPlayer p)
        {
            throw new ArgumentException("wrong media player type");
        }

        var element = new FlyleafHost();
        element.Loaded += (sender, _) =>
        {
            if (sender is FlyleafHost host)
            {
                host.Player = p.GetPlayer();
            }
        };

        return element;
    }

    public IMediaPlayer CreatePlayer() => new MediaPlayer();
}
