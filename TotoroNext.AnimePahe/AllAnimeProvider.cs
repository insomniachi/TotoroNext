using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Flurl.Http;
using FlurlGraphQL;
using TotoroNext.Anime.Abstractions;
using Uno.Logging;

namespace TotoroNext.AnimeHeaven;

internal partial class AllAnimeProvider : IAnimeProvider
{
    public const string Api = "https://api.allanime.day/api";
    public const string BaseUrl = "https://allanime.to/";

    public async IAsyncEnumerable<Episode> GetEpisodes(string animeId)
    {
        var jObject = await Api
            .WithGraphQLQuery(SHOW_QUERY)
            .SetGraphQLVariable("showId", animeId)
            .PostGraphQLQueryAsync()
            .ReceiveGraphQLRawSystemTextJsonResponse();

        var episodeDetails = jObject?["show"]?["availableEpisodesDetail"] as System.Text.Json.Nodes.JsonObject;

        if (episodeDetails is null)
        {
            this.Log().Error("availableEpisodesDetail not found");
            yield break;
        }

        var details = episodeDetails.Deserialize<EpisodeDetails>();

        foreach (var episode in Enumerable.Reverse((details?.sub ?? [])))
        {
            yield return new Episode(this, animeId, episode, float.Parse(episode));
        }
    }

    public async IAsyncEnumerable<VideoServer> GetServersAsync(string animeId, string episodeId)
    {
        var jsonNode = await Api
            .WithGraphQLQuery(EPISODE_QUERY)
            .SetGraphQLVariables(new
            {
                showId = animeId,
                translationType = "sub",
                episodeString = episodeId
            })
            .PostGraphQLQueryAsync()
            .ReceiveGraphQLRawSystemTextJsonResponse();

        if (jsonNode?["errors"] is { })
        {
            this.Log().Warn("Error : " + jsonNode.ToString());
        }

        var sourceArray = jsonNode?["episode"]?["sourceUrls"];
        var sourceObjs = sourceArray?.Deserialize<List<SourceUrlObj>>() ?? [];
        sourceObjs.Sort((x, y) => y.priority.CompareTo(x.priority));

        foreach (var item in sourceObjs)
        {
            if(item.sourceUrl.StartsWith("--"))
            {
                item.sourceUrl = DecryptSourceUrl(item.sourceUrl);
            }

            switch (item.sourceName)
            {
                case "Mp4":
                    if(await VideoServers.FromMp4Upload(item.sourceName, item.sourceUrl) is { } server)
                    {
                        yield return server;
                    }
                    continue;
                case "Yt-mp4":
                    yield return VideoServers.WithReferer(item.sourceName, item.sourceUrl, "https://allanime.day/");
                    continue;
                case "Vg":
                    continue;
                case "Fm-Hls":
                    continue;
                case "Sw":
                    continue;
                case "Ok":
                    continue;
                case "Ss-Hls":
                    continue;
                case "Vid-mp4":
                    continue;
                default:
                    break;
            }

            string? response = "";
            List<ApiV2Reponse> links = [];
            try
            {
                response = await $"https://allanime.day{item.sourceUrl.Replace("clock", "clock.json")}".GetStringAsync();
                var jObject = JsonNode.Parse(response)!.AsObject()!;
                links = jObject["links"].Deserialize<List<ApiV2Reponse>>() ?? [];
            }
            catch
            {
                continue;
            }

            switch (item.sourceName)
            {
                case "Luf-Mp4" or "S-mp4":
                    yield return VideoServers.WithReferer(item.sourceName, links[0].Url, "https://allanime.day/");
                    continue;
                default:
                    break;
            }
        }

        yield break;
    }

    public async IAsyncEnumerable<SearchResult> SearchAsync(string query)
    {
       var jObject = await Api
            .WithGraphQLQuery(SEARCH_QUERY)
            .SetGraphQLVariables(new
            {
                search = new
                {
                    allowAdult = true,
                    allowUnknown = true,
                    query
                },
                limit = 40
            })
            .PostGraphQLQueryAsync()
            .ReceiveGraphQLRawSystemTextJsonResponse();

        foreach (var item in jObject?["shows"]?["edges"]?.AsArray().OfType<JsonObject>() ?? [])
        {
            var title = $"{item?["name"]}";
            var id = $"{item?["_id"]}";
            Uri? image = null;
            try
            {
                image = new Uri($"{item?["thumbnail"]}");
            }
            catch { }

            yield return new SearchResult(this, id, title, image);
        }
    }

    private static string Decrypt(string target) => string.Join("", Convert.FromHexString(target).Select(x => (char)(x ^ 56)));

    private static string DecryptSourceUrl(string sourceUrl)
    {
        var index = sourceUrl.LastIndexOf('-') + 1;
        var encrypted = sourceUrl[index..];
        return Decrypt(encrypted);
    }

    public const string SEARCH_QUERY =
    $$"""
        query( $search: SearchInput
               $limit: Int
               $page: Int
               $translationType: VaildTranslationTypeEnumType
               $countryOrigin: VaildCountryOriginEnumType )
        {
            shows( search: $search
                    limit: $limit
                    page: $page
                    translationType: $translationType
                    countryOrigin: $countryOrigin )
            {
                pageInfo
                {
                    total
                }
                edges 
                {
                    _id,
                    name,
                    availableEpisodesDetail,
                    season,
                    score,
                    thumbnail,
                    malId,
                    aniListId
                }
            }
        }
        """;

    public const string SHOW_QUERY =
    """
        query ($showId: String!) {
            show(
                _id: $showId
            ) {
                availableEpisodesDetail,
                malId,
                aniListId
            }
        }
        """;

    public const string EPISODE_QUERY =
    """
        query ($showId: String!, $translationType: VaildTranslationTypeEnumType!, $episodeString: String!) {
            episode(
                showId: $showId
                translationType: $translationType
                episodeString: $episodeString
            ) {
                episodeString,
                sourceUrls,
                notes
            }
        }
        """;
}


class EpisodeDetails
{
    public List<string> sub { get; set; } = [];
    public List<string> dub { get; set; } = [];
    public List<string> raw { get; set; } = [];
}

[DebuggerDisplay("{priority} - {sourceUrl} - {type}")]
class SourceUrlObj
{

    public string sourceName { get; set; }
    public string sourceUrl { get; set; }
    public double priority { get; set; }
    public string type { get; set; }
}

public class ApiV2Reponse
{
    [JsonPropertyName("src")]
    public string Url { get; set; } = "";

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];
}
