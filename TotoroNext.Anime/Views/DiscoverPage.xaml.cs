using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class DiscoverPage : Page
{
    public DiscoverPage()
    {
        InitializeComponent();

        DataContextChanged += DiscoverPage_DataContextChanged;
    }

    private async void DiscoverPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is DiscoverViewModel vm)
        {

        }
    }
}
