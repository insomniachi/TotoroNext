// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class UserListFilterView : UserControl
{
	public UserListFilterView()
	{
		InitializeComponent();
	}

    public UserListFilterViewModel? ViewModel => DataContext as UserListFilterViewModel;
}
