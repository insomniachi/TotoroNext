using ReactiveUI.Uno;

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
            vm.NavigationFacade.Frame = NavFrame;
            vm.NavigateToDefault();
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
