using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using TotoroNext.Anime.ViewModels;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module.Abstractions;


namespace TotoroNext.Anime.Views;

public sealed partial class WatchPage : Page
{
    public WatchPage()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<FullScreenEntered>(this, (_, _) =>
        {
            EpisodeList.Visibility = Visibility.Collapsed;
            ServerList.Visibility = Visibility.Collapsed;
        });

        WeakReferenceMessenger.Default.Register<FullScreenExited>(this, (_, _) =>
        {
            EpisodeList.Visibility = Visibility.Visible;
            ServerList.Visibility = Visibility.Visible;
        });

        Loaded += WatchPage_Loaded;
    }

    private void WatchPage_Loaded(object sender, RoutedEventArgs e)
    {
        if(ViewModel is not { } vm)
        {
            return;
        }

        if(vm.MediaPlayer is not IInternalMediaPlayer mp)
        {
            return;
        }

        InternalMediaPlayer.Visibility = Visibility.Visible;
        InternalMediaPlayer.SetMediaPlayer(mp.MediaPlayer);
    }

    public WatchViewModel? ViewModel => DataContext as WatchViewModel;

    private void InternalMediaPlayer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send<ToggleAppWindowPresenterMessage>();
    }
}

