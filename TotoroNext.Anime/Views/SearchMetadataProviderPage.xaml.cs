
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class SearchMetadataProviderPage : Page
{
    public SearchMetadataProviderPage()
    {
        InitializeComponent();

        DataContextChanged += SearchMetadataProviderPage_DataContextChanged;
    }

    private void SearchMetadataProviderPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is SearchMetadataProviderViewModel vm)
        {
            vm.Initialize();
        }
    }

    public SearchMetadataProviderViewModel? ViewModel => DataContext as SearchMetadataProviderViewModel;

    private async void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if(args.InvokedItem is AnimeModel model)
        {
            await (ViewModel?.AnimeSelected(model) ?? Task.CompletedTask);
        }
    }
}
