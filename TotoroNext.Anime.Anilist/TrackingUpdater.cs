using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Anilist;

public class AnilistTrackingUpdater(IFactory<ITrackingService, Guid> factory,
                                    IEvent<PlaybackProgressEventArgs> playbackProgressEvent) : TrackingUpdater(factory.Create(Module.Id), playbackProgressEvent)
{

}
