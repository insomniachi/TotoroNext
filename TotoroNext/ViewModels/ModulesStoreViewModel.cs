using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class ModulesStoreViewModel(IModuleStore store,
                                           IEnumerable<Descriptor> descriptors) : ReactiveObject, IInitializable
{
    [Reactive]
    public partial List<ModuleManifest> Modules { get; set; }

    public void Initialize()
    {
        RxApp.MainThreadScheduler.Schedule(async () =>
        {
            Modules = await store.GetAllModules().ToListAsync();
        });

        this.WhenAnyValue(x => x.SelectedModule)
            .WhereNotNull()
            .Subscribe(_ => IsPaneOpen = true);
    }

    [Reactive]
    public partial ModuleManifest? SelectedModule { get; set; }

    [Reactive]
    public partial bool IsPaneOpen { get; set; }

    [Reactive]
    public partial bool IsDownloading { get; set; }


    [ReactiveCommand(CanExecute = nameof(CanDownloadObservable))]
    private async Task Download(ModuleManifest manifest)
    {
        IsDownloading = true;
        await store.DownloadModule(manifest);
        IsDownloading = false;
    }

    private IObservable<bool> CanDownloadObservable => this.WhenAnyValue(x => x.SelectedModule).Select(CanDownload);

    private bool CanDownload(ModuleManifest? manifest)
    {
        if (manifest is null)
        {
            return false;
        }

        if (descriptors.FirstOrDefault(x => x.Id == Guid.Parse(manifest.Id)) is not { } installedModule)
        {
            return true;
        }

        return Version.Parse(manifest.Versions[0].Version) > installedModule.Version;
    }

}
