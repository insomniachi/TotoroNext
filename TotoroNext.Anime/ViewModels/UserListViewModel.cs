using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Extensions;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

public partial class UserListViewModel : ReactiveObject, IAsyncInitializable, IPaneNavigatable
{
    private readonly ITrackingService? _trackingService;
    private readonly IAnimeProvider? _provider;
    private readonly SourceCache<AnimeModel, long> _animeCache = new(x => x.Id);
    private readonly ReadOnlyObservableCollection<AnimeModel> _anime;
    private readonly IEvent<NavigateToDataRequest> _navigateToData;

    public UserListViewModel(IFactory<ITrackingService, Guid> factory,
                             IFactory<IAnimeProvider, Guid> providerFactory,
                             IEvent<NavigateToDataRequest> navigateToData)
    {
        _trackingService = factory.CreateDefault();
        _provider = providerFactory.CreateDefault();
        _navigateToData = navigateToData;

        _animeCache
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .RefCount()
            .AutoRefresh()
            .Filter(Filter.WhenAnyPropertyChanged().Select(x => (Func<AnimeModel, bool>)x!.IsVisible))
            .Bind(out _anime)
            .DisposeMany()
            .Subscribe();
    }

    public UserListFilter Filter { get; } = new();

    public List<ListItemStatus> AllStatus { get; } = [ListItemStatus.Watching, ListItemStatus.PlanToWatch, ListItemStatus.Completed, ListItemStatus.OnHold];

    public ReadOnlyObservableCollection<AnimeModel> Items => _anime;

    [Reactive]
    public partial bool IsFilterPaneOpen { get; set; }

    public INavigator PaneNavigator { get; set; } = null!;

    public async Task InitializeAsync()
    {
        if(_trackingService is null)
        {
            return;
        }

        var items = await _trackingService.GetUserList();

        _animeCache.Edit(x => x.AddOrUpdate(items));
        Filter.RaisePropertyChanged(nameof(Filter.Status));
    }

    public void AnimeSelected(AnimeModel model)
    {
        PaneNavigator.NavigateToData(model);

        //if (_provider is null)
        //{
        //    return;
        //}

        //if (await _provider.SearchAndSelectAsync(model) is not { } result)
        //{
        //    return;
        //}

        //_navigateToData.Publish(new NavigateToDataRequest(new WatchViewModelNavigationParameter(result, model)));
    }

    [ReactiveCommand]
    private void ToggleFilterPane()
    {
        PaneNavigator.NavigateToData(Filter);
    }

    [ReactiveCommand] 
    private void ClearFilters() => Filter.Clear();
}

