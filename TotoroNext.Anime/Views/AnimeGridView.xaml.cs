using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class AnimeGridView : UserControl
{
    public AnimeGridView()
    {
        InitializeComponent();
    }

    public AnimeGridViewModel? ViewModel => DataContext as AnimeGridViewModel;
}
