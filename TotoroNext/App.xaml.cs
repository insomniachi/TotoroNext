using System.Diagnostics;
using TotoroNext.Module;
using Uno.Resizetizer;

namespace TotoroNext;
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        //Process.Start(new ProcessStartInfo
        //{
        //    FileName = @"C:\Users\athul\Downloads\mpv-x86_64-20250527-git-1d1535f\mpv.exe"
        //});

        IModule[] modules =
        [
            new Anime.AnimeModule(),
            new AnimeHeaven.Module(),
            new MediaEngine.Vlc.Module(),
            new MediaEngine.Mpv.Module(),
#if WINDOWS10_0_26100_0_OR_GREATER
            new MediaEngine.Flyleaf.FlyleafModule(),
#endif
        ];

        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Trace);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                .UseLocalization()
                .UseHttp((context, services) =>
                {

                })
                .ConfigureServices((context, services) =>
                {
                    services.AddEventAggregator();
                    foreach (var module in modules)
                    {
                        module.ConfigureServices(services);
                    }
                })
                .UseNavigation((views, routes) => RegisterRoutes(views, routes, modules))
            );

        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
        Container.ConfigureServices(Host.Services);
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes, IEnumerable<IModule> modules)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<MainPage, MainViewModel>());

        var context = new NavigationViewContext(views);
        foreach (var module in modules)
        {
            module.ConfigureNavigation(context);
        }

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
            Nested:
            [
                new ("Main", View: views.FindByViewModel<MainViewModel>(), IsDefault:true, Nested:
                [
                    ..context.Routes
                ]),
            ])
        );
    }
}
