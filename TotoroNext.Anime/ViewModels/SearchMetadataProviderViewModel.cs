using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
                                                     IEvent<NavigateToDataRequest> dataNavRequest) : ObservableObject, IInitializable
{
    private readonly IMetadataService? _metadataService = factory.CreateDefault();
    private readonly IAnimeProvider? _provider = providerFactory.CreateDefault();


    [ObservableProperty]
    public partial string Query { get; set; }

    [ObservableProperty]
    public partial List<AnimeModel> Items { get; set; }

    public void Initialize()
    {
        this.WhenAnyValue(x => x.Query)
            .Where(_ => _metadataService is not null)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(_metadataService!.SearchAnimeAsync)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(items => Items = items);
    }

    [RelayCommand]
    private async Task ItemInvoked(AnimeModel model)
    {
        if(_provider is null)
        {
            return;
        }

        if (await _provider.SearchAndSelectAsync(model) is not { } result)
        {
            return;
        }

        dataNavRequest.Publish(new(new WatchViewModelNavigationParameter(result, model)));
    }
}
