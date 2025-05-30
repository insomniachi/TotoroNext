namespace TotoroNext.Module;

public class Descriptor
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Version Version { get; init; }
    public List<string> Components { get; init; } = [];
    public string? Description { get; init; }
    public string? HeroImage { get; init; }
    public Type? SettingViewModel { get; init; }
}
