using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;
using Uno.Logging;
using Uno.UI.Extensions;

namespace TotoroNext.Module;

public interface IInitializable
{
    void Initialize();
}

public interface IAsyncInitializable
{
    Task InitializeAsync();
}

public interface IPaneNavigatable
{
    INavigator PaneNavigator { get; set; }
}

public class ControlNavigator(UIElement host,
                              IViewRegistry locator,
                              IServiceScopeFactory serviceScopeFactory) : INavigator
{
    public event EventHandler<NavigationResult>? Navigated;
    
    public UIElement Control { get; } = host;

    public bool NavigateToData(object data)
    {
        try
        {
            if (data is null)
            {
                return false;
            }

            var map = locator.FindByData(data.GetType());

            if (map is not { View: { } viewType, ViewModel: { } vmType })
            {
                return false;
            }

            var page = (FrameworkElement)Activator.CreateInstance(viewType)!;
            using var scope = serviceScopeFactory.CreateScope();
            var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType, data);

            ConfigurePage(page, vmObj);
            Navigate(page);
            Navigated?.Invoke(this, new(viewType, vmType));
            return true;
        }
        catch(Exception ex)
        {
            this.Log().Error("Unable to Navigate", ex);
            return false;
        }
    }

    public bool NavigateToRoute(string path)
    {
        try
        {
            var map = locator.FindByKey(path);

            if (map is not { View: { } viewType, ViewModel: { } vmType })
            {
                return false;
            }

            var page = (FrameworkElement)Activator.CreateInstance(viewType)!;
            using var scope = serviceScopeFactory.CreateScope();
            var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType);

            ConfigurePage(page, vmObj);
            Navigate(page);
            Navigated?.Invoke(this, new(viewType, vmType));

            return true;
        }
        catch
        {
            return true;
        }
    }

    public bool NavigateViewModel(Type vmType)
    {
        try
        {
            var map = locator.FindByViewModel(vmType);

            if (map is not { View: { } viewType })
            {
                return false;
            }

            var page = (FrameworkElement)Activator.CreateInstance(viewType)!;
            using var scope = serviceScopeFactory.CreateScope();
            var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType);

            ConfigurePage(page, vmObj);
            Navigate(page);
            Navigated?.Invoke(this, new(viewType, vmType));

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void Navigate(FrameworkElement page)
    {
        if (Control is SplitView sv)
        {
            if(NavigationExtensions.GetPaneWidth(page) is { } width && width > 0)
            {
                sv.OpenPaneLength = width;
            }
            sv.IsPaneOpen = true;
            sv.Pane = page;
        }
        else if (Control is ContentControl cc)
        {
            cc.Content = page;
        }
    }

    protected void ConfigurePage(FrameworkElement page, object vm)
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
                try
                {
                    await ia.InitializeAsync();
                }
                catch { }
            }
            if(vm is IPaneNavigatable { } pn && page.FindFirstDescendant<SplitView>() is { } sv)
            {
                var navigator = new ControlNavigator(sv, locator, serviceScopeFactory);
                pn.PaneNavigator = navigator;
            }
        };
        page.Unloaded += async (_, _) =>
        {
            if (vm is IDisposable d)
            {
                d.Dispose();
            }
            if (vm is IAsyncDisposable ad)
            {
                await ad.DisposeAsync();
            }
        };
    }
}
