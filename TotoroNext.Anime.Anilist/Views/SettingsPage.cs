using System.Web;
using CommunityToolkit.WinUI.Controls;
using TotoroNext.Anime.Anilist.ViewModels;

namespace TotoroNext.Anime.Anilist.Views;

public partial class SettingsPage : Microsoft.UI.Xaml.Controls.Page
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
                            SettingsCard("Login to your anilist account", "Authenticate", new FontIcon {Glyph = "\uE756"})
                            .Content(new Button()
                                .Content("Authenticate")
                                .Name(out var button, b =>
                                {
                                    b.Click += (s, e) =>
                                    {
                                        splitView.IsPaneOpen = true;
                                        splitView.Pane = GetPane(splitView);
                                    };
                                })),

                            SettingsCard("Include nsfw results","Include NSFW", new FontIcon {Glyph = "\uE740"})
                            .Content(new ToggleSwitch().IsOn(x => x.Binding(() => vm.IncludeNsfw).TwoWay())),

                            SettingsCard("Number of results returned when searching by name", "Search limit", new FontIcon{ Glyph = "\uF6FA"})
                            .Content(new NumberBox().Value(x => x.Binding(() => vm.SearchLimit).TwoWay()))

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

    public Grid GetPane(SplitView splitView)
    {
        return new Grid().Children([
            new WebView2().Source(new Uri("https://anilist.co/api/v2/oauth/authorize?client_id=10588&response_type=token"))
            .Name(out var view, webview =>
            {
                webview.NavigationCompleted += (s, e) =>
                {
                    var url = s.Source.ToString();
                    if(!url.Contains("access_token"))
                    {
                        return;
                    }

                    var queries = HttpUtility.ParseQueryString(url);

                    var token = new AniListAuthToken
                    {
                        AccessToken = queries[0]!,
                        ExpiresIn = long.Parse(queries[2]!),
                        CreatedAt = DateTime.Now
                    };

                    if(DataContext is SettingsViewModel vm)
                    {
                        vm.Token = token;
                    }

                    splitView.IsPaneOpen = false;
                };
            })
        ]);
    }

}
