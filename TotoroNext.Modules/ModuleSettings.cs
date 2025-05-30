using System.Text.Json;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;


internal class ModuleSettings<TDtata> : IModuleSettings<TDtata>
    where TDtata : class, new()
{
    private readonly string _filePath;

    internal ModuleSettings(Guid id)
    {
        _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", "Modules", $"{id}.json");

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

