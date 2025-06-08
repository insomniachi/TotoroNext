using System.Diagnostics;
using System.Reactive.Subjects;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

internal class VlcMediaPlayer(Settings settings) : IMediaPlayer
{
    private Process? _process;
    private readonly Subject<TimeSpan> _durationSubject = new();
    private readonly Subject<TimeSpan> _positionSubject = new();

    public IObservable<TimeSpan> DurationChanged => _durationSubject;
    public IObservable<TimeSpan> PositionChanged => _positionSubject;

    public void Play(Media media)
    {
        _process?.Kill();

        var startInfo = new ProcessStartInfo
        {
            FileName = settings.FileName,
            ArgumentList =
            {
                media.Uri.ToString(),
                "--http-host=127.0.0.1",
                "--http-port=8080",
                $"--meta-title={media.Metadata.Title}",
                $"--http-password={HttpInterface.Password}",
            }
        };

        if (settings.LaunchFullScreen)
        {
            startInfo.ArgumentList.Add("--fullscreen");
        }

        if (media.Metadata.Headers?.TryGetValue("user-agent", out string? userAgent) == true)
        {
            startInfo.ArgumentList.Add($"--http-user-agent={userAgent}");
        }

        if (media.Metadata.Headers?.TryGetValue("referer", out string? referer) == true)
        {
            startInfo.ArgumentList.Add($"--http-referrer={referer}");
        }

        _process = new Process() { StartInfo = startInfo };
        _process.Start();

        var webInterface = new HttpInterface(_process);
        webInterface.DurationChanged.Subscribe(_positionSubject.OnNext);
        webInterface.PositionChanged.Subscribe(_durationSubject.OnNext);
    }
}
