using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;
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
        if (SplitView.IsPaneOpen)
        {
            return;
        }

        if (sender is not AnimeCard { Anime: not null } card)
        {
            return;
        }

        await (ViewModel?.NavigateToWatch(card.Anime) ?? Task.CompletedTask);
    }

    private void AnimeCard_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not AnimeCard { Anime: not null } card)
        {
            return;
        }

        ViewModel?.PaneNavigator.NavigateToData(card.Anime);
    }

    private void BackgroundTapped(object sender, TappedRoutedEventArgs e)
    {
        if (SplitView.IsPaneOpen)
        {
            SplitView.IsPaneOpen = false;
            e.Handled = true;
        }
    }

}
