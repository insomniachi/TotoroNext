using CommunityToolkit.Mvvm.Input;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;
using Windows.Storage.Pickers;

namespace TotoroNext.MediaEngine.Vlc.ViewModels;

public partial class SettingsPageViewModel: ModuleSettingsViewModel<Settings>
{
    private readonly FileOpenPicker _picker;

    public SettingsPageViewModel(IModuleSettings<Settings> settings,
                                 FileOpenPicker picker) : base(settings)
    {
        _picker = picker;

        picker.FileTypeFilter.Add("*");
        Command = Settings.FileName;
        LaunchFullScreen = Settings.LaunchFullScreen;
    }

    public string? Command
    {
        get;
        set => SetAndSaveProperty(ref field, value, x => x.FileName = value ?? "");
    }

    public bool LaunchFullScreen
    {
        get;
        set => SetAndSaveProperty(ref field, value, x => x.LaunchFullScreen = value);
    }

    [RelayCommand]
    private async Task PickFileAsync()
    {
        var file = await _picker.PickSingleFileAsync();

        if(file is null)
        {
            return;
        }

        Command = file.Path;
    }
}
