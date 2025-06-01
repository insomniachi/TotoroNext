namespace TotoroNext.Module.Abstractions;

public interface IContentControlNavigator : INavigator
{
    event EventHandler<ContentControl> Initialized;
    ContentControl Frame { get; set; }
}
