using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Extensions;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

public partial class SearchMetadataProviderViewModel(IFactory<IMetadataService, Guid> factory,
                                                     IFactory<IAnimeProvider, Guid> providerFactory,
                                                     [FromKeyedServices("Main")] INavigator navigator) : ReactiveObject, IInitializable
{
    private readonly IMetadataService _metadataService = factory.Create(new Guid("b5d31e9b-b988-44e8-8e28-348f58cf1d04"));
    private readonly IAnimeProvider _provider = providerFactory.Create(new Guid("489576c5-2879-493b-874a-7eb14e081280"));


    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    private IObservable<List<AnimeModel>> ItemsObservable() =>
        this.WhenAnyValue(x => x.Query)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(_metadataService.SearchAnimeAsync)
            .ObserveOn(RxApp.MainThreadScheduler);

    public void Initialize()
    {
        InitializeOAPH();
    }

    public async Task AnimeSelected(AnimeModel model)
    {
        if (await _provider.SearchAndSelectAsync(model) is not { } result)
        {
            return;
        }

        navigator.NavigateToData(new WatchViewModelNavigationParameter(result, model));
    }
}
