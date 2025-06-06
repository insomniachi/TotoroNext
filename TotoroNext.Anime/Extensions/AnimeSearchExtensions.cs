using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Extensions;

internal static class AnimeSearchExtensions
{
    internal static async Task<SearchResult?> SearchAndSelectAsync(this IAnimeProvider provider, AnimeModel model)
    {
        var results = await provider.SearchAsync(model.Title).ToListAsync();

        if(results.Count == 0)
        {
            return null;
        }

        if (results.FirstOrDefault(x => string.Equals(x.Title, model.Title, StringComparison.OrdinalIgnoreCase)) is { } result)
        {
            return result;
        }
        else
        {
            return await Container.Services.GetRequiredService<IUserInteraction<List<SearchResult>, SearchResult>>().GetValue(results);
        }
    }

    internal static async Task<AnimeModel?> SearchAndSelectAsync(this IMetadataService provider, SearchResult model)
    {
        var results = await provider.SearchAnimeAsync(model.Title);

        if (results.Count == 0)
        {
            return null;
        }

        if (results.FirstOrDefault(x => string.Equals(x.Title, model.Title, StringComparison.OrdinalIgnoreCase)) is { } result)
        {
            return result;
        }
        else
        {
            return await Container.Services.GetRequiredService<IUserInteraction<List<AnimeModel>, AnimeModel>>().GetValue(results);
        }
    }
}
