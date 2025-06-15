using System.Web;
using CommunityToolkit.WinUI.Controls;
using MalApi;
using TotoroNext.Anime.MyAnimeList.ViewModels;

namespace TotoroNext.Anime.MyAnimeList.Views;

public partial class SettingsPage : Page
{
    private readonly string _redirectUrl = "https://github.com/insomniachi";
    public const string ClientId = "748da32a6defdd448c1f47d60b4bbe69";

    public SettingsPage()
    {
        this.DataContext<SettingsViewModel>((page, vm) =>
        {
            page.Content(new ScrollView()
                    .Margin(36)
                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                    .Content(new StackPanel()
                        .MaxWidth(1000)
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .Children(
                        [
                            SettingsCard("Login to your MyAnimeList account", "Authenticate", new FontIcon {Glyph = "\uE756"})
                                .Content(new Button()
                                    .Content("Authenticate")
                                    .Name(out var button, b =>
                                    {
                                        b.Click += async(s, e) =>
                                        {
                                            await Authenticate();
                                        };
                                    })),

                            SettingsCard("Include nsfw results","Include NSFW", new FontIcon {Glyph = "\uE740"})
                                .Content(new ToggleSwitch().IsOn(x => x.Binding(() => vm.IncludeNsfw).TwoWay())),

                            SettingsCard("Number of results returned when searching by name", "Search limit", new FontIcon{ Glyph = "\uF6FA"})
                                .Content(new NumberBox().Value(x => x.Binding(() => vm.SearchLimit).TwoWay()))
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

    public async Task Authenticate()
    {
        var dialog = new ContentDialog
        {
            Title = "Login",
            CloseButtonText = "Close",
            XamlRoot = XamlRoot
        };

        var content = new Grid().Children([
                    new WebView2().Source(new Uri(MalAuthHelper.GetAuthUrl(ClientId)))
                    .Name(out var view, webview =>
                    {
                        webview.NavigationCompleted += async (s, e) =>
                        {
                            var url = s.Source.ToString();
                            if(!url.StartsWith(_redirectUrl))
                            {
                                return;
                            }

                            var code = HttpUtility.ParseQueryString(url)[0];
                            var token = await MalAuthHelper.DoAuth(ClientId, code);

                            if(DataContext is SettingsViewModel vm)
                            {
                                vm.Token = token;
                            }

                            dialog.Hide();
                        };
                    })
                ])
            .Height(600)
            .Width(800);

        dialog.Content = content;

        await dialog.ShowAsync();
    }
}
