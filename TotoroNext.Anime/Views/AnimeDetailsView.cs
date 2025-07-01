using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media.Animation;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Anime.ViewModels.Parameters;
using TotoroNext.Module;

namespace TotoroNext.Anime.Views;

public partial class AnimeDetailsView : UserControl
{
    private int _lastSelectedIndex = 0;
    private readonly ContentControl? _contentFrame;

    public AnimeDetailsView()
    {
        this.DataContext<AnimeDetailsViewModel>((view, vm) =>
        {
            view
            .NavigationExtensions(paneWidth: 750)
            .Content(new Grid()
                .RowDefinitions([
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { MaxHeight = 435 },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                ])
                .Padding(16)
                .RowSpacing(8)
                .Children([
                    new TextBlock()
                    .Grid(row: 0)
                    .Margin(0,0,0,8)
                    .Style(x => x.ThemeResource("TitleTextBlockStyle"))
                    .Text(x => x.Binding(() => vm.Anime.Title)),

                    new Grid()
                    .Grid(row:1)
                    .ColumnDefinitions([
                        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(1.25, GridUnitType.Star) }
                    ])
                    .ColumnSpacing(24)
                    .Children([
                        new Border()
                        .CornerRadius(x => x.StaticResource("ControlCornerRadius"))
                        .Grid(column: 0)
                        .Child(new Image()
                            .Source(x => x.Binding(() => vm.Anime.Image))),

                        new Grid()
                        .Grid(column: 1)
                        .ColumnSpacing(16)
                        .RowSpacing(16)
                        .ColumnDefinitions("Auto,*")
                        .RowDefinitions("Auto,Auto,Auto,Auto,Auto,*")
                        .Children([
                            new TextBlock()
                            .Grid(row: 0, column: 0)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Text("Status"),
                            new ComboBox()
                            .Grid(row: 0, column: 1)
                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                            .ItemsSource(x => x.Binding(() => vm.Statuses))
                            .SelectedItem(x => x.Binding(() => vm.Status).TwoWay()),

                            new TextBlock()
                            .Grid(row: 1, column: 0)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Text("Progress"),
                            new NumberBox()
                            .Grid(row: 1, column: 1)
                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                            .LargeChange(1)
                            .SmallChange(1)
                            .SpinButtonPlacementMode(NumberBoxSpinButtonPlacementMode.Inline)
                            .Value(x => x.Binding(() => vm.Progress).TwoWay()),

                            new TextBlock()
                            .Grid(row: 2, column: 0)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Text("Score"),
                            new NumberBox()
                            .Grid(row: 2, column: 1)
                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                            .LargeChange(1)
                            .SmallChange(1)
                            .Maximum(10)
                            .SpinButtonPlacementMode(NumberBoxSpinButtonPlacementMode.Inline)
                            .Value(x => x.Binding(() => vm.Score).TwoWay()),

                            new TextBlock()
                            .Grid(row: 3, column: 0)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Text("Start Date"),
                            new DatePicker()
                            .Grid(row: 3, column: 1)
#if WINDOWS
                            .Name(out DatePicker _, dp => DatePickerNullWorkAround(dp, vm => vm.StartDate))
#endif
                            .SelectedDate(x => x.Binding(() => vm.StartDate).TwoWay()),

                            new TextBlock()
                            .Grid(row: 4, column: 0)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Text("Finish Date"),
                            new DatePicker()
                            .Grid(row: 4, column: 1)
#if WINDOWS
                            .Name(out DatePicker _, dp => DatePickerNullWorkAround(dp, vm => vm.FinishDate))
#endif
                            .SelectedDate(x => x.Binding(() => vm.FinishDate).TwoWay()),

                            new ScrollView()
                            .Grid(row: 5, column: 0, columnSpan: 2)
                            .Margin(0, 8, 0, 0)
                            .HorizontalScrollMode(ScrollingScrollMode.Disabled)
                            .Content(new TextBlock()
                                .TextWrapping(TextWrapping.Wrap)
                                .Text(x => x.Binding(() => vm.Anime.Description)))
                        ])
                    ]),

                    new SelectorBar()
                    .Grid(row: 2)
                    .Items([
                        BaseSelectorBar().Text("Episodes").IsSelected(true),
                        BaseSelectorBar().Text("Related"),
                        BaseSelectorBar().Text("Recommended"),
                    ])
                    .Name(out SelectorBar _, selector =>
                    {
                        selector.SelectionChanged += (s, _) => PivotSelectionChanged(s);
                    }),

                    new ContentControl()
                    .Grid(row: 3)
                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                    .VerticalAlignment(VerticalAlignment.Stretch)
                    .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                    .VerticalContentAlignment(VerticalAlignment.Stretch)
                    .NavigationExtensions(isAttached: true)
                    .ContentTransitions(new TransitionCollection())
                    .Name(out ContentControl _contentFrame, c => SetEdgeTransition(c, EdgeTransitionLocation.Right))

                ]));
        });
    }

    private static SelectorBarItem BaseSelectorBar() => new SelectorBarItem().FontSize(25).FontWeight(FontWeights.SemiLight);

    private static void SetEdgeTransition(ContentControl? control, EdgeTransitionLocation location)
    {
        if(control is null)
        {
            return;
        }

        var transitions = control.ContentTransitions;
        transitions.Clear();
        transitions.Add(new ContentThemeTransition());
#if WINDOWS
        transitions.Add(new EdgeUIThemeTransition { Edge = location });
#else
        transitions.Add(new EntranceThemeTransition());
#endif
    }


    private void PivotSelectionChanged(SelectorBar sender)
    {
        if (DataContext is not AnimeDetailsViewModel { } vm)
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
        SetEdgeTransition(_contentFrame, direction);
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
            default:
                break;
        }
    }

#if WINDOWS
    private static void DatePickerNullWorkAround(DatePicker dp, Func<AnimeDetailsViewModel, DateTimeOffset?> getter)
    {
        dp.Loaded += (_, _) =>
        {
            if (dp.DataContext is not AnimeDetailsViewModel { } viewModel)
            {
                return;
            }

            if (getter(viewModel) is null)
            {
                dp.SelectedDate = null;
            }
        };
    }
#endif

}
