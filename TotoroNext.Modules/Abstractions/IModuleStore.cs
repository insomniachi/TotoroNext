using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TotoroNext.Module.Abstractions;

public interface IModuleStore
{
    IAsyncEnumerable<ModuleManifest> GetAllModules();
    Task<bool> DownloadModule(ModuleManifest manifest);
}


public class ModuleStore(IHttpClientFactory httpClientFactory) : IModuleStore
{
    private readonly string _url = "https://raw.githubusercontent.com/insomniachi/TotoroNext/refs/heads/master/manifest.json";

    public async Task<bool> DownloadModule(ModuleManifest manifest)
    {
        using var client = httpClientFactory.CreateClient();
        try
        {
            var destination = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", "Modules", manifest.Name);
            var stream = await client.GetStreamAsync(manifest.Versions[0].SourceUrl);
            var path = Path.Combine(Path.GetTempPath(), manifest.Name, manifest.Versions[0].Version + ".zip");
            using var fs = new FileStream(path, FileMode.OpenOrCreate);
            await stream.CopyToAsync(fs);
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
        using var client = httpClientFactory.CreateClient();
        var response = await client.GetStringAsync(_url);
        var array = JsonNode.Parse(response)?.AsArray() ?? throw new InvalidOperationException("Failed to parse module manifest.");

        var manifests = array.Deserialize<List<ModuleManifest>>();
        
        foreach (var item in manifests ?? [])
        {
            yield return item;
        }
    }
}
