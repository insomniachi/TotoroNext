using Microsoft.Extensions.DependencyInjection;

namespace TotoroNext.Module;

public interface IModule
{
    public void ConfigureServices(IServiceCollection services);
    public void ConfigureNavigation(NavigationViewContext context);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNavigationViewItem<TViewModel>(this IServiceCollection services, string title, IconElement icon)
    {
        var item = new NavigationViewItem
        {
            Content = title,
            Icon = icon,
        };
        Navigation.SetRequest(item, $"./{typeof(TViewModel).Name}");

        return services.AddSingleton(item);
    }
}

public class NavigationViewContext(IViewRegistry views)
{
    public List<RouteMap> Routes { get; } = [];

    public void RegisterForNavigation<TView, TViewModel>()
        where TView : class, new()
        where TViewModel : class
    {
        views.Register(new ViewMap<TView, TViewModel>());
        Routes.Add(new RouteMap(typeof(TViewModel).Name, View: views.FindByViewModel<TViewModel>()));
    }
}
