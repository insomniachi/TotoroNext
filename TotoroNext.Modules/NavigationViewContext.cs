namespace TotoroNext.Module;

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
    
    public void RegisterForNavigation<TView, TViewModel, TData>()
        where TView : class, new()
        where TViewModel : class
        where TData: class
    {
        views.Register(new DataViewMap<TView, TViewModel, TData>());
        Routes.Add(new RouteMap(typeof(TViewModel).Name, View: views.FindByViewModel<TViewModel>()));
    }
}
