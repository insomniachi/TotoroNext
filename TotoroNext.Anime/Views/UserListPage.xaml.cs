using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class UserListPage : Page
{
    public UserListPage()
    {
        InitializeComponent();

        DataContextChanged += UserListPage_DataContextChanged;
    }

    private async void UserListPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is UserListViewModel vm)
        {
            await vm.Initialize();
        }
    }

    public UserListViewModel? ViewModel => DataContext as UserListViewModel;
}
