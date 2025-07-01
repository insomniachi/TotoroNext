using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.UserInteractions;


internal abstract class SelectResult<T>(XamlRoot xamlRoot) : ISelectionUserInteraction<T>
    where T : class
{
    public async Task<T?> GetValue(List<T> input)
    {
        var dialog = new ContentDialog
        {
            Title = GetTitle(),
            CloseButtonText = "Close",
            PrimaryButtonText = "Select",
            DefaultButton = ContentDialogButton.Primary,
            Content = new ListView()
                .ItemsSource(input)
                .Name(out var listView)
                .SelectionMode(ListViewSelectionMode.Single)
                .ItemTemplate<T>(CreateElement),
            XamlRoot = xamlRoot
        };

        var result = await dialog.ShowAsync();

        return result is ContentDialogResult.Primary
            ? listView.SelectedItem as T
            : null;
    }

    public abstract UIElement CreateElement(T model);
    public virtual string GetTitle() => "Select";
    public virtual string GetCloseButtonText() => "Close";
    public virtual string GetPrimaryButtonText() => "Select";
}

internal class SelectProviderResult(XamlRoot xamlRoot) : SelectResult<SearchResult>(xamlRoot)
{
    public override UIElement CreateElement(SearchResult model)
    {
        return new Grid()
            .Margin(8)
            .ColumnDefinitions("Auto,*")
            .ColumnSpacing(8)
            .Children(
            [
                new Image()
                .Source(() => model.Image, x => Converters.UriToImage(x)!)
                .Height(100).Width(75)
                .Stretch(Stretch.UniformToFill)
                .Grid(column: 0),

                new TextBlock()
                .Text(() => model.Title)
                .VerticalAlignment(VerticalAlignment.Center)
                .TextWrapping(TextWrapping.WrapWholeWords)
                .Grid(column: 1)
            ]);
    }
}

internal class SelectAnimeResult(XamlRoot xamlRoot) : SelectResult<AnimeModel>(xamlRoot)
{
    public override UIElement CreateElement(AnimeModel model)
    {
        return new Grid()
            .Margin(8)
            .ColumnDefinitions("Auto,*")
            .ColumnSpacing(8)
            .Children(
            [
                new Image()
                .Source(() => model.Image, x => Converters.StringToImage(x)!)
                .Height(100).Width(75)
                .Stretch(Stretch.UniformToFill)
                .Grid(column: 0),

                new TextBlock()
                .Text(() => model.Title)
                .VerticalAlignment(VerticalAlignment.Center)
                .TextWrapping(TextWrapping.WrapWholeWords)
                .Grid(column: 1)
            ]);
    }
}


internal class SelectServerResult(XamlRoot xamlRoot) : SelectResult<VideoServer>(xamlRoot)
{
    public override UIElement CreateElement(VideoServer model)
    {
        return new Grid()
            .Margin(8)
            .ColumnDefinitions("Auto,*")
            .ColumnSpacing(8)
            .Children(
            [
                new TextBlock()
                .Text(() => model.Name)
                .VerticalAlignment(VerticalAlignment.Center)
                .TextWrapping(TextWrapping.WrapWholeWords)
                .Grid(column: 1)
            ]);
    }

    public override string GetTitle() => "Select Server";
}
