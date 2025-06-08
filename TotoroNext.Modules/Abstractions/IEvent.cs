namespace TotoroNext.Module.Abstractions;

public interface IEvent<TArgs>
{
    IObservable<TArgs> OnNext();
    void Publish(TArgs data);
}
