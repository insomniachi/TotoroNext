using TotoroNext.MediaEngine.Vlc.ViewModels;

namespace TotoroNext.MediaEngine.Vlc.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    public SettingsPageViewModel? ViewModel => DataContext as SettingsPageViewModel;
}
