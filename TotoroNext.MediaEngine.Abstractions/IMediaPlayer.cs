namespace TotoroNext.MediaEngine.Abstractions;

public interface IMediaPlayer
{
    void Play(Media media);
    IObservable<TimeSpan> DurationChanged { get; }
    IObservable<TimeSpan> PositionChanged { get; }
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

public record MediaSection(MediaSectionType Type, TimeSpan Start, TimeSpan End);


public record MediaMetadata(string Title, IDictionary<string, string>? Headers = null, IReadOnlyList<MediaSection>? MedaSections = null);
