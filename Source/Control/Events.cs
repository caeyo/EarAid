using FMOD.Studio;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.EarAid.Control;

public static class Events
{
    public static readonly Dictionary<string, float> PathToVolume = new();
    public static readonly Dictionary<EventDescription, float> DescriptionToVolume = new();

    private static List<string> cachedSortedPaths;

    public static bool HasRegisteredPaths => PathToVolume.Count > 0;

    public static IReadOnlyList<string> SortedKnownPaths => cachedSortedPaths ??= BuildSortedPaths();

    public static void RebuildRegistry(IEnumerable<SoundGroup> groups)
    {
        IEnumerable<SoundGroup> soundGroups = groups as SoundGroup[] ?? groups.ToArray();
        int pathCount = soundGroups.Sum(group => group.EventPaths.Count);

        PathToVolume.Clear();
        DescriptionToVolume.Clear();

        if (pathCount > 0)
        {
            PathToVolume.EnsureCapacity(pathCount);
            DescriptionToVolume.EnsureCapacity(pathCount);
        }

        foreach (SoundGroup group in soundGroups)
        {
            float volume = VolumeConstants.ToFloat(group.Volume);

            foreach (string path in group.EventPaths)
            {
                if (!PathToVolume.TryAdd(path, volume))
                {
                    Logger.Warn(nameof(Events), $"Duplicate event path in SoundGroups: {path}");
                    continue;
                }

                EventDescription description = Audio.GetEventDescription(path);
                if (description != null)
                {
                    DescriptionToVolume[description] = volume;
                }
            }
        }
    }

    public static void SetGroupVolume(SoundGroup group)
    {
        float volume = VolumeConstants.ToFloat(group.Volume);

        foreach (string path in group.EventPaths)
        {
            PathToVolume[path] = volume;

            EventDescription description = Audio.GetEventDescription(path);
            if (description != null)
            {
                DescriptionToVolume[description] = volume;
            }
        }
    }

    public static bool IsEventAvailable(string path)
    {
        return Audio.GetEventDescription(path) != null;
    }

    public static HashSet<string> GetAssignedEventPaths(IEnumerable<SoundGroup> groups, SoundGroup excludeGroup = null)
    {
        HashSet<string> assigned = [];
        foreach (SoundGroup group in groups)
        {
            if (group == excludeGroup)
            {
                continue;
            }

            foreach (string path in group.EventPaths)
            {
                assigned.Add(path);
            }
        }
        return assigned;
    }

    private static List<string> BuildSortedPaths()
    {
        HashSet<string> paths = [];

        foreach (string path in Audio.cachedPaths.Values.Where(path => path.StartsWith("event:/")))
        {
            paths.Add(path);
        }

        foreach (string path in Audio.cachedModEvents.Keys)
        {
            paths.Add(path);
        }

        List<string> sorted = new(paths.Count);
        sorted.AddRange(paths);
        sorted.Sort();
        return sorted;
    }
}
