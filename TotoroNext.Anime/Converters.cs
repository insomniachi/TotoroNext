using System.ComponentModel;
using System.Reflection;
using System.Text;
using Microsoft.UI.Xaml.Media.Imaging;
using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime;

public static class Converters
{
    public static ImageSource? UriToImage(Uri? uri)
    {
        if(uri is null)
        {
            return null;
        }

        return new BitmapImage(uri);
    }

    public static ImageSource? StreamToImage(Stream? stream)
    {
        if(stream is null)
        {
            return null;
        }

        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        return bitmapImage;
    }

    public static ImageSource? StringToImage(string? uri)
    {
        if (uri is null)
        {
            return null;
        }

        return new BitmapImage(new Uri(uri));
    }

    public static Guid ToGuid(string guid) => Guid.Parse(guid);

	public static string EnumToDescription(Enum enumValue)
	{
		var name = enumValue.ToString();
		var field = enumValue.GetType().GetField(name);
		if (field != null)
		{
			if (field.GetCustomAttribute<DescriptionAttribute>() is { } attr)
			{
				return attr.Description;
			}
		}
		return name;
	}

    public static string NextEpisodeAiringTime(DateTime? airingAt, int current)
    {
        return airingAt is null
            ? string.Empty
            : $"EP{current + 1}: {(airingAt.Value - DateTime.Now).HumanizeTimeSpan()}";
    }

    public static Visibility ObjectToVisiblity(object? value) => value is null ? Visibility.Collapsed : Visibility.Visible;

    public static int GetUnwatchedEpsiodes(AnimeModel anime)
    {
        if (anime is null)
        {
            return -1;
        }

        if (anime.Tracking is null || anime.Tracking.WatchedEpisodes is null)
        {
            return -1;
        }

        if (anime.AiredEpisodes == 0)
        {
            return -1;
        }

        return (anime.AiredEpisodes - anime.Tracking.WatchedEpisodes.Value);
    }

    public static Visibility UnwatchedEpisodesVisiblity(AnimeModel anime)
    {
        return GetUnwatchedEpsiodes(anime) > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public static string HumanizeTimeSpan(this TimeSpan ts)
    {
        var sb = new StringBuilder();
        var week = ts.Days / 7;
        var days = ts.Days % 7;
        if (week > 0)
        {
            sb.Append($"{week}w ");
        }
        if (days > 0)
        {
            sb.Append($"{days}d ");
        }
        if (ts.Hours > 0)
        {
            sb.Append($"{ts.Hours.ToString().PadLeft(2, '0')}h ");
        }
        if (ts.Minutes > 0)
        {
            sb.Append($"{ts.Minutes.ToString().PadLeft(2, '0')}m ");
        }
        //if (ts.Seconds > 0)
        //{
        //    sb.Append($"{ts.Seconds.ToString().PadLeft(2, '0')}s ");
        //}

        return sb.ToString().TrimEnd();
    }
}
