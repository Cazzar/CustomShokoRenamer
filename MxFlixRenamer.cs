using Shoko.Models.Server;
using Shoko.Server;
using Shoko.Server.Models;
using Shoko.Server.Renamer;
using Shoko.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renamer.Cazzar
{
    [Renamer("MxFlix", Description = "MxFlix's custom renamer")]
    public class MxFlixRenamer : IRenamer
    {
        private readonly LegacyRenamer script;

        public MxFlixRenamer(RenameScript script)
        {
            this.script = new LegacyRenamer(script);
        }


        public (ImportFolder dest, string folder) GetDestinationFolder(SVR_VideoLocal_Place video)
        {
            try
            {
                var anime = RepoFactory.AniDB_Anime.GetByAnimeID(video.VideoLocal.GetAnimeEpisodes()[0].AniDB_Episode.AnimeID);
                bool IsPorn = anime.Restricted > 0;
                var location = @"E:\Plex\Anime";
                if (IsPorn) location = @"E:\Plex\Hentai";

                return (RepoFactory.ImportFolder.GetByImportLocation(location), Utils.RemoveInvalidFolderNameCharacters(anime.PreferredTitle));
            }
            catch
            {
                return script.GetDestinationFolder(video);
            }

        }

        public string GetFileName(SVR_VideoLocal_Place place) => script.GetFileName(place);

        public string GetFileName(SVR_VideoLocal video) => script.GetFileName(video);
    }
}
