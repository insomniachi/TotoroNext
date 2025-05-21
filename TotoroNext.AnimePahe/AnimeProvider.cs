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
            var url = Url.Combine(client.BaseUrl, node.GetAttributeValue("href", ""));

            yield return new SearchResult
            {
                Title = name,
                Image = new Uri(image),
                Id = url
            };
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
            yield return new Episode
            {
                Id = item.GetAttributeValue("href", "").Split("?").Last(),
                Number = float.Parse(item.QuerySelector(".watch2 .bc").InnerHtml)
            };
        }
    }

    public async IAsyncEnumerable<VideoServer> GetServers(Uri uri, string episodeId)
    {
        yield return new VideoServer
        {
            Name = "AnimeHeaven",
            Url = new Uri(Url.Combine(uri.ToString(), $"/episode.php?{episodeId}")),
        };

        await Task.CompletedTask;
    }

    private FlurlClient GetClient() => new(httpClientFactory.CreateClient("AnimeHeaven"));
}
