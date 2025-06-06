using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;
using TotoroNext.Anime.ViewModels;

namespace TotoroNext.Anime.Views;

public sealed partial class UserListPage : Page
{
    public UserListPage()
    {
        InitializeComponent();
    }

    public UserListViewModel? ViewModel => DataContext as UserListViewModel;

    private async void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is AnimeModel model)
        {
            await (ViewModel?.AnimeSelected(model) ?? Task.CompletedTask);
        }
    }

    private static int count = 0;

    private async Task<SearchResult?> SelectResult(List<SearchResult> results)
    {
        count++;

        if(count > 1)
        {
            return null;
        }

        var dialog = new ContentDialog
        {
            Title = "Select",
            CloseButtonText = "Close",
            PrimaryButtonText = "Select",
            DefaultButton = ContentDialogButton.Primary,
            Content = new ListView()
            .ItemsSource(results)
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
            XamlRoot = XamlRoot
        };

        var result = await dialog.ShowAsync();

        return result is ContentDialogResult.Primary
            ? listView.SelectedItem as SearchResult
            : null;
    }
}
