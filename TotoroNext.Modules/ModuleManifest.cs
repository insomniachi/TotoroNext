namespace TotoroNext.Module;

public class ModuleManifest
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Owner { get; set; }
    public required string[] Categories { get; set; } = [];
    public required VersionInfo[] Versions { get; set; }
}

public class VersionInfo
{
    public required string Version { get; set; }
    public string? Changelong { get; set; }
    public required string TargetVersion { get; set; }
    public required string SourceUrl { get; set; }
}
