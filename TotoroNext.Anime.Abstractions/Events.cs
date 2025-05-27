using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotoroNext.Module;

namespace TotoroNext.Anime.Abstractions;

public record AnimeSelectedEvent(SearchResult Item) : IEvent;
public record EpisodeSelectedEvent(Episode Item) : IEvent;
public record PlaybackDurationChangedEvent(TimeSpan Duration) : IEvent;
public record PlaybackPositionChangedEvent(TimeSpan Position) : IEvent;
