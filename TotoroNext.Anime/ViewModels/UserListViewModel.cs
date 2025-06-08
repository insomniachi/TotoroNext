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

public partial class UserListViewModel : ReactiveObject, IAsyncInitializable
{
    private readonly ITrackingService _trackingService;
    private readonly IAnimeProvider _provider;
    private readonly INavigator _navigator;
    private readonly SourceCache<AnimeModel, long> _animeCache = new(x => x.Id);
    private readonly ReadOnlyObservableCollection<AnimeModel> _anime;

    public UserListViewModel(IFactory<ITrackingService, Guid> factory,
                             IFactory<IAnimeProvider, Guid> providerFactory,
                             [FromKeyedServices("Main")] INavigator navigator)
    {
        _trackingService = factory.Create(new Guid("b5d31e9b-b988-44e8-8e28-348f58cf1d04"));
        _provider = providerFactory.Create(new Guid("489576c5-2879-493b-874a-7eb14e081280"));
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

    public async Task InitializeAsync()
    {
        var items = await _trackingService.GetUserList();

        _animeCache.Edit(x => x.AddOrUpdate(items));
        Filter.RaisePropertyChanged(nameof(Filter.Status));
    }

    public async Task AnimeSelected(AnimeModel model)
    {
        if (await _provider.SearchAndSelectAsync(model) is not { } result)
        {
            return;
        }

        _navigator.NavigateToData(new WatchViewModelNavigationParameter(result, model));
    }
}


public partial class Filter : ReactiveObject
{
    public Filter()
    {
        Status = ListItemStatus.Watching;
    }

    [Reactive]
    public partial ListItemStatus Status { get; set; }

    public bool IsVisible(AnimeModel model)
    {
        if (model.Tracking is null)
        {
            return true;
        }


        var listStatusCheck = Status == ListItemStatus.Watching
            ? model.Tracking.Status is ListItemStatus.Watching or ListItemStatus.Rewatching
            : model.Tracking.Status == Status;

        //var searchTextStatus = string.IsNullOrEmpty(SearchText) ||
        //                       model.Title.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase) ||
        //                       model.AlternativeTitles.Any(x => x.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));
        //var yearCheck = string.IsNullOrEmpty(Year) || !YearRegex().IsMatch(Year) || model.Season.Year.ToString() == Year;
        //var genresCheck = !Genres.Any() || Genres.All(x => model.Genres.Any(y => string.Equals(y, x, StringComparison.InvariantCultureIgnoreCase)));
        //var airingStatusCheck = AiringStatus is null || AiringStatus == model.AiringStatus;

        //var isVisible = listStatusCheck && searchTextStatus && yearCheck && genresCheck && airingStatusCheck;
        return listStatusCheck;
    }
}

