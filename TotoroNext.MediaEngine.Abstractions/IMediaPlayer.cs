namespace TotoroNext.MediaEngine.Abstractions;

public interface IMediaPlayer
{
    void Play(Media media);
    IObservable<TimeSpan> DurationChanged { get; }
    IObservable<TimeSpan> PositionChanged { get; }
}


public record Media(string Title, Uri Uri, IDictionary<string, string> Headers);
