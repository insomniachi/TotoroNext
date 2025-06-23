using ReactiveUI;
using TotoroNext.Module;
using TotoroNext.ViewModels;

namespace TotoroNext.Presentation;

public sealed partial class MainPage : Page
{

    public MainPage()
    {
        InitializeComponent();

#if WINDOWS
        //NavFrame.Navigated += (s, e) =>
        //{
        //    TitleBarControl.IsBackButtonVisible = NavFrame.CanGoBack;
        //};
#endif
        Loaded += MainPage_Loaded;
    }

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
                      if (e is { } view)
                      {
                          NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x =>
                          {
                              if(x.Tag is not NavigationViewItemTag tag)
                              {
                                  return false;
                              }

                              return tag.ViewType == view;
                          });
                      }
                  };
                  navigator.NavigateToRoute("My List");
                  NavView.UpdateLayout();

              });
        }
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
