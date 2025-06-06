using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class ModulesStoreViewModel(IModuleStore store,
                                           IEnumerable<Descriptor> descriptors) : ReactiveObject, IAsyncInitializable
{
    [Reactive]
    public partial List<ModuleManifest> Modules { get; set; }

    public async Task InitializeAsync()
    {
        Modules = await store.GetAllModules().ToListAsync();
    }

    public async Task Download(ModuleManifest manifest)
    {
        if(!CanDownload(manifest))
        {
            return;
        }

        await store.DownloadModule(manifest);
    }

    private bool CanDownload(ModuleManifest manifest)
    {
        if(descriptors.FirstOrDefault(x => x.Id == Guid.Parse(manifest.Id)) is not { } installedModule)
        {
            return true;
        }

        return Version.Parse(manifest.Versions[0].Version) > installedModule.Version;
    }
}
