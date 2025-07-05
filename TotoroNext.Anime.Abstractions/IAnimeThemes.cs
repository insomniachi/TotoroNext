namespace TotoroNext.Anime.Abstractions;

public interface IAnimeThemes
{
    Task<List<AnimeTheme>> FindById(long id, string serviceName);
}

public class AnimeTheme
{
    public Uri? Video { get; init; }
    public Uri? Audio { get; init; }
    public AnimeThemeType Type { get; init; }
    public int Number { get; init; }
    public string SongName { get; init; } = "";
    public string Artist { get; init; } = "";
    public string DisplayName => $"({Type}{Number}) - {SongName} by {Artist}";
}

public enum AnimeThemeType
{
    OP,
    ED
}
