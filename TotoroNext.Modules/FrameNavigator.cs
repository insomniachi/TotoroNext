using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;

public class FrameNavigator(IViewRegistry locator,
                            IServiceScopeFactory serviceScopeFactory) : IContentControlNavigator
{
    public event EventHandler<ContentControl>? Initialized;

    public ContentControl Frame
    {
        get => field;
        set
        {
            field = value;
            if(value is not null)
            {
                Initialized?.Invoke(this, value);
            }
        }
    } = null!;

    public void NavigateViewModel(Type vmType)
    {
        var map = locator.FindByViewModel(vmType);

        if (map is not { View: { } view })
        {
            return;
        }

        var type = (Page)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        type.DataContext = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType);
        Frame.Content = type;
    }

    public void NavigateToData<TData>(TData data)
    {
        if(data is null)
        {
            return;
        }

        var map = locator.FindByData(typeof(TData));
        
        if (map is not { View: { } view, ViewModel: { } vm })
        {
            return;
        }

        var type = (Page)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        type.DataContext = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm, data);
        Frame.Content = type;
    }

    public void NavigateToRoute(string path)
    {
        var map = locator.FindByKey(path);

        if (map is not { View : { } view, ViewModel : { } vm })
        {
            return;
        }

        var type = (Page)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        type.DataContext = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm);
        Frame.Content = type;
    }
}
