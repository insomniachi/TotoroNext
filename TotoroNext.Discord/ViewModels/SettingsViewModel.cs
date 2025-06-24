using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Discord.ViewModels;

internal partial class SettingsViewModel(IModuleSettings<Settings> data) : ModuleSettingsViewModel<Settings>(data)
{
    public bool IsEnabled
    {
        get;
        set => SetAndSaveProperty(ref field, value, x => x.IsEnabled = value);
    } = data.Value.IsEnabled;
}
