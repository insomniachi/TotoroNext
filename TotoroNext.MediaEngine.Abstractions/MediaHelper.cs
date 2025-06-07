using System.Diagnostics;
using System.Reflection;

namespace TotoroNext.MediaEngine.Abstractions;

public static class MediaHelper
{
    public static TimeSpan GetDuration(Uri url, IDictionary<string,string>? headers = null)
    {
        string ffprobePath = Directory.GetFiles(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!).FirstOrDefault(x => x.Contains("ffprobe"))!;

        var startInfo = new ProcessStartInfo
        {
            FileName = ffprobePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (headers is { Count : >0 } )
        {
            startInfo.ArgumentList.Add("-headers");
            startInfo.ArgumentList.Add(string.Join("\r\n", headers.Select(x => $"{x.Key}: {x.Value}")));
        }

        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add(url.ToString());
        startInfo.ArgumentList.Add("-show_entries");
        startInfo.ArgumentList.Add("format=duration");
        startInfo.ArgumentList.Add("-v");
        startInfo.ArgumentList.Add("quiet");
        startInfo.ArgumentList.Add("-of");
        startInfo.ArgumentList.Add("csv=p=0");

        using var process = new Process() { StartInfo = startInfo };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (double.TryParse(output.Trim(), out var seconds))
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return TimeSpan.Zero;
    }
}
