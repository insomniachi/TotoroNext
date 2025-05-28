using Microsoft.Extensions.DependencyInjection;

namespace TotoroNext.Module;

public interface IModule
{
    void ConfigureServices(IServiceCollection services);
    void ConfigureNavigation(NavigationViewContext context) { }
    void RegisterComponents(IComponentRegistry components) { }
}


public interface IModule<T> : IModule
    where T : new()
{
    Guid Id { get; }
}

