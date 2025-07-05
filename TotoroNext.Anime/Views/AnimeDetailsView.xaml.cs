using Microsoft.UI.Xaml.Media.Animation;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class AnimeDetailsView : UserControl
{
    private int _lastSelectedIndex = 0;

    public AnimeDetailsView()
    {
        InitializeComponent();
        SetEdgeTransition(EdgeTransitionLocation.Left);
    }

    public AnimeDetailsViewModel? ViewModel => DataContext as AnimeDetailsViewModel;

    private void Pivot_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if(ViewModel is not { } vm)
        {
            return;
        }

        var selectedItem = sender.SelectedItem;
        int newIndex = sender.Items.IndexOf(selectedItem);
        if (newIndex == -1)
        {
            newIndex = 0;
        }
        var direction = newIndex > _lastSelectedIndex ? EdgeTransitionLocation.Right : EdgeTransitionLocation.Left;
        SetEdgeTransition(direction);
        _lastSelectedIndex = newIndex;

        switch (selectedItem?.Text)
        {
            case "Episodes":
                vm.Navigator?.NavigateToData(new EpisodesListViewModelNagivationParameters(vm.Anime));
                break;
            case "Related":
                vm.Navigator?.NavigateToData(vm.Anime.Related.ToList());
                break;
            case "Recommended":
                vm.Navigator?.NavigateToData(vm.Anime.Recommended.ToList());
                break;
            case "Overrides":
                vm.Navigator?.NavigateToData(new OverridesViewModelNavigationParameters(vm.Anime));
                break;
            case "Songs":
                vm.Navigator?.NavigateToData(new SongsViewModelNavigationParameters(vm.Anime));
                break;
            default:
                break;
        }
    }

    private void SetEdgeTransition(EdgeTransitionLocation location)
    {
        var transitions = DetailsContentControl.ContentTransitions;
        transitions.Clear();
        transitions.Add(new ContentThemeTransition());
#if WINDOWS
        transitions.Add(new EdgeUIThemeTransition { Edge = location });
#else
        transitions.Add(new EntranceThemeTransition());
#endif
    }
}
