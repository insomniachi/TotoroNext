using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public partial class SearchMetadataProviderPage : Page
{
    public SearchMetadataProviderPage()
    {
        this.DataContext<SearchMetadataProviderViewModel>((page, vm) =>
        {
            page.Content(new Grid()
                .Margin(36)
                .RowSpacing(16)
                .RowDefinitions("Auto,*")
                .Children([

                    new AutoSuggestBox()
                    .PlaceholderText("Search")
                    .QueryIcon(new SymbolIcon().Symbol(Symbol.Find))
                    .Text(x => x.Binding(() => vm.Query).TwoWay()),

                    new ScrollView()
                    .Grid(row: 1)
                    .Content(new ItemsRepeater()
                        .ItemsSource(x => x.Binding(() => vm.Items).OneWay())
                        .ItemTemplate<AnimeModel>(item => new Grid()
                            .RowDefinitions("*,Auto")
                            .Children([

                                    new Image()
                                    .Source(x => x
                                        .Binding(() => item.Image)
                                        .Convert(Converters.StringToImage))
                                    .Stretch(Stretch.Fill),

                                    new Border()
                                    .Grid(row:1)
                                    .Height(60)
                                    .Padding(3)
                                    .BorderThickness(0,4,0,0)
                                    .Background(x => x.ThemeResource("CardBackgroundFillColorDefaultBrush"))
                                    .Child(new TextBlock()
                                        .FontSize(15)
                                        .TextAlignment(TextAlignment.Center)
                                        .TextTrimming(TextTrimming.WordEllipsis)
                                        .TextWrapping(TextWrapping.NoWrap)
                                        .ToolTipService(x => x.ToolTip(() => item.Title))
                                        .VerticalAlignment(VerticalAlignment.Center)
                                        .Text(x => x.Binding(() => item.Title)))

                            ])
                        )
                        .Layout(new UniformGridLayout
                        {
                            ItemsJustification = UniformGridLayoutItemsJustification.Start,
                            ItemsStretch= UniformGridLayoutItemsStretch.Fill,
                            MinColumnSpacing = 16,
                            MinItemHeight = 400,
                            MinItemWidth = 250,
                            MinRowSpacing = 16 
                        })
                        .CommandExtensions(x => x.Command(() => vm.ItemInvokedCommand))
                    )

                ]));
        });
    }
}
