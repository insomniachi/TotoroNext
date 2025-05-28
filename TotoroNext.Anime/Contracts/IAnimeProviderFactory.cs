using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime.Contracts;

public interface IAnimeProviderFactory
{
    IAnimeProvider GetProvider(Guid id);
}
