using System.Reactive.Subjects;
using Microsoft.Extensions.DependencyInjection;

namespace TotoroNext.Module;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventAggregator(this IServiceCollection services)
    {
        services.AddSingleton<IEventAggregator, EventAggregator>();
        return services;
    }

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

    public static IServiceCollection RegisterEvent<TEvent>(this IServiceCollection services)
        where TEvent : IEvent
    {
        var subject = new Subject<TEvent>();
        services.AddSingleton<IObservable<TEvent>>(subject);
        services.AddSingleton<IObserver<TEvent>>(subject);

        return services;
    }
}
