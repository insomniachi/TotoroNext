using Flurl;
using Flurl.Http;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using TotoroNext.Anime.Abstractions;

namespace TotoroNext.AnimeHeaven;

internal class AnimeHeavenProvider(IHttpClientFactory httpClientFactory) : IAnimeProvider
{
    public async IAsyncEnumerable<SearchResult> SearchAsync(string query)
    {
        using var client = GetClient();

        var response = await client.Request("fastsearch.php")
            .SetQueryParams(new
            {
                xhr = 1,
                s = query
            })
            .GetStreamAsync();

        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(response);

        var items = htmlDoc.DocumentNode.SelectNodes("a");
        if (items is null)
        {
            yield break;
        }

        foreach (var node in items)
        {
            var name = node.QuerySelector(".fastname").InnerText;
            var image = Url.Combine(client.BaseUrl, node.QuerySelector("img").GetAttributeValue("src", ""));
            var id = node.GetAttributeValue("href", "").Split("?").Last();

            yield return new SearchResult(this, id, name, new Uri(image));
        }
    }

    public async IAsyncEnumerable<Episode> GetEpisodes(string animeId)
    {
        using var client = GetClient();

        var response = await client.Request($"/anime.php?{animeId}").GetStreamAsync();

        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(response);

        var items = htmlDoc.DocumentNode.QuerySelectorAll(".ac3") ?? [];

        foreach (var item in items.Reverse())
        {
            var id = item.GetAttributeValue("href", "").Split("?").Last();
            var number = float.Parse(item.QuerySelector(".watch2 .bc").InnerHtml);

            yield return new Episode(this, animeId, id, number);
        }
    }

    public async IAsyncEnumerable<VideoServer> GetServersAsync(string animeId, string episodeId)
    {
        using var client = GetClient();

        yield return new VideoServer("AnimeHeaven", new Uri(Url.Combine(client.BaseUrl, $"/episode.php?{episodeId}")));

        await Task.CompletedTask;
    }

    private FlurlClient GetClient() => new(httpClientFactory.CreateClient("AnimeHeaven"));
}
