using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace TotoroNext.Presentation;

public partial class MainViewModel : ReactiveObject
{
    private INavigator _navigator;

    [Reactive]
    public partial string? Name { get; set; }

    public IList<NavigationViewItem> Items { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator,
        IRouteNotifier routeNotifier,
        IEnumerable<NavigationViewItem> navigationViewItems)
    {
        _navigator = navigator;
        Items = navigationViewItems.ToList();
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        routeNotifier.RouteChanged += RouteNotifier_RouteChanged;
    }

    private void RouteNotifier_RouteChanged(object? sender, RouteChangedEventArgs e)
    {
        var path = e.Navigator?.Route?.Base ?? "";
        if(Items.FirstOrDefault(x => Navigation.GetRequest(x).Replace("./", "") == path) is { } selectedItem)
        {
            selectedItem.IsSelected = true;
        }
    }

    public async Task NavigateToDefault()
    {
        await _navigator.NavigateRouteAsync(this, "./SearchProviderViewModel");
    }

    public string? Title { get; }
}
