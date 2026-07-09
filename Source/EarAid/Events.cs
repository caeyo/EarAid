using Celeste.Mod.EarAid.Module;
using FMOD.Studio;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.EarAid.EarAid;

public static class Events
{
    public static readonly Dictionary<string, int> PathToVolume = new();

    public static void RebuildRegistry(IEnumerable<SoundGroup> groups)
    {
        PathToVolume.Clear();

        foreach (SoundGroup group in groups)
        {
            foreach (string path in group.EventPaths)
            {
                if (PathToVolume.ContainsKey(path))
                {
                    Logger.Warn(nameof(Events), $"Duplicate event path in SoundGroups: {path}");
                    continue;
                }

                PathToVolume[path] = group.Volume;
            }
        }
    }

    public static bool IsEventAvailable(string path)
    {
        return Audio.GetEventDescription(path) != null;
    }

    public static HashSet<string> GetAssignedEventPaths(IEnumerable<SoundGroup> groups)
    {
        HashSet<string> assigned = new();
        foreach (SoundGroup group in groups)
        {
            foreach (string path in group.EventPaths)
            {
                assigned.Add(path);
            }
        }
        return assigned;
    }

    public static List<string> GetAllKnownEventPaths()
    {
        HashSet<string> paths = new();

        foreach (string path in Audio.cachedPaths.Values)
        {
            if (path.StartsWith("event:/"))
            {
                paths.Add(path);
            }
        }

        foreach (string path in Audio.cachedModEvents.Keys)
        {
            paths.Add(path);
        }

        List<string> sorted = paths.ToList();
        sorted.Sort();
        return sorted;
    }
}
