using System.Threading.Tasks;

namespace TotoroNext.Presentation;

public sealed partial class ModulesStorePage : Page
{
    public ModulesStorePage()
    {
        InitializeComponent();
        DataContextChanged += ModulesStorePage_DataContextChanged;
    }

    private async void ModulesStorePage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is ModulesStoreViewModel { } vm)
        {
            await vm.Initialize();
        }
    }

    public ModulesStoreViewModel? ViewModel => DataContext as ModulesStoreViewModel;

    private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {

    }
}
