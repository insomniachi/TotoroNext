using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TotoroNext.Module;
using Windows.Storage.Pickers;

namespace TotoroNext.MediaEngine.Mpv.ViewModels;

public partial class SettingsPageViewModel: ObservableObject
{
    private readonly IModuleSettings<ModuleSettings> _settings;
    private readonly FileOpenPicker _picker;

    public SettingsPageViewModel(IModuleSettings<ModuleSettings> settings,
                                 FileOpenPicker picker)
    {
        _settings = settings;
        _picker = picker;

        picker.FileTypeFilter.Add("*");
    }

    public string Command
    {
        get => _settings.Value.FileName;
        set
        {
            if (_settings.Value.FileName != value)
            {
                _settings.Value.FileName = value;
                OnPropertyChanged();
                _settings.Save();
            }
        }
    }

    public bool LaunchFullScreen
    {
        get => _settings.Value.LaunchFullScreen;
        set
        {
            if (_settings.Value.LaunchFullScreen != value)
            {
                _settings.Value.LaunchFullScreen = value;
                OnPropertyChanged();
                _settings.Save();
            }
        }
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
