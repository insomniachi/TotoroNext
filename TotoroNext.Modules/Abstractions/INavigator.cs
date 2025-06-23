namespace TotoroNext.Module.Abstractions;

public interface INavigator
{
    event EventHandler<Type>? Navigated;
    bool NavigateViewModel(Type vmType);
    bool NavigateToData(object data);
    bool NavigateToRoute(string path);
}

public interface INavigatorHost
{
    INavigator? Navigator { get; set; }
}

public record NavigateToViewModelRequest(Type Type);
public record NavigateToDataRequest(object Data);
public record NavigateToRouteRequest(string Path);
