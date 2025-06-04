using System.Net.Http.Headers;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Anilist.ViewModels;
using TotoroNext.Anime.Anilist.Views;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Anime.Anilist;

public class Module : IModule<Settings>
{
    public Descriptor Descriptor { get; } = new Descriptor
    {
        Id = new Guid("b5d31e9b-b988-44e8-8e28-348f58cf1d04"),
        Name = "Anilist",
        Components = [ComponentTypes.Metadata],
        Description = "AniList: The next-generation anime platform Track, share, and discover your favorite anime and manga with AniList. Discover your obsessions. ",
        SettingViewModel = typeof(SettingsViewModel),
        HeroImage = ResourceHelper.GetResource("anilist.jpg")
    };

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient(_ => Descriptor);
        services.AddModuleSettings(this);
        services.AddViewMap<SettingsPage, SettingsViewModel>();
        services.TryAddKeyedTransient<IMetadataService, AnilistMetadataService>(Descriptor.Id);
        services.AddTransient(sp =>
        {
            var settings = sp.GetRequiredKeyedService<IModuleSettings<Settings>>(Descriptor.Id);
            var client = new GraphQLHttpClient("https://graphql.anilist.co/", new NewtonsoftJsonSerializer());

            if(settings.Value.Auth is { } auth)
            {
                client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            }

            return client;
        });
    }
}


public class Settings
{
    public AniListAuthToken? Auth { get; set; }
    public bool IncludeNsfw { get; set; }
}

public class AniListAuthToken
{
    public string AccessToken { get; set; } = "";
    public long ExpiresIn { get; set; }
    public DateTime CreatedAt { get; set; }
}
