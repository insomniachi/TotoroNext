using CommunityToolkit.Mvvm.Messaging;
using ReactiveUI;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using TotoroNext.ViewModels;

namespace TotoroNext.Presentation;

public sealed partial class MainPage : Page
{

    public MainPage()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ClosePaneMessage>(this, (_, _) =>
        {
            MainSplitView.DispatcherQueue.TryEnqueue(() => MainSplitView.IsPaneOpen = false);
        });

#if WINDOWS
        //NavFrame.Navigated += (s, e) =>
        //{
        //    TitleBarControl.IsBackButtonVisible = NavFrame.CanGoBack;
        //};
#endif
        Loaded += MainPage_Loaded;
    }

    public MainViewModel? ViewModel => DataContext as MainViewModel;

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            NavView.ItemInvoked += (_, e) =>
            {
                if(e.IsSettingsInvoked)
                {
                    vm.Navigator?.NavigateViewModel(typeof(SettingsViewModel));
                }
            };

            vm.WhenAnyValue(x => x.Navigator)
              .WhereNotNull()
              .Subscribe(navigator =>
              {
                  navigator.Navigated += (s, e) =>
                  {
                      List<NavigationViewItem> items = [.. (List<NavigationViewItem>)NavView.MenuItemsSource, .. (List<NavigationViewItem>)NavView.FooterMenuItemsSource];

                      var selected = items.FirstOrDefault(x =>
                      {
                          if (x.Tag is not NavigationViewItemTag tag)
                          {
                              return false;
                          }

                          return tag.ViewType == e.ViewType;
                      });
                      NavView.SelectedItem = selected;
                  };
                  navigator.NavigateToRoute("My List");
                  NavView.UpdateLayout();
              });


            vm.PaneNavigator = ActivatorUtilities.CreateInstance<ControlNavigator>(Container.Services, MainSplitView);
        }
    }

    public static SplitViewDisplayMode ConvertDisplayMode(bool isInline) => isInline ? SplitViewDisplayMode.Inline : SplitViewDisplayMode.Overlay;

    private void MainSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
    {
        WeakReferenceMessenger.Default.Send(new PaneClosingMessange());
    }

#if WINDOWS
    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen ^= true;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {

    }
#endif

}
