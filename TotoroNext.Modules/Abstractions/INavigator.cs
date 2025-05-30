namespace TotoroNext.Module.Abstractions;

public interface INavigator
{
    void NavigateViewModel(Type vmType);
    void NavigateToData<TData>(TData data);
    void NavigateToRoute(string path);
}
