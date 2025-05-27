using System.Diagnostics;
using System.IO.Pipes;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.MediaEngine.Mpv;

internal class MpvMediaPlayer : IMediaPlayer
{
    private Process? _process;
    private NamedPipeClientStream? _ipcStream;
    private readonly Subject<TimeSpan> _durationSubject = new();
    private readonly Subject<TimeSpan> _positionSubject = new();

    public IObservable<TimeSpan> DurationChanged => _durationSubject;
    public IObservable<TimeSpan> PositionChanged => _positionSubject;

	public void Play(Media media)
	{
		_process?.Kill();
		_ipcStream?.Dispose();

		// Generate a unique pipe name for each instance
		var pipeName = $"mpv-pipe-{Guid.NewGuid()}";
		var pipePath = $@"\\.\pipe\{pipeName}";

		var startInfo = new ProcessStartInfo
		{
			FileName = @"C:\Users\athul\Downloads\mpv-x86_64-20250527-git-1d1535f\mpv.exe",
			ArgumentList =
			{
				media.Uri.ToString(),
				"--fullscreen",
				$"--title={media.Title}",
				$"--input-ipc-server={pipePath}"
			},
		};

		if (media.Headers is not null && media.Headers.Count > 0)
		{
			var headerFields = string.Join(" ", media.Headers.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
			startInfo.ArgumentList.Add($"--http-header-fields={headerFields}");
		}

		_process = Process.Start(startInfo);

		Task.Run(async () => await IpcLoop(pipeName));
	}

    private async Task IpcLoop(string pipePath)
    {
        using var pipe = new NamedPipeClientStream(".", pipePath, PipeDirection.InOut, PipeOptions.Asynchronous);
        _ipcStream = pipe;
        await pipe.ConnectAsync();

        // Observe properties
        SendIpcCommand(pipe, new { command = new object[] { "observe_property", 1, "duration" } });
        SendIpcCommand(pipe, new { command = new object[] { "observe_property", 2, "time-pos" } });

        var buffer = new byte[4096];
        var sb = new StringBuilder();

        while (pipe.IsConnected)
        {
            int bytesRead = pipe.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                string content = sb.ToString();
                int idx;
                while ((idx = content.IndexOf('\n')) >= 0)
                {
                    string line = content[..idx].Trim();
                    content = content[(idx + 1)..];
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        HandleIpcMessage(line);
                    }
                }
                sb.Clear();
                sb.Append(content);
            }
            else
            {
                Thread.Sleep(100);
            }
        }
    }

    private static void SendIpcCommand(NamedPipeClientStream pipe, object command)
    {
        var json = JsonSerializer.Serialize(command) + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);
        pipe.Write(bytes, 0, bytes.Length);
        pipe.Flush();
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
