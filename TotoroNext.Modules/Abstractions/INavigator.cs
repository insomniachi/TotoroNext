namespace TotoroNext.Module.Abstractions;

public interface INavigator
{
    event EventHandler<Type>? Navigated;
    void NavigateViewModel(Type vmType);
    void NavigateToData(object data);
    void NavigateToRoute(string path);
}

public interface INavigatorHost
{
    INavigator? Navigator { get; set; }
}

public record NavigateToViewModelRequest(Type Type);
public record NavigateToDataRequest(object Data);
public record NavigateToRouteRequest(string Path);
