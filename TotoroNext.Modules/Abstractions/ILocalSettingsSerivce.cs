namespace TotoroNext.Module.Abstractions;

public interface ILocalSettingsService
{
    T? ReadSetting<T>(string key, T? deafultValue = default);
    void SaveSetting<T>(string key, T value);
    void RemoveSetting(string key);
}
