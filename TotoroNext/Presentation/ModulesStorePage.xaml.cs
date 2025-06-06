using System.Threading.Tasks;
using TotoroNext.Module;

namespace TotoroNext.Presentation;

public sealed partial class ModulesStorePage : Page
{
    public ModulesStorePage()
    {
        InitializeComponent();
    }

    public ModulesStoreViewModel? ViewModel => DataContext as ModulesStoreViewModel;

    private async void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if(args.InvokedItem is ModuleManifest manifest && ViewModel is { } vm)
        {
            await vm.Download(manifest);
        }
    }
}
