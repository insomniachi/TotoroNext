using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.Anime.ViewModels;

public partial class UserListViewModel(IFactory<ITrackingService, Guid> factory) : ReactiveObject
{
    private readonly ITrackingService _trackingService = factory.Create(new Guid("b5d31e9b-b988-44e8-8e28-348f58cf1d04"));

    [Reactive]
    public partial List<AnimeModel> Items { get; set; }

    public async Task Initialize()
    {
        Items = (await _trackingService.GetUserList()) ?? [];
    }
}
