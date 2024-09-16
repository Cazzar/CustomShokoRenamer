using System;
using System.Linq;
using System.Text;
using Shoko.Plugin.Abstractions;
using Shoko.Plugin.Abstractions.Attributes;
using Shoko.Plugin.Abstractions.DataModels;
using Shoko.Plugin.Abstractions.Events;

namespace Renamer.Cazzar;

[RenamerID("CazzarRenamer")]
public class MyRenamer : IRenamer
{
    public string Name => "Cazzar Renamer";
    public string Description => "Cazzar's Custom Renamer (Linux)";
    public bool SupportsMoving => true;
    public bool SupportsRenaming => true;
    public RelocationResult GetNewPath(RelocationEventArgs args)
    {
        var filename = GetFileName(args);
        var dest = GetDestinationFolder(args);
        return new RelocationResult
        {
            FileName = filename,
            Path = dest.folder,
            DestinationImportFolder = dest.dest
        };
    }

    private string GetFileName(RelocationEventArgs args)
    {
        var file = args.File.Video;
        var episode = args.Episodes.FirstOrDefault();
        var anime = args.Series.FirstOrDefault();

        var name = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(file.AniDB.ReleaseGroup.ShortName))
            name.Append($"[{file.AniDB.ReleaseGroup.ShortName}]");

        name.Append($" {anime.PreferredTitle}");
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
        name.Append($" ({file.MediaInfo.VideoStream.Resolution}");
        if (file.AniDB.Source.Equals("DVD", StringComparison.InvariantCultureIgnoreCase) ||
            file.AniDB.Source.Equals("Blu-ray", StringComparison.InvariantCultureIgnoreCase))
            name.Append($" {file.AniDB.Source}");

        name.Append($" {(file.MediaInfo.VideoStream.Codec.Simplified ?? file.MediaInfo.VideoStream.Codec.Name).Replace("\\", "").Replace("/", "")}".TrimEnd());

        if (file.MediaInfo?.VideoStream?.BitDepth == 10)
            name.Append(" 10bit");
        name.Append(')');

        if (file.AniDB.Censored) name.Append(" [CEN]");

        name.Append($" [{file.Hashes.CRC.ToUpper()}]");
        name.Append($"{System.IO.Path.GetExtension(args.File.FileName)}");

        return name.ToString().ReplaceInvalidPathCharacters();
    }

    private static string PadNumberTo(int number, int max, char padWith = '0')
    {
        return number.ToString().PadLeft(Math.Max(max.ToString().Length, 2), padWith);
    }

    private (IImportFolder dest, string folder) GetDestinationFolder(RelocationEventArgs args)
    {
        var anime = args.Series.FirstOrDefault();
        var isPorn = anime.Restricted;
        var location = "/anime/";
        if (anime.Type == AnimeType.Movie) location = "/movies/";
        if (isPorn) location = "/porn/";

        var dest = args.AvailableFolders.FirstOrDefault(a => a.Path.Equals(location, StringComparison.InvariantCultureIgnoreCase));

        return (dest, anime.PreferredTitle.ReplaceInvalidPathCharacters());
    }
}