using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;

namespace TotoroNext.Anime.ViewModels.Parameters;

public partial record WatchViewModelNavigationParameter(SearchResult ProviderResult, AnimeModel? Anime = null);
