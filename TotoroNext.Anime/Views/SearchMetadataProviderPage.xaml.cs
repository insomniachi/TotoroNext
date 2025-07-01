
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class SearchMetadataProviderPage : Page
{
    public SearchMetadataProviderPage()
    {
        InitializeComponent();
    }

    public SearchMetadataProviderViewModel? ViewModel => DataContext as SearchMetadataProviderViewModel;

    private async void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is AnimeModel model)
        {
            await (ViewModel?.AnimeSelected(model) ?? Task.CompletedTask);
        }
    }
}
