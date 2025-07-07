using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module;

namespace TotoroNext.MediaEngine.Abstractions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInternalMediaPlayer(this IServiceCollection services)
    {
        var descriptor = new Descriptor()
        {
            Id = new Guid("9339e87b-ed0a-4b24-8dfb-dd8daeaa7d2a"),
            Name = "Internal",
            Components = [ComponentTypes.MediaEngine],
            IsInternal = true
        };
        services.AddTransient(_ => descriptor);
        services.AddKeyedTransient<IMediaPlayer, InternalMediaPlayer>(descriptor.Id);
        services.AddHostedService<VideoStreamProxyService>();
        return services;
    }
}
