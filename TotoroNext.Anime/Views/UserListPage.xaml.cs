using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class UserListPage : Page
{
    public UserListPage()
    {
        InitializeComponent();
    }

    public UserListViewModel? ViewModel => DataContext as UserListViewModel;

    private async void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is AnimeModel model)
        {
            await (ViewModel?.AnimeSelected(model) ?? Task.CompletedTask);
        }
    }
}
