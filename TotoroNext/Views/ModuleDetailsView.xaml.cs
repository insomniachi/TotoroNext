using TotoroNext.ViewModels;

namespace TotoroNext.Views;
public sealed partial class ModuleDetailsView : UserControl
{
    public ModuleDetailsView()
    {
        InitializeComponent();
    }

    public ModuleDetailsViewModel? ViewModel => DataContext as ModuleDetailsViewModel;
}
