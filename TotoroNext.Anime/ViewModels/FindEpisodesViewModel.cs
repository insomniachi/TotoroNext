using JetBrains.Annotations;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public class FindEpisodesViewModel(SearchResult result) : ReactiveObject
{
    public async Task Initialize()
    {
        var episodes = await result.Provider.GetEpisodes(result.Id).ToListAsync();
    }
}
