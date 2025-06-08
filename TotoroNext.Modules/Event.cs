using System.Reactive.Subjects;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;


public class Event<TArgs>(Subject<TArgs> subject) : IEvent<TArgs>
{
    public IObservable<TArgs> OnNext() => subject;
    public void Publish(TArgs data) => subject.OnNext(data);
}
