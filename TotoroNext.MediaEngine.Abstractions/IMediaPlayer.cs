namespace TotoroNext.MediaEngine.Abstractions;

public interface IMediaPlayer
{
    void Play(Uri uri, IDictionary<string,string> headers);
}
