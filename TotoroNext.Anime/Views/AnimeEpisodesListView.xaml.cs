using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class AnimeEpisodesListView : UserControl
{
    public AnimeEpisodesListView()
    {
        InitializeComponent();
    }

    public AnimeEpisodesListViewModel? ViewModel => DataContext as AnimeEpisodesListViewModel;
}
