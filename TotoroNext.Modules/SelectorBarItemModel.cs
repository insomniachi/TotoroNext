namespace TotoroNext.Module;

public class SelectorBarItemModel
{
    private SelectorBarItemModel(string name, Type vmType)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ViewModelType = vmType ?? throw new ArgumentNullException(nameof(vmType));
    }

    public string Name { get; }
    public Type ViewModelType { get; }

    public static SelectorBarItemModel CreateFromViewModel<T>(string name) where T : class
    {
        return new SelectorBarItemModel(name, typeof(T));
    }
}
