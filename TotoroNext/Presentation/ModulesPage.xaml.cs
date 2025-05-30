
using TotoroNext.Module;

namespace TotoroNext.Presentation;

public sealed partial class ModulesPage : Page
{
    public ModulesPage()
    {
        InitializeComponent();
    }

    public ModulesViewModel? ViewModel => DataContext as ModulesViewModel;

    private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if(args.InvokedItem is Descriptor d)
        {
            ViewModel?.NavigateToSettings(d);
        }
    }
}
