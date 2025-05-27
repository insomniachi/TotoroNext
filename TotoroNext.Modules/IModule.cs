using Microsoft.Extensions.DependencyInjection;

namespace TotoroNext.Module;

public interface IModule
{
    public void ConfigureServices(IServiceCollection services);
    public void ConfigureNavigation(NavigationViewContext context);
}
