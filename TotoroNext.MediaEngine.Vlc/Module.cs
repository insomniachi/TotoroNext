using Microsoft.Extensions.DependencyInjection;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.MediaEngine.Vlc.ViewModels;
using TotoroNext.MediaEngine.Vlc.Views;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

public class Module : IModule<Settings>
{
    public Descriptor Descriptor { get; } = new()
    {
        Id = new Guid("a5c4c1d1-4669-4423-bb77-d5285776b5c9"),
        Name = "VLC Media Player",
        Description = "A module for integrating VLC media player into TotoroNext.",
        HeroImage = ResourceHelper.GetResource("vlc.jpeg"),
        Components = [ ComponentTypes.MediaEngine ],
        SettingViewModel = typeof(SettingsPageViewModel)
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddViewMap<SettingsPage, SettingsPageViewModel>();
        services.AddTransient(_ => Descriptor);
        services.AddModuleSettings(this);
        services.AddKeyedTransient<IMediaPlayerElementFactory, VlcMediaPlayerElementFactory>(Descriptor.Id);
    }
}

public class Settings
{
    public Settings()
    {
        if (OperatingSystem.IsLinux())
        {
            FileName = "/usr/bin/vlc"; // Default path for VLC on Linux
        }
        else
        {
            FileName = "";
        }
    }

    public string FileName { get; set; }
    public bool LaunchFullScreen { get; set; } = true;
}
