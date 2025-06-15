using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.MyAnimeList;

public class MyAnimeListTrackingUpdater(IFactory<ITrackingService, Guid> factory,
                                        IEvent<PlaybackProgressEventArgs> playbackProgressEvent) : TrackingUpdater(factory.Create(Module.Id), playbackProgressEvent)
{

}
