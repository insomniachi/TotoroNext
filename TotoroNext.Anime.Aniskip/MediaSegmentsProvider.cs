using System.Diagnostics;
using TotoroNext.MediaEngine.Abstractions;

namespace TotoroNext.Anime.Aniskip;

internal class MediaSegmentsProvider(IAniskipClient client) : IMediaSegmentsProvider
{
    public async Task<List<MediaSegment>> GetSegments(long id, float episode, double mediaLength)
    {
        try
        {
            var result = await client.GetSkipTimes(id, episode, new GetSkipTimesQueryV2
            {
                EpisodeLength = mediaLength,
                Types = [SkipType.Recap, SkipType.Opening, SkipType.Ending]
            });

            if (!result.IsFound)
            {
                return [];
            }

            var segments = result.Results.OrderBy(x => x.Interval.StartTime).Select(x => new MediaSegment(ConvertType(x.SkipType),
                                                                   TimeSpan.FromSeconds(x.Interval.StartTime),
                                                                   TimeSpan.FromSeconds(x.Interval.EndTime))).ToList();

            var last = segments.Last();
            if (last.End.TotalSeconds < mediaLength)
            {
                segments.Add(new MediaSegment(MediaSectionType.Content, last.End, TimeSpan.FromSeconds(mediaLength)));
            }

            return segments;
        }
        catch
        {
            return [];
        }
    }

    private static MediaSectionType ConvertType(SkipType skipType)
    {
        return skipType switch
        {
            SkipType.Recap => MediaSectionType.Recap,
            SkipType.Opening => MediaSectionType.Opening,
            SkipType.Ending => MediaSectionType.Ending,
            _ => throw new UnreachableException()
        };
    }
}
