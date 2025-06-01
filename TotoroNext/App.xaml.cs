using System.Runtime.InteropServices;
using ReactiveUI;
using TotoroNext.Anime;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using Uno.Resizetizer;
using Windows.Storage.Pickers;

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

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {

        List<IModule> modules =
        [
            new Anime.Module(),
        ];

#if !DEBUG
        var store = new DebugModuleStore();
        modules.AddRange(
            [
                new Anime.AllAnime.Module(),
                new MediaEngine.Mpv.Module(),
                new MediaEngine.Vlc.Module()
            ]);
#else
        var store = new ModuleStore();
#endif

        modules.AddRange(await store.LoadModules().ToListAsync());

#if WINDOWS10_0_26100_0_OR_GREATER
        //modules.Add(new MediaEngine.Flyleaf.Module());
#endif

        var builder = this.CreateBuilder(args)
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
                        .CoreLogLevel(LogLevel.Warning);

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
                    services.AddSingleton<IModuleStore>(store);
                    services.AddCoreServices();
                    services.AddAnimeServices();
                    services.AddNavigationView("Main");
                    services.AddTransient(sp =>
                    {
                        var openPicker = new FileOpenPicker();

                        if(OperatingSystem.IsWindows())
                        {
                            var window = MainWindow;
                            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
                        }

                        return openPicker;
                    });

                    foreach (var module in modules)
                    {
                        module.ConfigureServices(services);
                    }

                    services.AddMainNavigationViewItem<ModulesPage, ModulesViewModel>("My Modules", new FontIcon { Glyph = "\uED35" }, true);
                    services.AddMainNavigationViewItem<ModulesStorePage, ModulesStoreViewModel>("Store", new FontIcon { Glyph = "\uEA40" });
                })
            );

        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = builder.Build();

        foreach (var module in modules)
        {
            module.RegisterComponents(Host.Services.GetRequiredService<IComponentRegistry>());
        }

        Container.ConfigureServices(Host.Services);

        MainWindow = new Window
        {
            Content = new MainPage
            {
                DataContext = ActivatorUtilities.CreateInstance<MainViewModel>(Host.Services)
            }
        };

        MainWindow.Activate();

        RxApp.MainThreadScheduler = new DispatcherQueueScheduler(MainWindow.DispatcherQueue);
        MainWindow.ExtendsContentIntoTitleBar = true;
    }
}
