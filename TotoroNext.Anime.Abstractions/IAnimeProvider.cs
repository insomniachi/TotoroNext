namespace TotoroNext.Anime.Abstractions;

public interface IAnimeProvider
{
    IAsyncEnumerable<SearchResult> SearchAsync(string query);
    IAsyncEnumerable<VideoServer> GetServers(string animeId, string episodeId);
    IAsyncEnumerable<Episode> GetEpisodes(string animeId);
}
