using Microsoft.Extensions.DependencyInjection;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;
using FLogLevel = Flyleaf.FFmpeg.LogLevel;

namespace TotoroNext.MediaEngine.Flyleaf;

public class FlyleafModule : IModule
{
    public void ConfigureNavigation(NavigationViewContext context) { }

    public void ConfigureServices(IServiceCollection services)
    {
        StartFlyleaf();

        services.AddKeyedTransient<IMediaPlayerElementFactory, MediaPlayerElementFactory>("Flyleaf");
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
