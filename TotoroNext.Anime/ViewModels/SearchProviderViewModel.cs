using System.Reactive.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.Extensions;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class SearchProviderViewModel(IFactory<IAnimeProvider, Guid> factory,
                                             IFactory<IMetadataService, Guid> metaDataFactory,
                                             [FromKeyedServices("Main")] INavigator navigator) : ReactiveObject, IInitializable
{
    private readonly IAnimeProvider? _provider = factory.CreateDefault();
    private readonly IMetadataService? _metadataService = metaDataFactory.CreateDefault();

    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    private IObservable<List<SearchResult>> ItemsObservable() =>
        this.WhenAnyValue(x => x.Query)
            .Where(_ => _provider is not null)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(query => _provider!.SearchAsync(query).ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);


    public void Initialize()
    {
        InitializeOAPH();
    }

    public async Task NavigateToWatch(SearchResult result)
    {
        if(_metadataService is null)
        {
            navigator.NavigateToData(new WatchViewModelNavigationParameter(result));
        }
        else
        {
            var anime = await _metadataService.SearchAndSelectAsync(result);
            navigator.NavigateToData(new WatchViewModelNavigationParameter(result, anime));
        }
    }
}
