using System;
using System.Text;
using Shoko.Commons.Extensions;
using Shoko.Models.Enums;
using Shoko.Models.Server;
using Shoko.Server.Models;
using Shoko.Server.Renamer;
using Shoko.Server.Repositories;
using Shoko.Server;

namespace Renamer.Cazzar
{

    [Renamer("CazzarRenamer", Description = "Cazzar's Custom Renamer")]
    public class MyRenamer : IRenamer
    {
        public string GetFileName(SVR_VideoLocal_Place video) => GetFileName(video.VideoLocal);

        public string GetFileName(SVR_VideoLocal video)
        {
            var file = video.GetAniDBFile();
            var episode = video.GetAnimeEpisodes()[0].AniDB_Episode;
            var anime = RepoFactory.AniDB_Anime.GetByAnimeID(episode.AnimeID);

            StringBuilder name = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(file.Anime_GroupNameShort))
                name.Append($"[{file.Anime_GroupNameShort}]");

            name.Append($" {anime.PreferredTitle}");
            if (anime.AnimeType != (int)AnimeType.Movie)
            {
                string prefix = "";

                if (episode.GetEpisodeTypeEnum() == EpisodeType.Credits) prefix = "C";
                if (episode.GetEpisodeTypeEnum() == EpisodeType.Other) prefix = "O";
                if (episode.GetEpisodeTypeEnum() == EpisodeType.Parody) prefix = "P";
                if (episode.GetEpisodeTypeEnum() == EpisodeType.Special) prefix = "S";
                if (episode.GetEpisodeTypeEnum() == EpisodeType.Trailer) prefix = "T";

                int epCount = 1;

                if (episode.GetEpisodeTypeEnum() == EpisodeType.Episode) epCount = anime.EpisodeCountNormal;
                if (episode.GetEpisodeTypeEnum() == EpisodeType.Special) epCount = anime.EpisodeCountSpecial;

                name.Append($" - {prefix}{PadNumberTo(episode.EpisodeNumber, epCount)}");
            }
            name.Append($" ({video.VideoResolution}");
            if (file.File_Source != null &&
                (file.File_Source.Equals("DVD", StringComparison.InvariantCultureIgnoreCase) ||
                 file.File_Source.Equals("Blu-ray", StringComparison.InvariantCultureIgnoreCase)))

            name.Append($" {file.File_Source}");

            name.Append($" {(file?.File_VideoCodec ?? video.VideoCodec).Replace("\\", "").Replace("/", "")}".TrimEnd());

            if (video.VideoBitDepth == "10")
                name.Append($" {video.VideoBitDepth}bit");
            name.Append(')');

            if (file.IsCensored != 0) name.Append(" [CEN]");

            name.Append($" [{video.CRC32.ToUpper()}]");
            name.Append($"{System.IO.Path.GetExtension(video.GetBestVideoLocalPlace().FilePath)}");

            return Utils.ReplaceInvalidFolderNameCharacters(name.ToString());
        }

        string PadNumberTo(int number, int max, char padWith = '0')
        {
            return number.ToString().PadLeft(max.ToString().Length, padWith);
        }

        public (ImportFolder dest, string folder) GetDestinationFolder(SVR_VideoLocal_Place video)
        {
            var anime = RepoFactory.AniDB_Anime.GetByAnimeID(video.VideoLocal.GetAnimeEpisodes()[0].AniDB_Episode.AnimeID);
            bool IsPorn = anime.Restricted > 0;
            var location = "/anime/";
            if (anime.GetAnimeTypeEnum() == AnimeType.Movie) location = "/movies/";
            if (IsPorn) location = "/porn/";


            ImportFolder dest = RepoFactory.ImportFolder.GetByImportLocation(location);

            return (dest, Utils.ReplaceInvalidFolderNameCharacters(anime.PreferredTitle));
        }
    }
}
                                                                                                      