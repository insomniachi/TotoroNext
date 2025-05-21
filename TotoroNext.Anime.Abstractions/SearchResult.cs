namespace TotoroNext.Anime.Abstractions;

public record SearchResult(IAnimeProvider Provider, string Id, string Title, Uri? Image = null);
