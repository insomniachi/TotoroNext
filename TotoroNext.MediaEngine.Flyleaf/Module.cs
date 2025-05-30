using Microsoft.Extensions.DependencyInjection;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using FLogLevel = Flyleaf.FFmpeg.LogLevel;

namespace TotoroNext.MediaEngine.Flyleaf;

public class Module : IModule
{
    public static Descriptor Descriptor { get; } = new()
    {
        Id = new Guid("d1f8c3b2-4e5f-4a6b-9c7d-8e1f2c3b4d5e"),
        Name = "Flyleaf Media Engine",
        Version = new Version(1, 0, 0),
        Description = "A module for integrating Flyleaf media player into TotoroNext.",
        HeroImage = "ms-appx:///TotoroNext.MediaEngine.Flyleaf/Assets/wmp.jpg",
        Components = [ ComponentTypes.MediaEngine ]
    };

    public void ConfigureServices(IServiceCollection services)
    {
        StartFlyleaf();

        services.AddTransient(_ => Descriptor); 
        services.AddKeyedTransient<IMediaPlayerElementFactory, MediaPlayerElementFactory>(Descriptor.Id);
    }

    private static void StartFlyleaf()
    {
        FlyleafLib.Engine.Start(new FlyleafLib.EngineConfig()
        {
#if RELEASE
            FFmpegPath = @"FFmpeg",
            FFmpegLogLevel = FLogLevel.Quiet,
            LogLevel = FlyleafLib.LogLevel.Quiet,

#else
            FFmpegLogLevel = FLogLevel.Warn,
            LogLevel = FlyleafLib.LogLevel.Debug,
            LogOutput = ":debug",
            FFmpegPath = @"E:\FFmpeg",
#endif
            UIRefresh = false,    // Required for Activity, BufferedDuration, Stats in combination with Config.Player.Stats = true
            UIRefreshInterval = 250,      // How often (in ms) to notify the UI
            UICurTimePerSecond = true,     // Whether to notify UI for CurTime only when it's second changed or by UIRefreshInterval
        });
    }
}
