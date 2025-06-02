using System.Reactive.Linq;
using ReactiveUI;
using TotoroNext.Anime.ViewModels;


namespace TotoroNext.Anime.Views;

public sealed partial class WatchPage : Page
{
    public WatchPage()
    {
        InitializeComponent();

        DataContextChanged += PageDataContextChanged;
    }
    private void PageDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is WatchViewModel vm)
        {
            vm.Initialize();
        }
    }

    public WatchViewModel? ViewModel => DataContext as WatchViewModel;
}

