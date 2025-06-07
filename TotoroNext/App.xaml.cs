using System.Reactive.Concurrency;
using System.Runtime.InteropServices;
using ReactiveUI;
using TotoroNext.Anime;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using Uno.Resizetizer;
using Windows.Storage.Pickers;

namespace TotoroNext;
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        List<IModule> modules =
        [
            new Anime.Module(),
        ];

#if DEBUG
        var store = new DebugModuleStore();
        modules.AddRange(
            [
                new Anime.AllAnime.Module(),
                new Anime.Anilist.Module(),
                new MediaEngine.Mpv.Module(),
                new MediaEngine.Vlc.Module()
            ]);
#else
        var store = new ModuleStore();
#endif

        modules.AddRange(await store.LoadModules().ToListAsync());

#if WINDOWS
        //modules.Add(new MediaEngine.Flyleaf.Module());
#endif

        var builder = this.CreateBuilder(args)
            .Configure((host, window) => host
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
                                LogLevel.Debug :
                                LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Debug);

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
                    services.AddTransient(_ => window.Content!.XamlRoot!);
                    services.AddSingleton<IModuleStore>(store);
                    services.AddCoreServices();
                    services.AddAnimeServices();
                    services.AddNavigationView("Main");
                    services.AddTransient(sp =>
                    {
                        var openPicker = new FileOpenPicker();

                        if(OperatingSystem.IsWindows())
                        {
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

        MainWindow.Content = new MainPage
        {
            DataContext = ActivatorUtilities.CreateInstance<MainViewModel>(Host.Services)
        };

        await Host.StartAsync();

        MainWindow.Activate();

#if WINDOWS
        RxApp.MainThreadScheduler = new DispatcherQueueScheduler(MainWindow.DispatcherQueue);
        MainWindow.ExtendsContentIntoTitleBar = true;
#else
        RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
#endif

        await FFBinaries.DownloadLatest();
    }
}
