using System.Diagnostics;
using System.IO.Pipes;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.MediaEngine.Mpv;

internal class MpvMediaPlayer(IModuleSettings<Settings> settings) : IMediaPlayer
{
    private Process? _process;
    private readonly Settings _settings = settings.Value;
    private NamedPipeClientStream? _ipcStream;
    private readonly Subject<TimeSpan> _durationSubject = new();
    private readonly Subject<TimeSpan> _positionSubject = new();

    public IObservable<TimeSpan> DurationChanged => _durationSubject;
    public IObservable<TimeSpan> PositionChanged => _positionSubject;

    public void Play(Media media)
    {
        _process?.Kill();
        _ipcStream?.Dispose();

        var pipeName = $"mpv-pipe-{Guid.NewGuid()}";
        var pipePath = $@"\\.\pipe\{pipeName}";

        var startInfo = new ProcessStartInfo
        {
            FileName = _settings.FileName,
            ArgumentList =
            {
                media.Uri.ToString(),
                $"--title={media.Metadata.Title}",
                $"--force-media-title={media.Metadata.Title}",
                $"--input-ipc-server={pipePath}"
            },
        };

        if (_settings.LaunchFullScreen)
        {
            startInfo.ArgumentList.Add("--fullscreen");
        }

        if (media.Metadata.Headers is { Count: > 0 } headers)
        {
            var headerFields = string.Join(" ", headers.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
            startInfo.ArgumentList.Add($"--http-header-fields={headerFields}");
        }

        if (media.Metadata.MedaSections is { Count: > 0 } sections)
        {
            var file = ChapterFileWriter.CreateChapterFile(sections);
            startInfo.ArgumentList.Add($"--chapters-file={file}");
        }

        _process = Process.Start(startInfo);

        Task.Run(async () =>
        {
            while (!File.Exists(pipePath))
            {
                await Task.Delay(500);
            }

            await IpcLoop(pipeName);
        });
    }

    private async Task IpcLoop(string pipeName)
    {
        using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        _ipcStream = pipe;
        await pipe.ConnectAsync();

        // Observe properties
        await SendIpcCommand(pipe, new { command = new object[] { "observe_property", 1, "duration" } });
        await SendIpcCommand(pipe, new { command = new object[] { "observe_property", 2, "time-pos" } });

        using var reader = new StreamReader(pipe, Encoding.UTF8);

        while (pipe.IsConnected)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line))
            {
                HandleIpcMessage(line);
            }
            else
            {
                await Task.Delay(100);
            }
        }
    }

    private static async Task SendIpcCommand(NamedPipeClientStream pipe, object command)
    {
        var json = JsonSerializer.Serialize(command) + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);
        await pipe.WriteAsync(bytes);
        await pipe.FlushAsync();
    }

    private void HandleIpcMessage(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("event", out var evt) && evt.GetString() == "property-change")
        {
            var name = root.GetProperty("name").GetString();
            var data = root.GetProperty("data");
            if (name == "duration" && data.ValueKind == JsonValueKind.Number)
            {
                _durationSubject.OnNext(TimeSpan.FromSeconds(data.GetDouble()));
            }
            else if (name == "time-pos" && data.ValueKind == JsonValueKind.Number)
            {
                _positionSubject.OnNext(TimeSpan.FromSeconds(data.GetDouble()));
            }
        }
    }
}
