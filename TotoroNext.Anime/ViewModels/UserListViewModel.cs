using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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

public partial class UserListViewModel(IFactory<ITrackingService, Guid> factory,
                         IFactory<IAnimeProvider, Guid> providerFactory,
                         IAnimeOverridesRepository animeOverridesRepository,
                         IMessenger messenger) : ObservableObject, IAsyncInitializable, IPaneNavigatable
{
    private readonly ITrackingService? _trackingService = factory.CreateDefault();
    private readonly IMessenger _messenger = messenger;
    private IEnumerable<AnimeModel>? _allItems;

    public UserListFilter Filter { get; } = new();

    public List<ListItemStatus> AllStatus { get; } = [ListItemStatus.Watching, ListItemStatus.PlanToWatch, ListItemStatus.Completed, ListItemStatus.OnHold];

    [ObservableProperty]
    public partial List<AnimeModel> Items { get; set; } = [];

    public INavigator PaneNavigator { get; set; } = null!;

    public async Task InitializeAsync()
    {
        if(_trackingService is null)
        {
            return;
        }

        _allItems = await _trackingService.GetUserList();
        Items = [.. _allItems];

        Filter.WhenAnyPropertyChanged().Subscribe(x =>
        {
            Items = [.. _allItems.Where(Filter.IsVisible)];
        });

        Filter.RaisePropertyChanged(nameof(Filter.Status));
    }

    public async Task NavigateToWatch(AnimeModel anime)
    {
        var overrides = animeOverridesRepository.GetOverrides(anime.Id);
        
        var provider = overrides?.Provider is { } providerId
            ? providerFactory.Create(providerId)
            : providerFactory.CreateDefault();

        if(provider is null)
        {
            return;
        }

        var result = await provider.SearchAndSelectAsync(anime);

        if (result is null)
        {
            return;
        }

        _messenger.Send(new NavigateToDataMessage(new WatchViewModelNavigationParameter(result, anime)));
    }

    [RelayCommand]
    private void ToggleFilterPane()
    {
        PaneNavigator.NavigateToData(Filter);
    }

    [RelayCommand] 
    private void ClearFilters() => Filter.Clear();
}

