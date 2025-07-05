
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;
using Windows.Media.Core;

namespace TotoroNext.Anime.Views;
public sealed partial class AnimeSongsView : UserControl
{
    public AnimeSongsView()
    {
        InitializeComponent();
    }

    public AnimeSongsViweModel? ViewModel => DataContext as AnimeSongsViweModel;

    public static MediaSource? GetVideoSource(Uri? uri)
    {
        if(uri is null)
        {
            return null;
        }

        return MediaSource.CreateFromUri(uri);
    }
}
