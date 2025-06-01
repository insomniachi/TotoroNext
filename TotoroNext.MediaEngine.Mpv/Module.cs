using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.MediaEngine.Mpv.ViewModels;
using TotoroNext.MediaEngine.Mpv.Views;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using Path = System.IO.Path;

namespace TotoroNext.MediaEngine.Mpv;

public class Module : IModule<ModuleSettings>
{
    public Descriptor Descriptor { get; } = new()
    {
        Id = new Guid("b8c3f0d2-1c5e-4f6a-9b7d-3f8e1c5f0d2a"),
        Name = "MPV Media Player",
        Description = "A module for integrating MPV media player into TotoroNext.",
        HeroImage = ResourceHelper.GetResource("mpv.jpeg"),
        Components = [ ComponentTypes.MediaEngine ],
        SettingViewModel = typeof(SettingsPageViewModel)
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddViewMap<SettingsPage, SettingsPageViewModel>();
        services.AddTransient(_ => Descriptor);
        services.AddModuleSettings(this);
        services.AddKeyedTransient<IMediaPlayerElementFactory, MpvMediaPlayerElementFactory>(Descriptor.Id);
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
