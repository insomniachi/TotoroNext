namespace TotoroNext.Anime.Abstractions;

public interface ITrackingService
{
    Task<Tracking> Update(long id, Tracking tracking);
    Task<bool> Remove(long id);
    Task<List<AnimeModel>> GetUserList();
}
