namespace TotoroNext.Anime.Abstractions;

public interface IVideoExtractor
{
    IAsyncEnumerable<VideoSource> Extract(Uri url);
}
