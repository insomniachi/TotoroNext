using CommunityToolkit.Mvvm.Messaging;
using ReactiveUI;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.ViewModels;

public partial class ModulesViewModel(IEnumerable<Descriptor> modules, IMessenger messenger) : ReactiveObject
{
    public List<Descriptor> Descriptors { get; } = [.. modules.Where(x => !x.IsInternal)];

    public void NavigateToSettings(Descriptor descriptor)
    {
        if (descriptor.SettingViewModel is not { } vmType)
        {
            return;
        }

        messenger.Send(new PaneNavigateToViewModelMessage(vmType, 600));
    }
}
