using System;
using Shoko.Abstractions.Plugin;

namespace Renamer.Cazzar;

/// <summary>
///   Shoko discovers plugins by looking for an <see cref="IPlugin"/>
///   implementation in the assembly, so the renamer needs one to be loaded.
/// </summary>
public class CazzarPlugin : IPlugin
{
    // Carried over from the assembly GUID this project has always used, so the
    // plugin keeps its identity in Shoko's stored configuration.
    public Guid ID => new("534cbed0-df97-4085-8916-912ba52e08f0");

    public string Name => "Cazzar Renamer";

    public string Description => "Cazzar's custom renamer.";
}
