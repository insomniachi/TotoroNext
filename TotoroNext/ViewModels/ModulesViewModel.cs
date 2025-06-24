using ReactiveUI;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class ModulesViewModel(IEnumerable<Descriptor> modules,
                                      IEvent<NavigateToViewModelRequest> request) : ReactiveObject, IPaneNavigatable
{
    public List<Descriptor> Descriptors { get; } = [.. modules];
    public INavigator PaneNavigator { get; set; } = null!;

    public void NavigateToSettings(Descriptor descriptor)
    {
        if (descriptor.SettingViewModel is not { } vmType)
        {
            return;
        }

        PaneNavigator.NavigateViewModel(vmType);
    }
}
