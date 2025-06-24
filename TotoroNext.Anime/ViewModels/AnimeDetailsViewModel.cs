using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.Extensions;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

public partial class AnimeDetailsViewModel(AnimeModel anime,
                                           IFactory<IMetadataService, Guid> metaFactory,
                                           IFactory<IAnimeProvider, Guid> providerFactory,
                                           IEvent<NavigateToDataRequest> dataNavRequest) : ObservableObject, IAsyncInitializable
{
    private readonly IMetadataService _metadataService = metaFactory.CreateDefault();
    private readonly IAnimeProvider _provider = providerFactory.CreateDefault();

    [ObservableProperty]
    public partial AnimeModel Anime { get; set; } = anime;

    [ObservableProperty]
    public partial List<EpisodeInfo> Episodes { get; set; } = [];

    [ObservableProperty]
    public partial ListItemStatus Status { get; set; } = anime.Tracking!.Status!.Value;

    [ObservableProperty]
    public partial double Progress { get; set; } = anime.Tracking!.WatchedEpisodes!.Value;

    [ObservableProperty]
    public partial double Score { get; set; } = anime.Tracking!.Score!.Value;

    [ObservableProperty]
    public partial DateTimeOffset? StartDate { get; set; } = anime.Tracking!.StartDate;

    [ObservableProperty]
    public partial DateTimeOffset? FinishDate { get; set; } = anime.Tracking!.FinishDate;

    public ListItemStatus[] Statuses { get; } = [.. Enum.GetValues<ListItemStatus>()];

    public async Task InitializeAsync()
    {
        Anime = await _metadataService.GetAnimeAsync(Anime.Id) ?? Anime;
        Episodes = await Anime.GetEpisodes();
    }

    [RelayCommand]
    private async Task WatchEpisode(EpisodeInfo episode)
    {
        var searchResult = await _provider.SearchAndSelectAsync(Anime);

        if(searchResult is null)
        {
            return;
        }

        var episodes = await searchResult.GetEpisodes().ToListAsync();
        var selectedEpisode = episodes.FirstOrDefault(x => (int)x.Number == episode.EpisodeNumber);

        if(selectedEpisode is null)
        {
            return;
        }

        dataNavRequest.Publish(new(new WatchViewModelNavigationParameter(searchResult, Anime, episodes, selectedEpisode, false)));
    }
}
