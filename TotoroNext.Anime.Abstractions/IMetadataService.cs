using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TotoroNext.Anime.Abstractions;

public interface IMetadataService
{
    Task<AnimeModel> GetAnimeAsync(long id);
    Task<List<AnimeModel>> SearchAnimeAsync(string term);
    Task<List<AnimeModel>> GetSeasonalAnimeAsync();
    Task<List<AnimeModel>> GetAiringAnimeAsync();
}

public partial class AnimeModel : ObservableObject
{
    public long Id { get; set; }
    public string Image { get; set; } = "";
    public string Title { get; set; } = "";
    public string EngTitle { get; set; } = "";
    public string RomajiTitle { get; set; } = "";
    [ObservableProperty] public partial Tracking? Tracking { get; set; }
    public int? TotalEpisodes { get; set; }
    public AiringStatus AiringStatus { get; set; }
    public float? MeanScore { get; set; }
    public int Popularity { get; set; }
}


public class Tracking
{
    public ListItemStatus? Status { get; set; }
    public int? Score { get; set; }
    public int? WatchedEpisodes { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
}


public enum ListItemStatus
{
    [Description("Watching")]
    Watching,

    [Description("Completed")]
    Completed,

    [Description("On-Hold")]
    OnHold,

    [Description("Plan to Watch")]
    PlanToWatch,

    [Description("Dropped")]
    Dropped,

    [Description("Rewatching")]
    Rewatching,

    [Description("Select status")]
    None
}

public enum AiringStatus
{
    [Description("Finished Airing")]
    FinishedAiring,
    
    [Description("Currently Airing")]
    CurrentlyAiring,
    
    [Description("Not Yet Aired")]
    NotYetAired
}
