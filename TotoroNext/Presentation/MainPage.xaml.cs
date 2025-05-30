namespace TotoroNext.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        DataContextChanged += MainPage_DataContextChanged;
    }

    private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is MainViewModel vm)
        {
            vm.NavigationFacade.Frame = NavFrame;
            vm.NavigateToDefault();
        }
    }
}
