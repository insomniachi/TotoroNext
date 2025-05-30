namespace TotoroNext.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        DataContextChanged += MainPage_DataContextChanged;

#if WINDOWS10_0_26100_0_OR_GREATER
        NavFrame.Navigated += (s, e) =>
        {
            TitleBarControl.IsBackButtonVisible = NavFrame.CanGoBack;
        };
#endif
    }

    private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is MainViewModel vm)
        {
            vm.NavigationFacade.Frame = NavFrame;
            vm.NavigateToDefault();
        }
    }

#if WINDOWS10_0_26100_0_OR_GREATER
    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen ^= true;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        if(!NavFrame.CanGoBack)
        {
            return;
        }

        NavFrame.GoBack(); 
    }
#endif

}
