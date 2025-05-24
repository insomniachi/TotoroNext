using System.Reactive.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class SearchProviderViewModel([FromKeyedServices("AllAnime")]IAnimeProvider provider) : ReactiveObject
{
    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    private IObservable<List<SearchResult>> ItemsObservable() => 
        this.WhenAnyValue(x => x.Query)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(query => provider.SearchAsync(query).ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);


    public void Initialize()
    {
        InitializeOAPH();
    }
}
