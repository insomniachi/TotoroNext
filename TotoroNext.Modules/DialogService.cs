using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;

public class DialogService(XamlRoot xamlRoot) : IDialogService
{
    public async Task<DialogResult> Ask(string title, string question)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            CloseButtonText = "No",
            PrimaryButtonText = "Yes",
            DefaultButton = ContentDialogButton.Primary,
            Content = new TextBlock().Text(question),
            XamlRoot = xamlRoot
        };

        var result = await dialog.ShowAsync();

        return result switch
        {
            ContentDialogResult.Primary => DialogResult.Yes,
            ContentDialogResult.Secondary => DialogResult.No,
            _ => DialogResult.Cancel
        };
    }
}
