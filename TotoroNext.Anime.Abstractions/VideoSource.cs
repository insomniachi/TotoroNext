namespace TotoroNext.Anime.Abstractions;

public class VideoSource
{
    public string? Title { get; set; }
    public string? Quality { get; init; }
    public required Uri Url { get; init; }
    public Dictionary<string, string> Headers { get; } = [];
}
