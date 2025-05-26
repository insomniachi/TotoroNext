using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace TotoroNext.Presentation;

public partial class MainViewModel : ReactiveObject
{
    private INavigator _navigator;

    [Reactive]
    public partial string? Name { get; set; }

    public IEnumerable<NavigationViewItem> Items { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator,
        IEnumerable<NavigationViewItem> navigationViewItems)
    {
        _navigator = navigator;
        Items = navigationViewItems;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public async Task NavigateToDefault()
    {
        await _navigator.NavigateRouteAsync(this, "./SearchProviderViewModel");
    }

    public string? Title { get; }
}
