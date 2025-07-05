using System.Reactive.Concurrency;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.Anime.ViewModels;

public sealed partial class AnimeSongsViweModel(SongsViewModelNavigationParameters @params,
                                                IAnimeThemes animeThemes,
                                                IFactory<IMediaPlayer, Guid> mediaPlayerFactory) : ObservableObject, IAsyncInitializable, IDisposable
{
    [ObservableProperty]
    public partial List<AnimeTheme> Themes { get; set; } = [];

    [ObservableProperty]
    public partial Uri? SelectedTheme { get; set; }

    public async Task InitializeAsync()
    {
        Themes = await animeThemes.FindById(@params.Anime.Id, @params.Anime.ServiceType ?? "MyAnimeList");
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
        player.Play(new Media(uri, new MediaMetadata(theme.DisplayName)), TimeSpan.Zero);
    }

    public void Dispose()
    {
        RxApp.MainThreadScheduler.Schedule(() => SelectedTheme = null);
    }
}
