using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using TotoroNext.Module.Abstractions;
using Path = System.IO.Path;

namespace TotoroNext.Module;


internal class ModuleSettings<TDtata> : IModuleSettings<TDtata>
    where TDtata : class, new()
{
    private readonly string _filePath;

    internal ModuleSettings(Descriptor descriptor)
    {
        _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", "Modules", descriptor.EntryPoint, $"settings.json");

        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        Value = new TDtata();

        if (File.Exists(_filePath))
        {
            var text = File.ReadAllText(_filePath);
            this.Log().LogDebugMessage(text);
            if(JsonSerializer.Deserialize<TDtata>(text) is { } data)
            {
                Value = data;
            }
        }
    }

    public TDtata Value { get; private set; }

    public void Save()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(Value));
    }
}


public abstract class ModuleSettingsViewModel<TSettings>(IModuleSettings<TSettings> data) : ObservableObject
    where TSettings : class, new()
{
    protected TSettings Settings => data.Value;

    protected void SetAndSaveProperty<TProperty>(ref TProperty field, TProperty value, Action<TSettings> settingUpdate, [CallerMemberName]string propertyName = "")
    {
        if(SetProperty(ref field, value, propertyName))
        {
            settingUpdate(data.Value);
            data.Save();
        }
    }
}

public class ResourceHelper
{
    public static string GetResource(string name)
    {

#if WINDOWS
        var targetFramework = "net9.0-windows10.0.26100";
#else
        var targetFramework = "net9.0-desktop";
#endif

        return new Uri($"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                       "TotoroNext", 
                       "Modules", 
                       Assembly.GetCallingAssembly().GetName().Name ?? "", 
                       targetFramework,
                       "Assets",
                       name)}").AbsoluteUri;
    }
}
