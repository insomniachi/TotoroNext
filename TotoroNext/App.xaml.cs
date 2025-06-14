using System.Reactive.Concurrency;
using ReactiveUI;
using TotoroNext.Anime;
using TotoroNext.Anime.Abstractions;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using TotoroNext.ViewModels;
using TotoroNext.Views;
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
#else
        var store = new ModuleStore();
#endif

        modules.AddRange(await store.LoadModules().ToListAsync());

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
                .UseHttp()
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

                        if (OperatingSystem.IsWindows())
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

                    services.RegisterFactory<ITrackingService>(nameof(SettingsModel.SelectedTrackingService))
                            .RegisterFactory<IMediaPlayer>(nameof(SettingsModel.SelectedMediaEngine))
                            .RegisterFactory<IMetadataService>(nameof(SettingsModel.SelectedTrackingService))
                            .RegisterFactory<IAnimeProvider>(nameof(SettingsModel.SelectedAnimeProvider))
                            .RegisterFactory<IMediaSegmentsProvider>(nameof(SettingsModel.SelectedSegmentsProvider));

                    services.AddMainNavigationViewItem<ModulesPage, ModulesViewModel>("My Modules", new FontIcon { Glyph = "\uED35" }, true);
                    services.AddMainNavigationViewItem<ModulesStorePage, ModulesStoreViewModel>("Store", new FontIcon { Glyph = "\uEA40" });
                    services.AddTransient<ViewMap>(_ => new ViewMap<SettingsPage, SettingsViewModel>());
                    services.AddSingleton<SettingsViewModel>();
                    services.AddTransient(sp =>
                    {
                        var vm = sp.GetRequiredService<SettingsViewModel>();
                        vm.Initialize();
                        return vm.Settings;
                    });
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

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterFactory<TService>(this IServiceCollection services, string key)
        where TService : notnull
    {
        services.AddTransient<IFactory<TService, Guid>, Factory<TService, Guid>>(sp =>
        {
            var factory = sp.GetRequiredService<IServiceScopeFactory>();
            var settings = sp.GetRequiredService<ILocalSettingsService>();
            return new Factory<TService, Guid>(factory, settings, key);
        });

        return services;
    }
}


public class DebugModuleStore : IModuleStore
{
    public async IAsyncEnumerable<IModule> LoadModules()
    {
        await Task.CompletedTask;

        // Anime Providers
        yield return new Anime.AllAnime.Module();

        // Anime Tracking/Metadata
        yield return new Anime.Anilist.Module();

        // Misc
        yield return new Anime.Aniskip.Module();
        yield return new Discord.Module();

        // Media Players
        yield return new MediaEngine.Mpv.Module();
        yield return new MediaEngine.Vlc.Module();

    }

    public Task<bool> DownloadModule(ModuleManifest manifest) => Task.FromResult(false);
    public IAsyncEnumerable<ModuleManifest> GetAllModules() => AsyncEnumerable.Empty<ModuleManifest>();
}
