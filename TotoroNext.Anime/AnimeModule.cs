using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Anime.Views;
using TotoroNext.Module;

namespace TotoroNext.Anime;

public class AnimeModule : IModule
{
    public void ConfigureNavigation(NavigationViewContext context)
    {
        context.RegisterForNavigation<SearchProviderPage, SearchProviderViewModel>();
        context.RegisterForNavigation<FindEpisodesPage, FindEpisodesViewModel, SearchResult>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        StartFlyleaf();

        services.AddNavigationViewItem<SearchProviderViewModel>("Search", new SymbolIcon(Symbol.Find));
        services.AddNavigationViewItem<FindEpisodesViewModel>("Episodes", new SymbolIcon(Symbol.World));
    }

    private static void StartFlyleaf()
    {
        FlyleafLib.Engine.Start(new FlyleafLib.EngineConfig()
        {
#if RELEASE
            FFmpegPath = @"FFmpeg",
            FFmpegLogLevel = Flyleaf.FFmpeg.LogLevel.Quiet,
            LogLevel = FlyleafLib.LogLevel.Quiet,

#else
            FFmpegLogLevel = Flyleaf.FFmpeg.LogLevel.Warn,
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
