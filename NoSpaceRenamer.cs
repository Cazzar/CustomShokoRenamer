using Shoko.Plugin.Abstractions;
using Shoko.Plugin.Abstractions.Attributes;

// TODO maybe update this with some way to run another existing renamer config
/*
namespace Renamer.Cazzar
{
    [RenamerID("NoSpaces", Description = "Renamer that doesn't use spaces")]
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
*/