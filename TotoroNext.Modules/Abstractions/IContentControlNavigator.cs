namespace TotoroNext.Module.Abstractions;

public interface IContentControlNavigator : INavigator
{
    event EventHandler<Type>? Navigated;
    ContentControl Control { get; set; }
}
