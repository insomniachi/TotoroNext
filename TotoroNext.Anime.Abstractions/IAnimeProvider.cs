namespace TotoroNext.Anime.Abstractions;

public interface IAnimeProvider
{
    IAsyncEnumerable<SearchResult> SearchAsync(string query);
    IAsyncEnumerable<VideoServer> GetServers(Uri uri, string episodeId);
    IAsyncEnumerable<Episode> GetEpisodes(string animeId);
}
