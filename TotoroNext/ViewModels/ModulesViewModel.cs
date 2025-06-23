using ReactiveUI;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Presentation;

public partial class ModulesViewModel(IEnumerable<Descriptor> modules,
                                      IEvent<NavigateToViewModelRequest> request) : ReactiveObject
{
    public List<Descriptor> Descriptors { get; } = [.. modules];

    public void NavigateToSettings(Descriptor descriptor)
    {
        if (descriptor.SettingViewModel is not { } vmType)
        {
            return;
        }

        request.Publish(new NavigateToViewModelRequest(vmType));
    }
}
