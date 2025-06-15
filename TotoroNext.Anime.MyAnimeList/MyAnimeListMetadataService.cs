using MalApi;
using MalApi.Interfaces;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.MyAnimeList.Views;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.MyAnimeList;

internal class MyAnimeListMetadataService : IMetadataService
{
    private readonly string[] _commonFields =
    [
        AnimeFieldNames.Synopsis,
        AnimeFieldNames.TotalEpisodes,
        AnimeFieldNames.Broadcast,
        AnimeFieldNames.UserStatus,
        AnimeFieldNames.NumberOfUsers,
        AnimeFieldNames.Rank,
        AnimeFieldNames.Mean,
        AnimeFieldNames.AlternativeTitles,
        AnimeFieldNames.Popularity,
        AnimeFieldNames.StartSeason,
        AnimeFieldNames.Genres,
        AnimeFieldNames.Status,
        AnimeFieldNames.Videos,
        AnimeFieldNames.StartDate,
        AnimeFieldNames.MediaType
    ];

    private readonly string _recursiveAnimeProperties = $"my_list_status,status,{AnimeFieldNames.TotalEpisodes},{AnimeFieldNames.Mean}";
    private readonly IMalClient _client;
    private readonly Settings _settings;

    public MyAnimeListMetadataService(IMalClient client, IModuleSettings<Settings> settings)
    {
        client.SetClientId(SettingsPage.ClientId);

        if (settings.Value.Auth is { } token)
        {
            client.SetAccessToken(token.AccessToken);
        }

        _client = client;
        _settings = settings.Value;

    }

    public async Task<List<AnimeModel>> GetAiringAnimeAsync()
    {
        var request = _client
            .Anime()
            .Top(AnimeRankingType.Airing)
            .WithLimit(15)
            .WithFields(_commonFields);

        if (_settings.IncludeNsfw)
        {
            request.IncludeNsfw();
        }

        var result = await request.Find();

        return [.. result.Data.Select(x => MalToModelConverter.ConvertModel(x.Anime))];
    }

    public async Task<AnimeModel> GetAnimeAsync(long id)
    {
        var malModel = await _client.Anime().WithId(id)
                                   .WithFields(_commonFields)
                                   .WithField(x => x.Genres)
                                   .WithFields($"related_anime{{{_recursiveAnimeProperties}}}")
                                   .WithFields($"recommendations{{{_recursiveAnimeProperties}}}")
                                   .Find();

        return MalToModelConverter.ConvertModel(malModel);
    }

    public async Task<List<AnimeModel>> GetSeasonalAnimeAsync()
    {
        IGetSeasonalAnimeListRequest baseRequest(Abstractions.Models.Season season)
        {
            var request = _client.Anime()
                                .OfSeason((AnimeSeason)(int)season.SeasonName, season.Year)
                                .WithFields(_commonFields);

            if (_settings.IncludeNsfw)
            {
                request.IncludeNsfw();
            }

            return request;
        }

        var current = AnimeHelpers.CurrentSeason();
        var prev = AnimeHelpers.PrevSeason();
        var next = AnimeHelpers.NextSeason();

        var response = new List<AnimeModel>();

        foreach (var season in new[] { current, prev, next })
        {
            var pagedAnime = await baseRequest(season).Find();

            response.AddRange(pagedAnime.Data.Select(MalToModelConverter.ConvertModel));
        }

        return response;
    }

    public async Task<List<AnimeModel>> SearchAnimeAsync(string term)
    {
        var request = _client
            .Anime()
            .WithName(term)
            .WithFields(_commonFields)
            .WithLimit(5);

        if (_settings.IncludeNsfw)
        {
            request.IncludeNsfw();
        }

        var result = await request.Find();

        return [.. result.Data.Select(MalToModelConverter.ConvertModel)];
    }
}
