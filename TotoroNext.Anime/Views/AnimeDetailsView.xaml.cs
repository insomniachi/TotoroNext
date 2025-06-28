

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
}
