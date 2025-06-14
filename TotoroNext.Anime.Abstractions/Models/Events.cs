namespace TotoroNext.Anime.Abstractions.Models;

public record PlaybackProgressEventArgs(AnimeModel Anime, Episode Episode, TimeSpan Duration, TimeSpan Position);
public record PlaybackEndedEventArgs;
