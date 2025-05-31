using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Contracts;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public partial class SearchProviderViewModel(IAnimeProviderFactory factory,
                                             [FromKeyedServices("Main")]INavigator navigator,
                                             IModuleStore moduleStore) : ReactiveObject
{
    private readonly IAnimeProvider _provider = factory.GetProvider(new Guid("489576c5-2879-493b-874a-7eb14e081280"));

    [Reactive]
    public partial string Query { get; set; }

    [ObservableAsProperty(PropertyName = "Items")]
    private IObservable<List<SearchResult>> ItemsObservable() => 
        this.WhenAnyValue(x => x.Query)
            .Where(query => query is { Length: > 3 })
            .Throttle(TimeSpan.FromMilliseconds(500))
            .SelectMany(query => _provider.SearchAsync(query).ToListAsync().AsTask())
            .ObserveOn(RxApp.MainThreadScheduler);


    public void Initialize()
    {
        InitializeOAPH();

        moduleStore.GetAllModules().ToListAsync().AsTask().ToObservable().Subscribe(x => 
        {
            // Handle module loading if necessary
        });
    }

    public void NavigateToWatch(SearchResult result)
    {
        navigator.NavigateToData(result);
    }
}
