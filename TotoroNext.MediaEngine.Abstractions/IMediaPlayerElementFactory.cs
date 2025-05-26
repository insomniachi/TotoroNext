namespace TotoroNext.MediaEngine.Abstractions;

public interface IMediaPlayerElementFactory
{
    UIElement CreateElement(IMediaPlayer player);
    IMediaPlayer CreatePlayer();
}
