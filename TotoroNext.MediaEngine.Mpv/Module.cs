using Microsoft.Extensions.DependencyInjection;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.MediaEngine.Mpv;

public class Module : IModule
{
    public void ConfigureNavigation(NavigationViewContext context) { }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKeyedTransient<IMediaPlayerElementFactory, MpvMediaPlayerElementFactory>("MPV");
    }
}
