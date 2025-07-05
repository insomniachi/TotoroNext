
using TotoroNext.Module;
using TotoroNext.ViewModels;

namespace TotoroNext.Views;

public sealed partial class ModulesPage : Page
{
    public ModulesPage()
    {
        InitializeComponent();
    }

    public ModulesViewModel? ViewModel => DataContext as ModulesViewModel;

    private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is Descriptor d)
        {
            ViewModel?.NavigateToSettings(d);
        }
    }

}
