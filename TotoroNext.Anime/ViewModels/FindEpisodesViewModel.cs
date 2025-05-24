using FlyleafLib.MediaPlayer;
using JetBrains.Annotations;
using ReactiveUI;
using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime.ViewModels;

[UsedImplicitly]
public class FindEpisodesViewModel(SearchResult result) : ReactiveObject
{
    public Player MediaPlayer { get; } = new Player();

    public async Task Initialize()
    {
        var episodes = await result.GetEpisodes().ToListAsync();

        var servers = await episodes.First().GetServers().ToListAsync();

        var streams = await servers.First(x => x.Name == "S-mp4").Extract().ToListAsync();
        var stream = streams.First();

        foreach (var item in stream.Headers)
        {
            MediaPlayer.Config.Demuxer.FormatOpt[item.Key] = item.Value;
        }
        MediaPlayer.Open(stream.Url.ToString());
        MediaPlayer.Play();
    }

}
