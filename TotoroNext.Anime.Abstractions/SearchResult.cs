namespace TotoroNext.Anime.Abstractions;

public class SearchResult
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public Uri? Image { get; init; }
}
