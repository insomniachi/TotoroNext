

using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class AnimeDetailsView : UserControl
{
    public AnimeDetailsView()
    {
        InitializeComponent();
    }

    public AnimeDetailsViewModel? ViewModel => DataContext as AnimeDetailsViewModel;

    private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {

    }

    private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if(args.InvokedItem is EpisodeInfo { } ep && ViewModel is { } vm)
        {
            vm.WatchEpisodeCommand.Execute(ep);
        }
    }
}
