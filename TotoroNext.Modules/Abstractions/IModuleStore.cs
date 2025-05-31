namespace TotoroNext.Module.Abstractions;

public interface IModuleStore
{
    IEnumerable<ModuleManifest> GetAllModules();
}


public class ModuleStore(IHttpClientFactory httpClientFactory) : IModuleStore
{
    public async IAsyncEnumerable<ModuleManifest> GetAllModules()
    {
        using var client = httpClientFactory.CreateClient();
    }
}
