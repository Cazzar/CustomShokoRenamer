using System;
using System.Linq;
using System.Text;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Metadata;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Abstractions.Video.Enums;
using Shoko.Abstractions.Video.Relocation;

namespace Renamer.Cazzar;

public class CazzarRenamer : IRelocationProvider
{
    public string Name => "Cazzar Renamer";
    public string Description => "Cazzar's Custom Renamer (Linux)";
    public bool SupportsMoving => true;
    public bool SupportsRenaming => true;

    public RelocationResult GetPath(RelocationContext context)
    {
        var anime = context.Series.FirstOrDefault();
        if (anime is null)
            return RelocationResult.FromError("No series metadata is available for the file.");

        var episode = context.Episodes.FirstOrDefault();
        if (episode is null && anime.Type != AnimeType.Movie)
            return RelocationResult.FromError("No episode metadata is available for the file.");

        try
        {
            var result = new RelocationResult();

            if (context.RenameEnabled)
            {
                if (context.Video.MediaInfo?.VideoStream is null)
                    return RelocationResult.FromError("No video stream information is available for the file.");

                result.FileName = GetFileName(context, anime, episode);
            }
            else
            {
                result.SkipRename = true;
            }

            if (context.MoveEnabled)
            {
                var location = GetLocation(anime);
                var folder = context.AvailableFolders.FirstOrDefault(a => a.Path.Equals(location, StringComparison.InvariantCultureIgnoreCase));
                if (folder is null)
                    return RelocationResult.FromError($"No managed folder is available at \"{location}\".");

                result.ManagedFolder = folder;
                result.Path = anime.Title.ReplaceInvalidPathCharacters();
            }
            else
            {
                result.SkipMove = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            return RelocationResult.FromError(ex);
        }
    }

    private static string GetFileName(RelocationContext context, ISeries anime, IEpisode episode)
    {
        var video = context.Video;
        var release = video.ReleaseInfo;

        var name = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(release?.Group?.ShortName))
            name.Append($"[{release.Group.ShortName}]");

        name.Append($" {anime.Title}");
        if (anime.Type != AnimeType.Movie)
        {
            var prefix = episode.Type switch
            {
                EpisodeType.Credits => "C",
                EpisodeType.Other => "O",
                EpisodeType.Parody => "P",
                EpisodeType.Special => "S",
                EpisodeType.Trailer => "T",
                _ => ""
            };

            var epCount = episode.Type switch
            {
                EpisodeType.Episode => anime.EpisodeCounts.Episodes,
                EpisodeType.Special => anime.EpisodeCounts.Specials,
                _ => 1
            };

            name.Append($" - {prefix}{PadNumberTo(episode.EpisodeNumber, epCount)}");
        }

        name.Append($" ({video.MediaInfo.VideoStream.Resolution}");
        if (release?.Source is ReleaseSource.DVD or ReleaseSource.BluRay)
            name.Append($" {(release.Source == ReleaseSource.BluRay ? "Blu-ray" : "DVD")}");

        name.Append($" {(video.MediaInfo.VideoStream.Codec.Simplified ?? video.MediaInfo.VideoStream.Codec.Name).Replace("\\", "").Replace("/", "")}".TrimEnd());

        if (video.MediaInfo.VideoStream.BitDepth == 10)
            name.Append(" 10bit");
        name.Append(')');

        if (release?.IsCensored ?? false) name.Append(" [CEN]");

        name.Append($" [{video.Hashes.FirstOrDefault(h => h.Type is "CRC32")?.Value.ToUpper()}]");
        name.Append($"{System.IO.Path.GetExtension(context.File.FileName)}");

        return name.ToString().ReplaceInvalidPathCharacters();
    }

    private static string PadNumberTo(int number, int max, char padWith = '0')
    {
        return number.ToString().PadLeft(Math.Max(max.ToString().Length, 2), padWith);
    }

    private static string GetLocation(ISeries anime)
    {
        if (anime.Restricted) return "/porn/";
        return anime.Type == AnimeType.Movie ? "/movies/" : "/anime/";
    }
}
