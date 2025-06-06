using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.UserInteractions;

internal class SelectProviderResult(XamlRoot xamlRoot) : IUserInteraction<List<SearchResult>, SearchResult>
{
    public async Task<SearchResult?> GetValue(List<SearchResult> input)
    {
        var dialog = new ContentDialog
        {
            Title = "Select",
            CloseButtonText = "Close",
            PrimaryButtonText = "Select",
            DefaultButton = ContentDialogButton.Primary,
            Content = new ListView()
            .ItemsSource(input)
            .Name(out var listView)
            .SelectionMode(ListViewSelectionMode.Single)
            .ItemTemplate<SearchResult>(item =>
                new Grid()
                .Margin(8)
                .ColumnDefinitions("Auto,*")
                .ColumnSpacing(8)
                .Children(
                [
                    new Image()
                    .Source(() => item.Image, x => Converters.UriToImage(x)!)
                    .Height(100).Width(75)
                    .Stretch(Stretch.UniformToFill)
                    .Grid(column: 0),

                    new TextBlock()
                    .Text(() => item.Title)
                    .VerticalAlignment(VerticalAlignment.Center)
                    .TextWrapping(TextWrapping.WrapWholeWords)
                    .Grid(column: 1)
                ])),
            XamlRoot = xamlRoot
        };

        var result = await dialog.ShowAsync();

        return result is ContentDialogResult.Primary
            ? listView.SelectedItem as SearchResult
            : null;
    }
}

internal class SelectAnimeResult(XamlRoot xamlRoot) : IUserInteraction<List<AnimeModel>, AnimeModel>
{
    public async Task<AnimeModel?> GetValue(List<AnimeModel> input)
    {
        var dialog = new ContentDialog
        {
            Title = "Select",
            CloseButtonText = "Close",
            PrimaryButtonText = "Select",
            DefaultButton = ContentDialogButton.Primary,
            Content = new ListView()
            .ItemsSource(input)
            .Name(out var listView)
            .SelectionMode(ListViewSelectionMode.Single)
            .ItemTemplate<AnimeModel>(item =>
                new Grid()
                .Margin(8)
                .ColumnDefinitions("Auto,*")
                .ColumnSpacing(8)
                .Children(
                [
                    new Image()
                    .Source(() => item.Image, x => Converters.StringToImage(x)!)
                    .Height(100).Width(75)
                    .Stretch(Stretch.UniformToFill)
                    .Grid(column: 0),

                    new TextBlock()
                    .Text(() => item.Title)
                    .VerticalAlignment(VerticalAlignment.Center)
                    .TextWrapping(TextWrapping.WrapWholeWords)
                    .Grid(column: 1)
                ])),
            XamlRoot = xamlRoot
        };

        var result = await dialog.ShowAsync();

        return result is ContentDialogResult.Primary
            ? listView.SelectedItem as AnimeModel
            : null;
    }
}
