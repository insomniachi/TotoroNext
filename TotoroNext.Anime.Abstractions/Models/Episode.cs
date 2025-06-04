namespace TotoroNext.Anime.Abstractions.Models;

public class Episode(IAnimeProvider provider, string showId, string id, float number, string name = "", Uri? image = null)
{
    private readonly IAnimeProvider _provider = provider;

    public string ShowId { get; } = showId;
    public string Id { get; } = id;
    public float Number { get; } = number;
    public string Name { get; } = name;
    public Uri? Image { get; } = image;

    public IAsyncEnumerable<VideoServer> GetServersAsync() => _provider.GetServersAsync(ShowId, Id);
}
