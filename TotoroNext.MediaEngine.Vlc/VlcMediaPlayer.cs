using System.Diagnostics;
using System.IO.Pipes;
using System.Reactive.Subjects;
using System.Diagnostics.CodeAnalysis;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Vlc;

internal class VlcMediaPlayer : IMediaPlayer
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
            FileName = @"C:\Program Files\VideoLAN\VLC\vlc.exe",
            ArgumentList =
            {
                media.Uri.ToString(),
                "--http-host=127.0.0.1",
                "--http-port=8080",
                "--fullscreen",
                $"--meta-title={media.Title}",
                $"--http-password={HttpInterface.Password}",
            }
        };

        if(media.Headers.TryGetValue("user-agent", out string? userAgent))
        {
            startInfo.ArgumentList.Add($":http-user-agent={userAgent}");
        }

        if(media.Headers.TryGetValue("referer", out string? referer))
        {
            startInfo.ArgumentList.Add($":http-referer={referer}");
        }

        _process = new Process() { StartInfo = startInfo };
        _process.Start();

        var webInterface = new HttpInterface(_process);
        webInterface.DurationChanged.Subscribe(_positionSubject.OnNext);
        webInterface.PositionChanged.Subscribe(_durationSubject.OnNext);    
    }
}
