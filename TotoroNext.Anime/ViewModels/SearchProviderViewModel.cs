using System.Reactive.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class SearchProviderViewModel(IFactory<IAnimeProvider, Guid> factory,
                                             IFactory<IMetadataService, Guid> metaDataFactory,
                                             [FromKeyedServices("Main")]INavigator navigator) : ReactiveObject, IInitializable
{
    private readonly IAnimeProvider _provider = factory.Create(new Guid("489576c5-2879-493b-874a-7eb14e081280"));
    private readonly IMetadataService _metadataService = metaDataFactory.Create(new Guid("b5d31e9b-b988-44e8-8e28-348f58cf1d04"));

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
        navigator.NavigateToData(new WatchViewModelNavigationParameter(result));
    }
}
