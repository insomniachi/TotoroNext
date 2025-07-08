using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using TotoroNext.Anime.UserControls;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Module.Abstractions;

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
        if (args.Element is not ItemContainer container)
        {
            return;
        }

        if (container.Child is not AnimeCard card)
        {
            return;
        }

        card.UpdateBindings();
    }

    private async void AnimeCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (this.FindAscendant<SplitView>() is { } sv && sv.IsPaneOpen)
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

        ViewModel?.OpenAnimeDetails(card.Anime);
    }

    private void BackgroundTapped(object sender, TappedRoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new ClosePaneMessage());
    }

}
