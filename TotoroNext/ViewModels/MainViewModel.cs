using System.Reactive.Linq;
using CommunityToolkit.Mvvm.Messaging;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class MainViewModel : ReactiveObject,
                                     INavigatorHost, 
                                     IRecipient<NavigateToViewModelMessage>,
                                     IRecipient<NavigateToDataMessage>
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
                         IMessenger messenger)
    {
        MenuItems = [.. navigationViewItems.Where(x => x.Tag is NavigationViewItemTag { IsFooterItem: false })];
        FooterItems = [.. navigationViewItems.Where(x => x.Tag is NavigationViewItemTag { IsFooterItem: true })];
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        messenger.Register<NavigateToDataMessage>(this);
        messenger.Register<NavigateToViewModelMessage>(this);

        this.WhenAnyValue(x => x.Navigator)
            .WhereNotNull()
            .FirstAsync()
            .Subscribe(navigator =>
            {
                navigator.Navigated += (_, type) =>
                {
                    UpdateSelections(navigationViewItems, type);
                };
            });
    }

    private static void UpdateSelections(IEnumerable<NavigationViewItem> items, Type viewType)
    {
        foreach (var item in items)
        {
            if(item.Tag is not NavigationViewItemTag tag)
            {
                return;
            }

            item.DispatcherQueue.TryEnqueue(() =>
            {
                item.IsSelected = tag.ViewType == viewType;
            });
        }
    }

    public void Receive(NavigateToViewModelMessage message)
    {
        Navigator?.NavigateViewModel(message.ViewModel);
    }

    public void Receive(NavigateToDataMessage message)
    {
        Navigator?.NavigateToData(message.Data);
    }

    public string? Title { get; }
}
