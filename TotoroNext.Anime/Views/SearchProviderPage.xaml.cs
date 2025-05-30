using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class SearchProviderPage : Page
{
    public SearchProviderPage()
    {
        InitializeComponent();

        DataContextChanged += DiscoverPage_DataContextChanged;
    }

    private void DiscoverPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is SearchProviderViewModel vm)
        {
            vm.Initialize();
        }
    }

    public SearchProviderViewModel? ViewModel => DataContext as SearchProviderViewModel;

    private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if(args.InvokedItem is not SearchResult result)
        {
            return;
        }

        ViewModel?.NavigateToWatch(result);
    }
}
