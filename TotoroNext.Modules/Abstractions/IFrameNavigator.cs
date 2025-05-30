namespace TotoroNext.Module.Abstractions;

public interface IFrameNavigator : INavigator
{
    event EventHandler<Frame> Initialized;
    Frame Frame { get; set; }
}
