using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Discord;

public class Module : IModule
{
    public static Descriptor Descriptor { get; } = new Descriptor
    {
        Id = new Guid("b3329249-3e29-4625-8211-2934dade3c37"),
        Name = "Discord Rich Presense",
        HeroImage = ResourceHelper.GetResource("discord-logo.jpg"),
        Description = "Custom discord rich presense while watching"
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient(_ => Descriptor);
        services.AddHostedService<RpcService>();
    }
}
