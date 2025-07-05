
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.UserControls;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Views;

public sealed partial class SearchMetadataProviderPage : Page
{
    public SearchMetadataProviderPage()
    {
        InitializeComponent();
    }

    public SearchMetadataProviderViewModel? ViewModel => DataContext as SearchMetadataProviderViewModel;

    private async void AnimeCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if(this.FindAscendant<SplitView>() is { } sv && sv.IsPaneOpen)
        {
            return;
        }

        if (sender is AnimeCard { Anime: not null } card )
        {
            await (ViewModel?.WatchAnime(card.Anime) ?? Task.CompletedTask);
        }
    }

    private void AnimeCard_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is AnimeCard { Anime: not null } card)
        {
            ViewModel?.PaneNavigator.NavigateToData(card.Anime);
        }
    }

    private void BackgroundTapped(object sender, TappedRoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new ClosePaneMessage());
    }

    private void ItemsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
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
}
