namespace TotoroNext.Module.Abstractions;
public interface IEventAggregator
{
    IObservable<TEvent> GetObservable<TEvent>() where TEvent : IEvent;
    IObserver<TEvent> GetObserver<TEvent>() where TEvent : IEvent;
}
