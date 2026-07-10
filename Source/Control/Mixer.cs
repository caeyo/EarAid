using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;

namespace Celeste.Mod.EarAid.Control;

public static class Mixer
{
    public static void LoadHooks()
    {
        On.FMOD.Studio.EventDescription.createInstance += MixNewInstances;
    }

    public static void UnloadHooks()
    {
        On.FMOD.Studio.EventDescription.createInstance -= MixNewInstances;
    }

    private static RESULT MixNewInstances(On.FMOD.Studio.EventDescription.orig_createInstance orig, EventDescription self, out EventInstance instance)
    {
        RESULT result = orig(self, out instance);

        if (Events.HasRegisteredPaths && Events.DescriptionToVolume.TryGetValue(self, out float volume))
        {
            instance?.setVolume(volume);
        }

        return result;
    }

    public static void MixExistingInstances(string path, int volume)
    {
        float scaledVolume = VolumeConstants.ToFloat(volume);
        ForEachInstance(path, instance => instance.setVolume(scaledVolume));
    }

    public static void MixExistingInstances(IEnumerable<string> paths, int volume)
    {
        float scaledVolume = VolumeConstants.ToFloat(volume);
        foreach (string path in paths)
        {
            ForEachInstance(path, instance => instance.setVolume(scaledVolume));
        }
    }

    public static void MixAllRegisteredInstances()
    {
        foreach (KeyValuePair<string, float> entry in Events.PathToVolume)
        {
            ForEachInstance(entry.Key, instance => instance.setVolume(entry.Value));
        }
    }

    private static void ForEachInstance(string path, Action<EventInstance> action)
    {
        if (!Events.IsEventAvailable(path))
        {
            return;
        }

        EventDescription eventDesc = Audio.GetEventDescription(path);

        if (eventDesc?.getInstanceList(out EventInstance[] instanceArray) is not RESULT.OK)
        {
            return;
        }
        foreach (EventInstance eventInst in instanceArray)
        {
            action(eventInst);
        }
    }
}
