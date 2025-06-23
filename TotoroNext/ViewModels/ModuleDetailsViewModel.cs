using TotoroNext.Module;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module.Abstractions;
using ReactiveUI;
using System.Reactive.Linq;

namespace TotoroNext.ViewModels;

public partial class ModuleDetailsViewModel(ModuleManifest manifest,
                                            IModuleStore store, 
                                            IEnumerable<Descriptor> descriptors) : ReactiveObject, IInitializable
{
    public ModuleManifest Manifest { get; } = manifest;

    [Reactive]
    public partial bool IsDownloading { get; set; }

    [Reactive]
    public partial string DownloadButtonText { get; set; }

    [Reactive]
    public partial bool CanDownload { get; set; }


    public void Initialize()
    {
        if(Manifest is null)
        {
            return;
        }

        IsDownloading = false;
        CanDownload = CanDownloadManifest(Manifest);
    }

    [ReactiveCommand(CanExecute = nameof(CanDownload))]
    private async Task Download(ModuleManifest manifest)
    {
        IsDownloading = true;
        await store.DownloadModule(manifest);
        IsDownloading = false;
        CanDownload = false;
    }

    private bool CanDownloadManifest(ModuleManifest? manifest)
    {
        if (manifest is null)
        {
            return false;
        }

        if (descriptors.FirstOrDefault(x => x.Id == Guid.Parse(manifest.Id)) is not { } installedModule)
        {
            DownloadButtonText = "Download";
            return true;
        }

        var isNewVersion = Version.Parse(manifest.Versions[0].Version) > installedModule.Version;

        if(isNewVersion)
        {
            DownloadButtonText = "Update";
        }

        return isNewVersion;
    }
}
