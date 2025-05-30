using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class MainViewModel : ReactiveObject
{

    [Reactive]
    public partial string? Name { get; set; }

    public IList<NavigationViewItem> MenuItems { get; }
    public IList<NavigationViewItem> FooterItems { get; }

    public IFrameNavigator NavigationFacade { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        [FromKeyedServices("Main")]IFrameNavigator navigationFacade,
        IEnumerable<NavigationViewItem> navigationViewItems)
    {
        NavigationFacade = navigationFacade;

        MenuItems = [.. navigationViewItems.Where(x => x.Tag is false)];
        FooterItems = [.. navigationViewItems.Where(x => x.Tag is true)];
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
