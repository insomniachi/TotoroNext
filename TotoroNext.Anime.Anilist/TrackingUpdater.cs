using TotoroNext.Anime.Abstractions;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Anilist;

public class AnilistTrackingUpdater(IFactory<ITrackingService, Guid> factory,
                                    IEventAggregator eventAggregator) : TrackingUpdater(factory.Create(Module.Id), eventAggregator)
{

}
