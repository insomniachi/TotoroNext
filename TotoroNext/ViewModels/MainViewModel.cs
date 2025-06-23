using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class MainViewModel : ReactiveObject, INavigatorHost
{

    [Reactive]
    public partial string? Name { get; set; }

    public IList<NavigationViewItem> MenuItems { get; }
    public IList<NavigationViewItem> FooterItems { get; }

    [Reactive]
    public partial INavigator? Navigator { get; set; }

    public MainViewModel(IStringLocalizer localizer,
                         IOptions<AppConfig> appInfo,
                         IEnumerable<NavigationViewItem> navigationViewItems,
                         IEvent<NavigateToViewModelRequest> viewNavRequest,
                         IEvent<NavigateToDataRequest> dataNavRequest)
    {
        MenuItems = [.. navigationViewItems.Where(x => x.Tag is NavigationViewItemTag { IsFooterItem: false })];
        FooterItems = [.. navigationViewItems.Where(x => x.Tag is NavigationViewItemTag { IsFooterItem: true })];
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        viewNavRequest.OnNext()
            .Subscribe(req => Navigator?.NavigateViewModel(req.Type));
        dataNavRequest.OnNext()
            .Subscribe(req => Navigator?.NavigateToData(req.Data));
    }

    public string? Title { get; }
}
