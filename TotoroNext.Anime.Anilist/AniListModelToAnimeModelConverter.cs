using System.Diagnostics;
using System.Text.RegularExpressions;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;

namespace TotoroNext.Anime.Anilist;

public partial class AniListModelToAnimeModelConverter
{
    [GeneratedRegex(@"(</?i>)|(<br>)")]
    private static partial Regex DescriptionCleanRegex();

    public static AnimeModel ConvertModel(Media media)
    {
        return new AnimeModel
        {
            Title = media.Title.Romaji ?? media.Title.English ?? string.Empty,
            EngTitle = media.Title.English ?? media.Title.Romaji ?? string.Empty,
            RomajiTitle = media.Title.Romaji ?? media.Title.English ?? string.Empty,
            Id = media.Id ?? 0,
            MalId = media.IdMal ?? 0,
            Image = media.CoverImage.Large,
            TotalEpisodes = media.Episodes,
            AiringStatus = ConvertStatus(media.Status),
            MeanScore = media.MeanScore,
            Popularity = media.Popularity ?? 0,
            Tracking = ConvertTracking(media.MediaListEntry),
            NextEpisodeAt = ConvertToExactTime(media.NextAiringEpisode?.TimeUntilAiring),
            AiredEpisodes = media.NextAiringEpisode?.Episode - 1 ?? 0,
            Season = GetSeason(media.Season, media.SeasonYear),
        };
    }

    public static AnimeModel[] ConvertSimple(IEnumerable<Media> media)
    {
        if (media is null)
        {
            return [];
        }

        return media.Where(x => x.Type == MediaType.Anime)
            .Select(x => new AnimeModel
            {
                Title = x.Title.Romaji ?? x.Title.English ?? string.Empty,
                EngTitle = x.Title.English ?? x.Title.Romaji ?? string.Empty,
                RomajiTitle = x.Title.Romaji ?? x.Title.English ?? string.Empty,
                Id = x.Id ?? 0,
                Image = x.CoverImage.Large,
                Tracking = ConvertTracking(x.MediaListEntry),
                AiringStatus = ConvertStatus(x.Status),
            })
            .ToArray();
    }

    public static AiringStatus ConvertStatus(MediaStatus? status)
    {
        return status switch
        {
            MediaStatus.Releasing => AiringStatus.CurrentlyAiring,
            MediaStatus.Finished => AiringStatus.FinishedAiring,
            _ => AiringStatus.NotYetAired
        };
    }

    public static DateTime? ConvertToExactTime(int? secondsTillAiring)
    {
        if (secondsTillAiring is null)
        {
            return null;
        }

        return DateTime.Now + TimeSpan.FromSeconds(secondsTillAiring.Value);
    }

    public static Tracking? ConvertTracking(MediaList listEntry)
    {
        if (listEntry == null)
        {
            // uncomment for debuging other people list
            //return new Tracking
            //{
            //    Status = AnimeStatus.Dropped
            //};

            return null;
        }

        return new Tracking
        {
            WatchedEpisodes = listEntry.Progress,
            Score = (int)(listEntry.Score ?? 0),
            Status = ConvertListStatus(listEntry.Status),
            StartDate = ConvertDate(listEntry.StartedAt),
            FinishDate = ConvertDate(listEntry.CompletedAt),
        };
    }

    public static ListItemStatus? ConvertListStatus(MediaListStatus? status)
    {
        return status switch
        {
            MediaListStatus.Current => ListItemStatus.Watching,
            MediaListStatus.Planning => ListItemStatus.PlanToWatch,
            MediaListStatus.Paused => ListItemStatus.OnHold,
            MediaListStatus.Dropped => ListItemStatus.Dropped,
            MediaListStatus.Completed => ListItemStatus.Completed,
            MediaListStatus.Repeating => ListItemStatus.Rewatching,
            _ => null
        };
    }

    public static MediaListStatus? ConvertListStatus(ListItemStatus? status)
    {
        return status switch
        {
            ListItemStatus.Watching => MediaListStatus.Current,
            ListItemStatus.PlanToWatch => MediaListStatus.Planning,
            ListItemStatus.OnHold => MediaListStatus.Paused,
            ListItemStatus.Completed => MediaListStatus.Completed,
            ListItemStatus.Dropped => MediaListStatus.Dropped,
            ListItemStatus.Rewatching => MediaListStatus.Repeating,
            _ => null
        };
    }


    public static DateTime? ConvertDate(FuzzyDate date)
    {
        if (date is null || date.Year is null || date.Month is null || date.Day is null)
        {
            return null;
        }

        return new DateTime(date.Year.Value, date.Month.Value, date.Day.Value);
    }

    public static FuzzyDateInput? ConvertDate(DateTime? date)
    {
        if (date is null)
        {
            return null;
        }

        return new FuzzyDateInput
        {
            Year = date.Value.Year,
            Month = date.Value.Month,
            Day = date.Value.Day,
        };
    }

    public static MediaSeason ConvertSeason(AnimeSeason season)
    {
        return season switch
        {
            AnimeSeason.Spring => MediaSeason.Spring,
            AnimeSeason.Summer => MediaSeason.Summer,
            AnimeSeason.Fall => MediaSeason.Fall,
            AnimeSeason.Winter => MediaSeason.Winter,
            _ => throw new UnreachableException()
        };
    }

    public static AnimeSeason ConvertSeason(MediaSeason season)
    {
        return season switch
        {
            MediaSeason.Spring => AnimeSeason.Spring,
            MediaSeason.Summer => AnimeSeason.Summer,
            MediaSeason.Fall => AnimeSeason.Fall,
            MediaSeason.Winter => AnimeSeason.Winter,
            _ => throw new UnreachableException()
        };
    }

    private static DayOfWeek? GetBroadcastDay(FuzzyDate date)
    {
        if (date is null || date.Year is null || date.Month is null || date.Day is null)
        {
            return null;
        }

        return new DateOnly(date.Year.Value, date.Month.Value, date.Day.Value).DayOfWeek;
    }

    private static IEnumerable<string> GetAlternateTiltes(MediaTitle title)
    {
        var list = new List<string>();

        if (!string.IsNullOrEmpty(title.English))
        {
            list.Add(title.English);
        }
        if (!string.IsNullOrEmpty(title.Romaji))
        {
            list.Add(title.Romaji);
        }
        if (!string.IsNullOrEmpty(title.Native))
        {
            list.Add(title.Native);
        }

        return list.Distinct();
    }

    private static string ConvertFormat(MediaFormat? format)
    {
        return format switch
        {
            MediaFormat.Tv => "TV",
            MediaFormat.Ova => "OVA",
            MediaFormat.Novel => "Novel",
            MediaFormat.Movie => "Movie",
            MediaFormat.Music => "Music",
            MediaFormat.Ona => "ONA",
            MediaFormat.Special => "Special",
            MediaFormat.OneShot => "One Shot",
            MediaFormat.TvShort => "TV Short",
            _ => ""
        };
    }

    private static Season? GetSeason(MediaSeason? season, int? year)
    {
        if (season is null || year is null)
        {
            return null;
        }

        return new Season(ConvertSeason(season.Value), year.Value);
    }
}
