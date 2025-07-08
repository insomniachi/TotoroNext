using CommunityToolkit.Mvvm.Messaging;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Module.Abstractions;
using Windows.Media.Core;

namespace TotoroNext.Anime.Views;
public sealed partial class AnimeSongsView : UserControl
{
    public AnimeSongsView()
    {
        InitializeComponent();

        Unloaded += (_, _) => Pause();
        WeakReferenceMessenger.Default.Register<PaneClosingMessange>(this, (_, _) => Pause());
    }

    public AnimeSongsViewModel? ViewModel => DataContext as AnimeSongsViewModel;

    public static MediaSource? GetVideoSource(Uri? uri)
    {
        if (uri is null)
        {
            return null;
        }

        return MediaSource.CreateFromUri(uri);
    }

    private void Pause()
    {
        try
        {
            if (MP.MediaPlayer.CurrentState is Windows.Media.Playback.MediaPlayerState.Playing)
            {
                MP.MediaPlayer.Pause();
            }
        }
        catch { }
    }
}
