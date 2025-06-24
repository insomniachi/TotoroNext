using CommunityToolkit.WinUI.Controls;
using TotoroNext.MediaEngine.Vlc.ViewModels;
using TotoroNext.Module;

namespace TotoroNext.MediaEngine.Vlc.Views;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.DataContext<SettingsPageViewModel>((page, vm) =>
        {
            page.Content(new ScrollView()
                    .Margin(36)
                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                    .Content(new StackPanel()
                        .MaxWidth(1000)
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .Children(
                        [
                            new Image().Source(ResourceHelper.GetResource("vlc.jpeg")).Stretch(Stretch.Uniform),
                            SettingsCard("Command used to launch vlc from Terminal", "Command", new FontIcon {Glyph = "\uE756"})
                                .Content(new StackPanel()
                                .Orientation(Orientation.Horizontal)
                                .Spacing(8)
                                .Children(
                                [
                                    new TextBlock()
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .Text(x => x.Binding(() => vm.Command).OneWay())
                                    .Width(200)
                                    .TextTrimming(TextTrimming.CharacterEllipsis)
                                    .ToolTipService(b => b.ToolTip(() => vm.Command)),

                                    new Button()
                                    .Content("Browse")
                                    .Command(() => vm.PickFileCommand)
                                ])),

                            SettingsCard("Start in fullscreen mode","Start Fullscreen", new FontIcon {Glyph = "\uE740"})
                                .Content(new StackPanel()
                                .Orientation(Orientation.Horizontal)
                                .Spacing(8)
                                .Children(
                                [
                                    new ToggleSwitch()
                                    .IsOn(x => x.Binding(() => vm.LaunchFullScreen).TwoWay())
                                ])),
                        ])));
        });
    }

    private static SettingsCard SettingsCard(string description, string header, FontIcon icon)
    {
        return new SettingsCard()
        {
            Description = description,
            Header = header,
            HeaderIcon = icon
        };
    }
}
