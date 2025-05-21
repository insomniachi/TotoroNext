using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime.ViewModels;

internal partial class DiscoverViewModel([FromKeyedServices("AnimeHeaven")]IAnimeProvider provider) : ReactiveObject
{
    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    public IObservable<List<SearchResult>> ItemsObservable() => this.WhenAnyValue(x => x.Query)
        .Where(query => query is { Length: > 3 })
        .Throttle(TimeSpan.FromMilliseconds(500))
        .SelectMany(async query => await provider.SearchAsync(query).ToListAsync());


    public void Initialize()
    {
        InitializeOAPH();
    }
}
