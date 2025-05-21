namespace TotoroNext.Anime.Abstractions;

public class Episode
{
    public required string Id { get; init; }
    public string? Name { get; init; }
    public float Number { get; init; }
    public Uri? Image { get; init; }
}
