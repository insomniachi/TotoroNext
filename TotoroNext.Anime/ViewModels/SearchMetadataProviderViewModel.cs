using System.Reactive.Linq;
using CommunityToolkit.Mvvm.Messaging;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Extensions;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

public partial class SearchMetadataProviderViewModel(IFactory<IMetadataService, Guid> factory,
                                                     IFactory<IAnimeProvider, Guid> providerFactory,
                                                     IMessenger messenger) : ReactiveObject, IInitializable
{
    private readonly IMetadataService? _metadataService = factory.CreateDefault();
    private readonly IAnimeProvider? _provider = providerFactory.CreateDefault();


    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    private IObservable<List<AnimeModel>> ItemsObservable() =>
        this.WhenAnyValue(x => x.Query)
            .Where(_ => _metadataService is not null)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(_metadataService!.SearchAnimeAsync)
            .ObserveOn(RxApp.MainThreadScheduler);

    public void Initialize()
    {
        InitializeOAPH();
    }

    public async Task WatchAnime(AnimeModel model)
    {
        if(_provider is null)
        {
            return;
        }

        if (await _provider.SearchAndSelectAsync(model) is not { } result)
        {
            return;
        }

        messenger.Send(new NavigateToDataMessage(new WatchViewModelNavigationParameter(result, model)));
    }
}
