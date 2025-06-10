using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
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

public partial class UserListViewModel : ReactiveObject, IAsyncInitializable
{
    private readonly ITrackingService? _trackingService;
    private readonly IAnimeProvider? _provider;
    private readonly INavigator _navigator;
    private readonly SourceCache<AnimeModel, long> _animeCache = new(x => x.Id);
    private readonly ReadOnlyObservableCollection<AnimeModel> _anime;

    public UserListViewModel(IFactory<ITrackingService, Guid> factory,
                             IFactory<IAnimeProvider, Guid> providerFactory,
                             [FromKeyedServices("Main")] INavigator navigator)
    {
        _trackingService = factory.CreateDefault();
        _provider = providerFactory.CreateDefault();
        _navigator = navigator;

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

    public Filter Filter { get; } = new();

    public List<ListItemStatus> AllStatus { get; } = [ListItemStatus.Watching, ListItemStatus.PlanToWatch, ListItemStatus.Completed, ListItemStatus.OnHold];

    public ReadOnlyObservableCollection<AnimeModel> Items => _anime;

    [Reactive]
    public partial bool IsFilterPaneOpen { get; set; }

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

    public async Task AnimeSelected(AnimeModel model)
    {
        if (_provider is null)
        {
            return;
        }

        if (await _provider.SearchAndSelectAsync(model) is not { } result)
        {
            return;
        }

        _navigator.NavigateToData(new WatchViewModelNavigationParameter(result, model));
    }

    [ReactiveCommand]
    private void ToggleFilterPane() => IsFilterPaneOpen ^= true;

    [ReactiveCommand] 
    private void ClearFilters() => Filter.Clear();
}


public partial class Filter : ReactiveObject
{
    public Filter()
    {
        Status = ListItemStatus.Watching;
        Term = "";
        Year = "";
    }

    [Reactive]
    public partial ListItemStatus Status { get; set; }

    [Reactive]
    public partial string Term { get; set; }

    [Reactive]
    public partial string Year { get; set; }

    public bool IsVisible(AnimeModel model)
    {
        if (model.Tracking is null)
        {
            return true;
        }

        var listStatusCheck = Status == ListItemStatus.Watching
            ? model.Tracking.Status is ListItemStatus.Watching or ListItemStatus.Rewatching
            : model.Tracking.Status == Status;

        var searchTextStatus = string.IsNullOrEmpty(Term) ||
                               model.Title.Contains(Term, StringComparison.InvariantCultureIgnoreCase);

        //var searchTextStatus = string.IsNullOrEmpty(Term) ||
        //                       model.Title.Contains(Term, StringComparison.InvariantCultureIgnoreCase) ||
        //                       model.AlternativeTitles.Any(x => x.Contains(Term, StringComparison.InvariantCultureIgnoreCase));
        var yearCheck = string.IsNullOrEmpty(Year) || !YearRegex().IsMatch(Year) || model.Season?.Year.ToString() == Year;
        //var genresCheck = !Genres.Any() || Genres.All(x => model.Genres.Any(y => string.Equals(y, x, StringComparison.InvariantCultureIgnoreCase)));
        //var airingStatusCheck = AiringStatus is null || AiringStatus == model.AiringStatus;

        var isVisible = listStatusCheck && searchTextStatus && yearCheck /* && genresCheck && airingStatusCheck*/;
        return isVisible;
    }

    public void Clear()
    {
        Term = "";
        Year = "";
    }

    [GeneratedRegex(@"(19[5-9][0-9])|(20\d{2})")]
    private partial Regex YearRegex();
}

