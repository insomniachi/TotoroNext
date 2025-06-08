using TotoroNext.Anime.ViewModels;


namespace TotoroNext.Anime.Views;

public sealed partial class WatchPage : Page
{
    public WatchPage()
    {
        InitializeComponent();
    }

    public WatchViewModel? ViewModel => DataContext as WatchViewModel;
}

