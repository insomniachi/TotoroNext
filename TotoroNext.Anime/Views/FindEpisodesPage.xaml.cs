using ReactiveUI;
using TotoroNext.Anime.ViewModels;


namespace TotoroNext.Anime.Views;

public sealed partial class FindEpisodesPage : Page
{
    public FindEpisodesPage()
    {
        InitializeComponent();

        DataContextChanged += PageDataContextChanged;
    }
    private void PageDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if(args.NewValue is FindEpisodesViewModel vm)
        {
            vm.Initialize();

            Host.WhenAnyValue(x => x.Player)
                .WhereNotNull()
                .Subscribe(x => vm.MediaPlayer = x);
        }
    }

    public FindEpisodesViewModel? ViewModel => DataContext as FindEpisodesViewModel;
}

