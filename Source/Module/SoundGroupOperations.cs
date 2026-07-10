using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.EarAid.Module;

public static class SoundGroupOperations
{
    public static void DeleteGroup(SoundGroup group)
    {
        List<string> paths = new(group.EventPaths);
        EarAidModule.Settings.SoundGroups.Remove(group);
        Events.RebuildRegistry(EarAidModule.Settings.SoundGroups);

        if (EarAidModule.Settings.Enabled)
        {
            Mixer.MixExistingInstances(paths, VolumeConstants.DefaultVolume);
        }

        EarAidModule.Instance.SaveSettings();
    }

    public static void UpdateGroup(SoundGroup group, string displayName, IEnumerable<string> eventPaths)
    {
        List<string> oldPaths = group.EventPaths;
        HashSet<string> newPathSet = new(eventPaths);

        group.DisplayName = displayName.Trim();
        group.EventPaths = new List<string>(eventPaths);

        Events.RebuildRegistry(EarAidModule.Settings.SoundGroups);

        if (EarAidModule.Settings.Enabled)
        {
            IEnumerable<string> removedPaths = oldPaths.Where(path => !newPathSet.Contains(path));
            Mixer.MixExistingInstances(removedPaths, VolumeConstants.DefaultVolume);
            Mixer.MixExistingInstances(group.EventPaths, group.Volume);
        }

        EarAidModule.Instance.SaveSettings();
    }

    public static void RemoveEventFromGroup(SoundGroup group, string eventPath)
    {
        if (group.EventPaths.Count <= 1 || !group.EventPaths.Remove(eventPath))
        {
            return;
        }

        Events.RebuildRegistry(EarAidModule.Settings.SoundGroups);

        if (EarAidModule.Settings.Enabled)
        {
            Mixer.MixExistingInstances(eventPath, VolumeConstants.DefaultVolume);
        }

        EarAidModule.Instance.SaveSettings();
    }
}
