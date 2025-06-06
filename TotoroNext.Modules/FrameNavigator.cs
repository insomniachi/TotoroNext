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

        var type = (Page)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType);
        type.DataContext = vmObj;
        Frame.Content = type;
        Navigated?.Invoke(this, view);

        if (vmObj is IInitializable { } i)
        {
            i.Initialize();
        }
        if (vmObj is IAsyncInitializable { } ia)
        {
            Task.Run(ia.InitializeAsync);
        }
    }

    public void NavigateToData<TData>(TData data)
    {
        if (data is null)
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
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm, data);
        type.DataContext = vmObj;

        Frame.Content = type;
        Navigated?.Invoke(this, view);

        if (vmObj is IInitializable { } i)
        {
            i.Initialize();
        }
        if (vmObj is IAsyncInitializable { } ia)
        {
            Task.Run(ia.InitializeAsync);
        }

    }

    public void NavigateToRoute(string path)
    {
        var map = locator.FindByKey(path);

        if (map is not { View: { } view, ViewModel: { } vm })
        {
            return;
        }

        var type = (Page)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm);
        type.DataContext = vmObj;

        Frame.Content = type;
        Navigated?.Invoke(this, view);

        if (vmObj is IInitializable { } i)
        {
            i.Initialize();
        }
        if (vmObj is IAsyncInitializable { } ia)
        {
            Task.Run(ia.InitializeAsync);
        }
    }
}
