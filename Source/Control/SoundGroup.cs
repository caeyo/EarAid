using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Control;

public class SoundGroup
{
    public string DisplayName { get; set; }
    public int Volume { get; set; } = VolumeConstants.DefaultVolume;
    public List<string> EventPaths { get; set; } = [];
}
