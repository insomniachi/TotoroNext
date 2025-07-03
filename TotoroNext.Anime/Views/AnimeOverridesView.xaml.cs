using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;
public sealed partial class AnimeOverridesView : UserControl
{
    public AnimeOverridesView()
    {
        InitializeComponent();
    }

    public AnimeOverridesViewModel? ViewModel => DataContext as AnimeOverridesViewModel;
}
