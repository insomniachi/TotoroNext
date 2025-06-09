
using TotoroNext.ViewModels;

namespace TotoroNext.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    public SettingsViewModel? ViewModel => DataContext as SettingsViewModel;
}
