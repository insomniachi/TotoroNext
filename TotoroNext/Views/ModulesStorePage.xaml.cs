using TotoroNext.Module;

namespace TotoroNext.Presentation;

public sealed partial class ModulesStorePage : Page
{
    public ModulesStorePage()
    {
        InitializeComponent();
    }

    public ModulesStoreViewModel? ViewModel => DataContext as ModulesStoreViewModel;
}
