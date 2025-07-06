using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ReactiveUI;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class MainViewModel : ObservableObject,
                                     INavigatorHost, 
                                     IRecipient<NavigateToViewModelMessage>,
                                     IRecipient<NavigateToDataMessage>,
                                     IRecipient<PaneNavigateToViewModelMessage>,
                                     IRecipient<PaneNavigateToDataMessage>
{
    public const double DefaultPaneLength = 500;
    private readonly List<NavigationViewItem> _allItems;


    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial double? PaneWidth { get; set; } = DefaultPaneLength;

    [ObservableProperty]
    public partial bool IsPaneOpen { get; set; }

    [ObservableProperty]
    public partial bool IsPaneInline { get; set; }

    public Type? CurrentView { get; set; }
   
    public Type? CurrentPaneView { get; set; }

    public IList<NavigationViewItem> MenuItems { get; }
    
    public IList<NavigationViewItem> FooterItems { get; }

    [ObservableProperty]
    public partial INavigator? Navigator { get; set; }

    [ObservableProperty]
    public partial INavigator? PaneNavigator { get; set; }

    public MainViewModel(IStringLocalizer localizer,
                         IOptions<AppConfig> appInfo,
                         IEnumerable<NavigationViewItem> navigationViewItems,
                         IMessenger messenger)
    {
        _allItems = [.. navigationViewItems];
        MenuItems = [.. navigationViewItems.Where(x => x.Tag is NavigationViewItemTag { IsFooterItem: false })];
        FooterItems = [.. navigationViewItems.Where(x => x.Tag is NavigationViewItemTag { IsFooterItem: true })];
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        messenger.Register<NavigateToDataMessage>(this);
        messenger.Register<NavigateToViewModelMessage>(this);
        messenger.Register<PaneNavigateToDataMessage>(this);
        messenger.Register<PaneNavigateToViewModelMessage>(this);

        this.WhenAnyValue(x => x.Navigator)
            .WhereNotNull()
            .FirstAsync()
            .Subscribe(navigator =>
            {
                navigator.Navigated += (_, result) =>
                {
                    CurrentView = result.ViewModelType;
                };
            });

        this.WhenAnyValue(x => x.PaneNavigator)
            .WhereNotNull()
            .FirstAsync()
            .Subscribe(navigator =>
            {
                navigator.Navigated += (_, result) =>
                {
                    CurrentPaneView = result.ViewModelType;
                };
            });
    }

    public void Receive(NavigateToViewModelMessage message)
    {
        Navigator?.NavigateViewModel(message.ViewModel);
    }

    public void Receive(NavigateToDataMessage message)
    {
        Navigator?.NavigateToData(message.Data);
    }

    public void Receive(PaneNavigateToViewModelMessage message)
    {
        RxApp.MainThreadScheduler.Schedule(() =>
        {
            IsPaneInline = message.IsInline;
            IsPaneOpen = true;
            PaneWidth = message.PaneWidth ?? DefaultPaneLength;
            PaneNavigator?.NavigateViewModel(message.ViewModel);
        });
    }

    public void Receive(PaneNavigateToDataMessage message)
    {
        RxApp.MainThreadScheduler.Schedule(() =>
        {
            IsPaneInline = message.IsInline;
            IsPaneOpen = true;
            PaneWidth = message.PaneWidth ?? DefaultPaneLength;
            PaneNavigator?.NavigateToData(message.Data);
        });
    }

    public string? Title { get; }
}
