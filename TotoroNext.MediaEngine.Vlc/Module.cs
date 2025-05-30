using Microsoft.Extensions.DependencyInjection;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

public class Module : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKeyedTransient<IMediaPlayerElementFactory, VlcMediaPlayerElementFactory>("VLC");
    }
}
