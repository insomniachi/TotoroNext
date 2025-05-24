namespace TotoroNext.Anime.Abstractions;

public class SearchResult(IAnimeProvider provider, string id, string title, Uri? image = null)
{
    private readonly IAnimeProvider _provider = provider;

    public string Id { get; } = id;
    public string Title { get; } = title;
    public Uri? Image { get; } = image;

    public IAsyncEnumerable<Episode> GetEpisodes() => _provider.GetEpisodes(Id);
}
