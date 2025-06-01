using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using Flurl;

namespace TotoroNext.Module.Abstractions;

public interface IModuleStore
{
    IAsyncEnumerable<ModuleManifest> GetAllModules();
    Task<bool> DownloadModule(ModuleManifest manifest);
    IAsyncEnumerable<IModule> LoadModules();
}

public class DebugModuleStore : IModuleStore
{
    public IAsyncEnumerable<IModule> LoadModules() => AsyncEnumerable.Empty<IModule>();
    public Task<bool> DownloadModule(ModuleManifest manifest) => Task.FromResult(false);
    public IAsyncEnumerable<ModuleManifest> GetAllModules() => AsyncEnumerable.Empty<ModuleManifest>();
}


public class ModuleStore : IModuleStore
{
    private readonly HttpClient _client = new();
    private readonly string _url = "https://raw.githubusercontent.com/insomniachi/TotoroNext/refs/heads/master/manifest.json";
    private readonly string _modulesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", "Modules");

    public async IAsyncEnumerable<IModule> LoadModules()
    {
        var manifests = await GetAllModules().ToListAsync();

        foreach (var item in Directory.GetFiles(_modulesPath, "*.dll", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(item);

            if(!manifests.Any(x => x.EntryPoint == fileName))
            {
                continue;
            }

            var context = new ModuleLoadContext(item);
            var assembly = context.LoadFromAssemblyPath(item);
            var modules = assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IModule)) && !x.IsAbstract).ToList();

            if (modules.Count == 0)
            {
                context.Unload();
                continue;
            }

            foreach (var moduleType in modules)
            {
                yield return (IModule)Activator.CreateInstance(moduleType)!;
            }
        }
    }

    public async Task<bool> DownloadModule(ModuleManifest manifest)
    {
        try
        {
#if WINDOWS10_0_26100_0_OR_GREATER
            var targetFramework = "net9.0-windows10.0.26100";
#else
            var targetFramework = "net9.0-desktop";
#endif
            var destination = Path.Combine(_modulesPath, manifest.Name);
            var downloadUrl = Url.Combine(manifest.Versions[0].SourceUrl, targetFramework + ".zip");
            var stream = await _client.GetStreamAsync(downloadUrl);
            ZipFile.ExtractToDirectory(stream, destination, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async IAsyncEnumerable<ModuleManifest> GetAllModules()
    {
        var response = await _client.GetStringAsync(_url);
        var array = JsonNode.Parse(response)?.AsArray() ?? throw new InvalidOperationException("Failed to parse module manifest.");

        var manifests = array.Deserialize<List<ModuleManifest>>();
        
        foreach (var item in manifests ?? [])
        {
            yield return item;
        }
    }
}
