using System.Reactive.Subjects;
using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<IComponentRegistry, ComponentRegistry>();
        services.AddSingleton<IViewRegistry, ViewRegistry>();

        return services;
    }

    public static IServiceCollection AddNavigationViewItem<TView, TViewModel>(this IServiceCollection services, string navigationViewName, string title, IconElement icon, object? tag = null)
        where TView : class, new()
        where TViewModel : class
    {
        services.AddKeyedViewMap<TView, TViewModel>(title);
        services.AddTransient(sp =>
        {
            var facade = sp.GetRequiredKeyedService<IFrameNavigator>(navigationViewName);

            var item = new NavigationViewItem
            {
                Tag = tag,
                Content = title,
                Icon = icon,
            };

            item.Tapped += (_, _) =>
            {
                facade.NavigateViewModel(typeof(TViewModel));
            };

            facade.Initialized += (_, e) =>
            {
                e.Navigated += (_, args) =>
                {
                    item.IsSelected = args.Content is Page page && page.GetType() == typeof(TView);
                };
            };

            return item;
        });

        return services;
    }

    public static IServiceCollection AddMainNavigationViewItem<TView,TViewModel>(this IServiceCollection services, string title, IconElement icon, bool isFooterItem = false)
        where TView : class, new()
        where TViewModel : class
    {
        return services.AddNavigationViewItem<TView, TViewModel>("Main", title, icon, isFooterItem);
    }

    public static IServiceCollection RegisterEvent<TEvent>(this IServiceCollection services)
        where TEvent : IEvent
    {
        var subject = new Subject<TEvent>();
        services.AddSingleton<IObservable<TEvent>>(subject);
        services.AddSingleton<IObserver<TEvent>>(subject);

        return services;
    }

    public static IServiceCollection AddModuleSettings<TData>(this IServiceCollection services, IModule<TData> module)
        where TData : class, new()
    {
        return services.AddSingleton<IModuleSettings<TData>>(_ => new ModuleSettings<TData>(module.Descriptor.Id));
    }

    public static IServiceCollection AddNavigationView(this IServiceCollection services, string key)
    {
        services.AddKeyedSingleton<IFrameNavigator, FrameNavigator>(key);
        services.AddKeyedSingleton<INavigator>(key, (sp, k) => sp.GetRequiredKeyedService<IFrameNavigator>(k));
        return services;
    }

    public static IServiceCollection AddViewMap<TView,TViewModel>(this IServiceCollection services)
        where TView : class, new ()
        where TViewModel : class
    {
        return services.AddViewMap(new ViewMap<TView, TViewModel>());
    }

    public static IServiceCollection AddViewMap(this IServiceCollection services, ViewMap map)
    {
        services.AddTransient(_ => map);
        services.AddTransient(map.ViewModel);

        return services;
    }

    public static IServiceCollection AddDataViewMap<TView, TViewModel, TData>(this IServiceCollection services)
        where TView : class, new()
        where TViewModel : class
        where TData : class
    {
        return services.AddViewMap(new DataViewMap<TView, TViewModel, TData>());
    }

    public static IServiceCollection AddKeyedViewMap<TView, TViewModel>(this IServiceCollection services, string key)
        where TView : class, new()
        where TViewModel : class
    {
        return services.AddViewMap(new KeyedViewMap<TView, TViewModel>(key));
    }
}
