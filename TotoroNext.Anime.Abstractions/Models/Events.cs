using TotoroNext.Module;

namespace TotoroNext.Anime.Abstractions.Models;

public record AnimeSelectedEvent(SearchResult Item) : IEvent;
public record EpisodeSelectedEvent(Episode Item) : IEvent;
public record PlaybackDurationChangedEvent(TimeSpan Duration) : IEvent;
public record PlaybackPositionChangedEvent(TimeSpan Position) : IEvent;
public record TrackableAnimeSelectedEvent(AnimeModel Item) : IEvent;
