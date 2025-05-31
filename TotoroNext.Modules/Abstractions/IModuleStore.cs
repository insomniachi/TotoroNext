using System.Text.Json;
using System.Text.Json.Nodes;

namespace TotoroNext.Module.Abstractions;

public interface IModuleStore
{
    IAsyncEnumerable<ModuleManifest> GetAllModules();
}


public class ModuleStore(IHttpClientFactory httpClientFactory) : IModuleStore
{
    private readonly string _url = "https://raw.githubusercontent.com/insomniachi/TotoroNext/refs/heads/master/manifest.json";

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
