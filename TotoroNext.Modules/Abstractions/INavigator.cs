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

public class NavigateToViewModelMessage(Type vm)
{
    public Type ViewModel { get; } = vm;
}

public class NavigateToDataMessage(object data)
{
    public object Data { get; } = data;
}

public class NavigateToRouteMessage(string path)
{
    public string Path { get; } = path;
}
