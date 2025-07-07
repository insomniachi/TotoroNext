using System.Reactive;
using Windows.Media.Playback;

namespace TotoroNext.MediaEngine.Abstractions;

public interface IMediaPlayer
{
    void Play(Media media, TimeSpan startPosition);
    IObservable<TimeSpan> DurationChanged { get; }
    IObservable<TimeSpan> PositionChanged { get; }
    IObservable<Unit> PlaybackStopped { get; }
}

public interface IInternalMediaPlayer : IMediaPlayer
{
    MediaPlayer MediaPlayer { get; }
}


public record Media(Uri Uri, MediaMetadata Metadata);


public enum MediaSectionType
{
    Recap,
    Opening,
    Content,
    Ending,
    Preview,
}

public record MediaSegment(MediaSectionType Type, TimeSpan Start, TimeSpan End);


public record MediaMetadata(string Title, IDictionary<string, string>? Headers = null, IReadOnlyList<MediaSegment>? MedaSections = null);
