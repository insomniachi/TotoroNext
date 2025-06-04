using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.Anime.ViewModels;

public partial class SearchMetadataProviderViewModel(IFactory<IMetadataService, Guid> factory) : ReactiveObject
{
    private readonly IMetadataService _metadataService = factory.Create(new Guid("b5d31e9b-b988-44e8-8e28-348f58cf1d04"));
    
    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    private IObservable<List<AnimeModel>> ItemsObservable() =>
        this.WhenAnyValue(x => x.Query)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(query => _metadataService.SearchAnimeAsync(query))
            .ObserveOn(RxApp.MainThreadScheduler);

    public void Initialize()
    {
        InitializeOAPH();
    }
}
