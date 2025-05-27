using Microsoft.Extensions.DependencyInjection;

namespace TotoroNext.Module;

internal class EventAggregator(IServiceScopeFactory serviceScopeFactory) : IEventAggregator
{
    public IObservable<TEvent> GetObservable<TEvent>() where TEvent : IEvent
    {
        using var scope = serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IObservable<TEvent>>();
    }

    public IObserver<TEvent> GetObserver<TEvent>() where TEvent : IEvent
    {
        using var scope = serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IObserver<TEvent>>();
    }
}
