using System.Reactive;
using System.Reactive.Linq;
using Flurl;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace TotoroNext.MediaEngine.Abstractions;

public sealed class InternalMediaPlayer : IInternalMediaPlayer
{
    private readonly MediaPlayer _mp = new() { AutoPlay = true };

    MediaPlayer IInternalMediaPlayer.MediaPlayer => _mp;

    public InternalMediaPlayer()
    {
        DurationChanged = Observable
            .FromEventPattern<TypedEventHandler<MediaPlayer, object>, object>(
                h => _mp.MediaOpened += h,
                h => _mp.MediaOpened -= h)
            .Select(_ => _mp.PlaybackSession.NaturalDuration);

        PositionChanged = Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Select(_ => _mp.PlaybackSession.Position)
            .DistinctUntilChanged();

        PlaybackStopped = Observable
            .FromEventPattern<TypedEventHandler<MediaPlayer, object>, object>(
                h => _mp.MediaEnded += h,
                h => _mp.MediaEnded -= h)
            .Select(_ => Unit.Default);
    }

    public IObservable<TimeSpan> DurationChanged { get; }
    public IObservable<TimeSpan> PositionChanged { get; }
    public IObservable<Unit> PlaybackStopped { get; }

    public void Play(Media media, TimeSpan startPosition)
    {
        var uri = media.Metadata.Headers is { Count: > 0 }
            ? CreateProxyUrl(media)
            : media.Uri;

        _mp.Source = MediaSource.CreateFromUri(uri);

        // Wait for media to open, then seek
        void handler(MediaPlayer mp, object e)
        {
            mp.MediaOpened -= handler;
            mp.Position = startPosition;
            mp.Play();
        }

        _mp.MediaOpened += handler;
    }

    private static Uri CreateProxyUrl(Media media)
    {
        var request = $"http://localhost:{VideoStreamProxyService.Port}/video".AppendQueryParam("url", media.Uri);

        if (media.Metadata.Headers is { Count: > 0 })
        {
            foreach (var kvp in media.Metadata.Headers)
            {
                request = request.AppendQueryParam(kvp.Key, kvp.Value);
            }
        }

        return request.ToUri();
    }

    public Task SeekTo(TimeSpan position)
    {
        _mp.Position = position;
        return Task.CompletedTask;
    }
}
