using System.Reactive.Concurrency;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.Anime.ViewModels;

public sealed partial class AnimeSongsViewModel(SongsViewModelNavigationParameters @params,
                                                IAnimeThemes animeThemes,
                                                IFactory<IMediaPlayer, Guid> mediaPlayerFactory) : ObservableObject, IAsyncInitializable
{
    [ObservableProperty]
    public partial List<AnimeTheme> Themes { get; set; } = [];

    [ObservableProperty]
    public partial Uri? SelectedTheme { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public event EventHandler OnDisposed;

    public async Task InitializeAsync()
    {
        IsLoading = true;

        Themes = await animeThemes.FindById(@params.Anime.Id, @params.Anime.ServiceType ?? "MyAnimeList");

        IsLoading = false;
    }

    [RelayCommand]
    private void Play(AnimeTheme theme)
    {
        SelectedTheme = theme.Video;
    }

    [RelayCommand]
    private void PlayAudio(AnimeTheme them)
    {
        SelectedTheme = them.Audio;
    }

    [RelayCommand]
    private void OpenInMediaPlayer(AnimeTheme theme)
    {
        if(theme.Video is not { } uri)
        {
            return;
        }

        var player = mediaPlayerFactory.CreateDefault();
        player.Play(new Media(uri, new MediaMetadata(theme.GetDisplayName())), TimeSpan.Zero);
    }
}
