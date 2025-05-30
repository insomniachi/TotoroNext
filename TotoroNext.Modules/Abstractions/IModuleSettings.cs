namespace TotoroNext.Module.Abstractions;

public interface IModuleSettings<TData>
    where TData : class, new()
{
    TData Value { get; }
    void Save();
}

