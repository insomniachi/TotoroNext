using Microsoft.UI.Windowing;
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

    public async Task<DialogResult> AskSkip()
    {
        DialogResult result = DialogResult.Cancel;
        var dialogWindow = new Window()
        {
            ExtendsContentIntoTitleBar = true,
        };
        var appWindow = dialogWindow.AppWindow;
        var presenter = (OverlappedPresenter)appWindow.Presenter;
        presenter.IsAlwaysOnTop = true;
        var windowWidth = 200;
        dialogWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32
        {
            Width = windowWidth,
            Height = 75
        });


        var content = new Grid()
            .ColumnSpacing(4)
            .Padding(8)
            .Background(new SolidColorBrush(Colors.Transparent))
            .ColumnDefinitions("*,*")
            .Children([
                new Button().Content("Skip")
                .FontSize(20)
                .HorizontalAlignment(HorizontalAlignment.Stretch)
                .VerticalAlignment(VerticalAlignment.Stretch)
                .Grid(column: 0)
                .Name(out Button _, b => b.Click += (_, _) => { result = DialogResult.Yes; dialogWindow.Close(); }),

                new Button().Content("Cancel")
                .FontSize(20)
                .HorizontalAlignment(HorizontalAlignment.Stretch)
                .VerticalAlignment(VerticalAlignment.Stretch)
                .Grid(column: 1)
                .Name(out Button _, b => b.Click += (_, _) => { result = DialogResult.No; dialogWindow.Close(); })
            ]);

        dialogWindow.Content = content;
        dialogWindow.Activate();
        presenter.SetBorderAndTitleBar(false, false);
        presenter.IsResizable = false;
#if WINDOWS
        var displayArea = DisplayArea.GetFromWindowId(appWindow.OwnerWindowId, DisplayAreaFallback.Primary);
        var screenWidth = displayArea.WorkArea.Width;
        var screenHeight = displayArea.WorkArea.Height;
        appWindow.Move(new Windows.Graphics.PointInt32
        {
            X = screenWidth - windowWidth - 10,
            Y = 10
        });
#endif
        var tcs = new TaskCompletionSource<DialogResult>();
        dialogWindow.Closed += (_, __) => tcs.SetResult(result);
        return await tcs.Task;
    }
}
