using Microsoft.UI.Xaml.Media.Animation;
using TotoroNext.Anime.ViewModels;
using TotoroNext.Module;

[assembly: Uno.Extensions.Markup.Generator.GenerateMarkupForAssembly(typeof(NavigationExtensions))]

namespace TotoroNext.Anime.Views;

public partial class UserListFilterView : UserControl
{
    public UserListFilterView()
    {
        this.DataContext<UserListViewModel>((view, vm) =>
        {
            view
            .NavigationExtensions(paneWidth: 500)
            .Content
            (   
                new Grid()
                .Children
                (
                    new StackPanel()
                    .Spacing(16)
                    .Padding(16)
                    .Children
                    (
                       [

                        new TextBlock()
                        .Text("Filter")
                        .Style(x => x.ThemeResource("TitleTextBlockStyle")),

                        new AutoSuggestBox()
                        .PlaceholderText("Name")
                        .Text(x => x
                            .Binding(() => vm.Filter.Term)
                            .TwoWay()
                            .UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                        .QueryIcon(new FontIcon().Glyph("\uE8AC")),

                        new AutoSuggestBox()
                        .PlaceholderText("Year")
                        .Text(x => x
                            .Binding(() => vm.Filter.Year)
                            .TwoWay()
                            .UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                        .QueryIcon(new FontIcon().Glyph("\uE787"))

                        ]
                    )
                )
            );
        });
    }
}
