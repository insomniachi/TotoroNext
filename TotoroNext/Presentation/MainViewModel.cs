using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class MainViewModel : ReactiveObject
{

    [Reactive]
    public partial string? Name { get; set; }

    public IList<NavigationViewItem> Items { get; }

    public IFrameNavigator NavigationFacade { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        [FromKeyedServices("Main")]IFrameNavigator navigationFacade,
        IEnumerable<NavigationViewItem> navigationViewItems)
    {
        NavigationFacade = navigationFacade;

        Items = [.. navigationViewItems];
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public void NavigateToDefault()
    {
        NavigationFacade.NavigateToRoute("Search");
    }

    public string? Title { get; }
}
