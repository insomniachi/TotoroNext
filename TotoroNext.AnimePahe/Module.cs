using System.Net;
using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Module;

namespace TotoroNext.AnimeHeaven;
public class Module : IModule
{
    public void ConfigureNavigation(NavigationViewContext context) { }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKeyedTransient<IAnimeProvider, AnimeHeavenProvider>("AnimeHeaven");
        services.AddKeyedTransient<IAnimeProvider, AllAnimeProvider>("AllAnime");
        services.AddHttpClient("AnimeHeaven", client =>
        {
            client.BaseAddress = new Uri("https://animeheaven.me");
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, Http.UserAgent);
            client.DefaultRequestVersion = HttpVersion.Version11;
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
        });
    }
}
