using System.Reactive.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class SearchProviderViewModel(IFactory<IAnimeProvider, Guid> factory,
                                             [FromKeyedServices("Main")]INavigator navigator) : ReactiveObject
{
    private readonly IAnimeProvider _provider = factory.Create(new Guid("489576c5-2879-493b-874a-7eb14e081280"));

    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    private IObservable<List<SearchResult>> ItemsObservable() => 
        this.WhenAnyValue(x => x.Query)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(query => _provider.SearchAsync(query).ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);


    public void Initialize()
    {
        InitializeOAPH();
    }

    public void NavigateToWatch(SearchResult result)
    {
        navigator.NavigateToData(result);
    }
}
