
namespace TotoroNext.Presentation;

public sealed partial class ModulesPage : Page
{
    public ModulesPage()
    {
        InitializeComponent();
    }

    public ModulesViewModel? ViewModel => DataContext as ModulesViewModel;
}
