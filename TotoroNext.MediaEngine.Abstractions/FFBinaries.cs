using System.IO.Compression;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace TotoroNext.MediaEngine.Abstractions;

public static class FFBinaries
{
    public static async Task DownloadLatest()
    {
        var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!);

        if (files.Any(x => x.Contains("ffprob")))
        {
            return;
        }

        using var client = new HttpClient();

        var release = await client.GetFromJsonAsync<FFBinaryRelease>("https://ffbinaries.com/api/v1/version/latest");

        if (release is null)
        {
            return;
        }

        FFBinary? bin = null;
        if (OperatingSystem.IsWindows())
        {
            bin = release.Bin.Windows;
        }
        else if (OperatingSystem.IsLinux())
        {
            bin = release.Bin.Linux;
        }
        else if (OperatingSystem.IsMacOS())
        {
            bin = release.Bin.Mac;
        }

        if (bin is null)
        {
            return;
        }

        var stream = await client.GetStreamAsync(bin.FFProb);
        ZipFile.ExtractToDirectory(stream, ".", true);
    }
}

public class FFBinaryRelease
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("permalink")]
    public string Permalink { get; set; } = "";

    [JsonPropertyName("bin")]
    public FFReleasePlatforms Bin { get; set; } = new();
}

public class FFReleasePlatforms
{
    [JsonPropertyName("windows-64")]
    public FFBinary Windows { get; set; } = new();

    [JsonPropertyName("linux-64")]
    public FFBinary Linux { get; set; } = new();

    [JsonPropertyName("osx-64")]
    public FFBinary Mac { get; set; } = new();
}

public class FFBinary
{
    [JsonPropertyName("ffmpeg")]
    public string FFMpeg { get; set; } = "";

    [JsonPropertyName("ffprobe")]
    public string FFProb { get; set; } = "";
}

