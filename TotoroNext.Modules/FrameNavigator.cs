using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;

public class FrameNavigator(IViewRegistry locator,
                            IServiceScopeFactory serviceScopeFactory) : IFrameNavigator
{
    public event EventHandler<Frame>? Initialized;

    public Frame Frame
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

        var navigated = Frame.Navigate(view);

        if(navigated && Frame.Content is Page page)
        {
            using var scope = serviceScopeFactory.CreateScope();
            page.DataContext = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType);
        }
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

        var navigated = Frame.Navigate(view);

        if (navigated && Frame.Content is Page page)
        {
            using var scope = serviceScopeFactory.CreateScope();
            page.DataContext = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm, data);
        }
    }

    public void NavigateToRoute(string path)
    {
        var map = locator.FindByKey(path);

        if (map is not { View : { } view, ViewModel : { } vm })
        {
            return;
        }

        var navigated = Frame.Navigate(view);

        if (navigated && Frame.Content is Page page)
        {
            using var scope = serviceScopeFactory.CreateScope();
            page.DataContext = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm);
        }
    }
}
