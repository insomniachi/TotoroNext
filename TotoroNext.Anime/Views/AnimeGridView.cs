using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.UserControls;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public partial class AnimeGridView : UserControl
{
    public AnimeGridView()
    {
        this.DataContext<AnimeGridViewModel>((view, vm) =>
        {
            view.Content(new ScrollView()
                .Content(new ItemsRepeater()
                .ItemsSource(x => x.Binding(() => vm.Items).OneWay())
                .ItemTemplate<AnimeModel>(item =>
                {
                    return new AnimeCard().Anime(() => item);
                })
                .Layout(new UniformGridLayout
                {
                    ItemsJustification = UniformGridLayoutItemsJustification.Start,
                    ItemsStretch = UniformGridLayoutItemsStretch.Fill,
                    MaximumRowsOrColumns = 3,
                    MinColumnSpacing = 16,
                    MinRowSpacing = 16,
                    MinItemHeight = 300,
                    MinItemWidth = 125
                })));
        });
    }
}
