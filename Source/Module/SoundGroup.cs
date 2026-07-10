using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Module;

public class SoundGroup
{
    public string DisplayName { get; set; }
    public int Volume { get; set; } = VolumeConstants.DefaultVolume;
    public List<string> EventPaths { get; set; } = new();
}
