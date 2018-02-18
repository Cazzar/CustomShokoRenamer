using Shoko.Models.Server;
using Shoko.Server.Models;
using Shoko.Server.Renamer;

namespace Renamer.Cazzar
{
    [Renamer("NoSpaces", Description = "Renamer that doesn't use spaces")]
    class NoSpaceRenamer : IRenamer
    {
        private readonly LegacyRenamer script;

        public NoSpaceRenamer(RenameScript script)
        {
            this.script = new LegacyRenamer(script);
        }

        public string GetFileName(SVR_VideoLocal_Place place) => script.GetFileName(place);

        public string GetFileName(SVR_VideoLocal video) => script.GetFileName(video);

        public (ImportFolder dest, string folder) GetDestinationFolder(SVR_VideoLocal_Place video)
        {
            var (dest, folder) = script.GetDestinationFolder(video);
            return (dest, folder.Replace(' ', '_'));
        }
    }
}
