using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;

public interface IInitializable
{
    void Initialize();
}

public interface IAsyncInitializable
{
    Task InitializeAsync();
}

public class FrameNavigator(IViewRegistry locator,
                            IServiceScopeFactory serviceScopeFactory) : IContentControlNavigator
{
    public event EventHandler<Type>? Navigated;

    public ContentControl Frame { get; set; } = null!;

    public void NavigateViewModel(Type vmType)
    {
        var map = locator.FindByViewModel(vmType);

        if (map is not { View: { } view })
        {
            return;
        }

        var page = (Page)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType);

        ConvigurePage(page, vmObj);
        
        Frame.Content = page;
        Navigated?.Invoke(this, view);
    }

    public void NavigateToData<TData>(TData data)
    {
        if (data is null)
        {
            return;
        }

        var map = locator.FindByData(typeof(TData));

        if (map is not { View: { } viewType, ViewModel: { } vmType })
        {
            return;
        }

        var page = (Page)Activator.CreateInstance(viewType)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType, data);

        ConvigurePage(page, vmObj);
        
        Frame.Content = page;
        Navigated?.Invoke(this, viewType);
    }

    public void NavigateToRoute(string path)
    {
        var map = locator.FindByKey(path);

        if (map is not { View: { } view, ViewModel: { } vm })
        {
            return;
        }

        var page = (Page)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm);

        ConvigurePage(page, vmObj);
        
        Frame.Content = page;
        Navigated?.Invoke(this, view);
    }

    private static void ConvigurePage(Page page, object vm)
    {
        page.DataContext = vm;
        page.Loaded += async (_, _) =>
        {
            if (vm is IInitializable { } i)
            {
                i.Initialize();
            }
            if (vm is IAsyncInitializable { } ia)
            {
                await ia.InitializeAsync();
            }
        };
        page.Unloaded += async (_, _) =>
        {
            if(vm is IDisposable d)
            {
                d.Dispose();
            }
            if(vm is IAsyncDisposable ad)
            {
                await ad.DisposeAsync();
            }
        };
    }
}
