using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class SearchProviderPage : Page
{
    public SearchProviderPage()
    {
        InitializeComponent();
    }

    public SearchProviderViewModel? ViewModel => DataContext as SearchProviderViewModel;

    private async void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is not SearchResult result)
        {
            return;
        }

        await (ViewModel?.NavigateToWatch(result) ?? Task.CompletedTask);
    }
}
