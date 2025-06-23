using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;
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
    public event EventHandler<Type>? Navigated;
    
    public UIElement Control { get; } = host;

    public void NavigateToData(object data)
    {
        if (data is null)
        {
            return;
        }

        var map = locator.FindByData(data.GetType());

        if (map is not { View: { } viewType, ViewModel: { } vmType })
        {
            return;
        }

        var page = (FrameworkElement)Activator.CreateInstance(viewType)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType, data);

        ConfigurePage(page, vmObj);
        Navigate(page);
        Navigated?.Invoke(this, viewType);
    }

    public void NavigateToRoute(string path)
    {
        var map = locator.FindByKey(path);

        if (map is not { View: { } view, ViewModel: { } vm })
        {
            return;
        }

        var page = (FrameworkElement)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vm);

        ConfigurePage(page, vmObj);
        Navigate(page);
        Navigated?.Invoke(this, view);
    }

    public void NavigateViewModel(Type vmType)
    {
        var map = locator.FindByViewModel(vmType);

        if (map is not { View: { } view })
        {
            return;
        }

        var page = (FrameworkElement)Activator.CreateInstance(view)!;
        using var scope = serviceScopeFactory.CreateScope();
        var vmObj = ActivatorUtilities.CreateInstance(scope.ServiceProvider, vmType);

        ConfigurePage(page, vmObj);
        Navigate(page);
        Navigated?.Invoke(this, view);
    }

    private void Navigate(FrameworkElement page)
    {
        if (Control is SplitView sv)
        {
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
                await ia.InitializeAsync();
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

public static class NavigationExtensions
{

    public static bool GetIsAttached(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsAttachedProperty);
    }

    public static void SetIsAttached(DependencyObject obj, bool value)
    {
        obj.SetValue(IsAttachedProperty, value);
    }

    public static readonly DependencyProperty IsAttachedProperty =
        DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(NavigationExtensions), new PropertyMetadata(false, OnIsAttachedChanged));

    private static void OnIsAttachedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        if (args.NewValue is not true)
        {
            return;
        }

        var navigator = ActivatorUtilities.CreateInstance<ControlNavigator>(Container.Services, element);

        element.DataContextChanged += (_, args) =>
        {
            if (args.NewValue is INavigatorHost nh)
            {
                nh.Navigator = navigator;
            }
        };
    }
}
