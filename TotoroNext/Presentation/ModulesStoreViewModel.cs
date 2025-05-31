using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class ModulesStoreViewModel(IModuleStore store) : ReactiveObject
{
    [Reactive]
    public partial List<ModuleManifest> Modules { get; set; }

    public async Task Initialize()
    {
        Modules = await store.GetAllModules().ToListAsync();
    }

    public async Task Download(ModuleManifest manifest)
    {
        await store.DownloadModule(manifest);
    }
}
