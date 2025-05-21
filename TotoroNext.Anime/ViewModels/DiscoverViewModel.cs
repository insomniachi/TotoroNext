using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime.ViewModels;

internal partial class DiscoverViewModel([FromKeyedServices("AnimeHeaven")]IAnimeProvider provider) : ObservableObject
{
    [ObservableProperty]
    public partial List<SearchResult> Items { get; set; }

    public async Task Initialize()
    {
        Items = await provider.SearchAsync("Hyouka").ToListAsync();
    }
}
