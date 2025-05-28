using Microsoft.Extensions.DependencyInjection;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.MediaEngine.Mpv.ViewModels;
using TotoroNext.MediaEngine.Mpv.Views;
using TotoroNext.Module;

namespace TotoroNext.MediaEngine.Mpv;

public class Module : IModule<ModuleSettings>
{
    public Guid Id { get; } = new("b8c3f0d2-1c5e-4f6a-9b7d-3f8e1c5f0d2a");

    public void ConfigureNavigation(NavigationViewContext context) 
    {
        context.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddModuleSettings(this);
        services.AddNavigationViewItem<SettingsPageViewModel>("Mpv", new SymbolIcon { Symbol = Symbol.Play });
        services.AddKeyedTransient<IMediaPlayerElementFactory, MpvMediaPlayerElementFactory>("MPV");
    }
}

public class ModuleSettings
{
    public ModuleSettings()
    {
        if (OperatingSystem.IsLinux())
        {
            FileName = "/usr/bin/mpv"; // Default path for MPV on Linux
        }
        else
        {
            FileName = "";
        }
    }

    public string FileName { get; set; }
    public bool LaunchFullScreen { get; set; } = true;
}
