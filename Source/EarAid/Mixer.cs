using System.Collections.Generic;
using Celeste.Mod.EarAid.EarAid;
using FMOD;
using FMOD.Studio;

namespace Celeste.Mod.EarAid;

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
        string path = Audio.GetEventName(self);

        if (Events.PathToVolume.TryGetValue(path, out int volume))
        {
            instance?.setVolume(volume / 10f);
        }

        return result;
    }

    public static void MixExistingInstances(string path, int volume)
    {
        if (!Events.IsEventAvailable(path))
        {
            return;
        }

        EventDescription eventDesc = Audio.GetEventDescription(path);

        if (eventDesc?.getInstanceList(out EventInstance[] instanceArray) is RESULT.OK)
        {
            for (int i = 0; i < instanceArray.Length; i++)
            {
                instanceArray[i].setVolume(volume / 10f);
            }
        }
    }

    public static void MixExistingInstances(IEnumerable<string> paths, int volume)
    {
        foreach (string path in paths)
        {
            MixExistingInstances(path, volume);
        }
    }
}
