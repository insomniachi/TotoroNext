using TotoroNext.Anime.UserControls;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class UserListPage : Page
{
    public UserListPage()
    {
        InitializeComponent();
    }

    public UserListViewModel? ViewModel => DataContext as UserListViewModel;

    private void AnimeList_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if(args.Element is not ItemContainer container)
        {
            return;
        }

        if(container.Child is not AnimeCard card)
        {
            return;
        }

        card.UpdateBindings();
    }

    private async void AnimeCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if(sender is not AnimeCard { Anime: not null } card) 
        {
            return;
        }

        await (ViewModel?.AnimeSelected(card.Anime) ?? Task.CompletedTask);
    }
}
