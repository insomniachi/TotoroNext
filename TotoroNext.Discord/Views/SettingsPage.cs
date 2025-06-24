using CommunityToolkit.WinUI.Controls;
using TotoroNext.Discord.ViewModels;
using TotoroNext.Module;

namespace TotoroNext.Discord.Views;

internal partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.DataContext<SettingsViewModel>((page, vm) =>
        {
            page.Content(new SplitView()
                .Name(out var splitView)
                .PanePlacement(SplitViewPanePlacement.Right)
                .OpenPaneLength(600)
                .DisplayMode(SplitViewDisplayMode.Inline)
                .Content(new ScrollView()
                    .Margin(36)
                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                    .Content(new StackPanel()
                        .MaxWidth(1000)
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .Children(
                        [
                            new Image().Source(ResourceHelper.GetResource("discord-logo.jpg")).Stretch(Stretch.Uniform),
   
                            SettingsCard("Show discord rich presence while watching","Enabled", new FontIcon {Glyph = "\uE90A"})
                            .Content(new ToggleSwitch().IsOn(x => x.Binding(() => vm.IsEnabled).TwoWay())),
                        ]))));
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
