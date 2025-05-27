namespace TotoroNext.Module;
public interface IEventAggregator
{
    IObservable<TEvent> GetObservable<TEvent>() where TEvent : IEvent;
    IObserver<TEvent> GetObserver<TEvent>() where TEvent : IEvent;
}
