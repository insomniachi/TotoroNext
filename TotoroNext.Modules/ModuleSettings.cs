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
        _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", "Modules", descriptor.EntryPoint, $"{descriptor.Id}.json");

        if(File.Exists(_filePath) && JsonSerializer.Deserialize<TDtata>(File.ReadAllText(_filePath)) is { } data)
        {
            Value = data;
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            Value = new TDtata();
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

    protected void SetAndSaveProperty<TProperty>(ref TProperty field, TProperty value, [CallerMemberName]string propertyName = "")
    {
        if(SetProperty(ref field, value, propertyName))
        {
            data.Save();
        }
    }
}
