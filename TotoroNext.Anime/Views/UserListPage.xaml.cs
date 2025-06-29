using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.UserControls;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class UserListPage : Page
{
    private readonly Subject<(int Count, AnimeModel Anime)> _tappedSubject = new();
    private int _tappedCount;

    public UserListPage()
    {
        InitializeComponent();

        _tappedSubject
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .SelectMany(x =>
            {
                // double tap
                if(x.Count % 2 == 0)
                {
                    ViewModel?.PaneNavigator.NavigateToData(x.Anime);
                    return Observable.Return(Unit.Default);
                }
                else // single tap
                {
                    return (ViewModel?.NavigateToWatch(x.Anime) ?? Task.CompletedTask).ToObservable();
                }
            })
            .Subscribe(_ => _tappedCount = 0);
    }

    public UserListViewModel? ViewModel => DataContext as UserListViewModel;

    private void AnimeList_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if(args.Element is not ItemContainer container)
        {
            return;
        }

        if(container.Child is not AnimeCard card)
        {
            return;
        }

        card.UpdateBindings();
    }

    private void AnimeCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (SplitView.IsPaneOpen)
        {
            return;
        }

        if (sender is not AnimeCard { Anime: not null } card)
        {
            return;
        }

        _tappedSubject.OnNext(new(++_tappedCount, card.Anime));
    }

    private void AnimeCard_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if(SplitView.IsPaneOpen)
        {
            return;
        }

        if (sender is not AnimeCard { Anime: not null } card)
        {
            return;
        }

        _tappedSubject.OnNext(new(++_tappedCount, card.Anime));
    }

    private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if(SplitView.IsPaneOpen)
        {
            SplitView.IsPaneOpen = false;
            e.Handled = true;
        }
    }
}
