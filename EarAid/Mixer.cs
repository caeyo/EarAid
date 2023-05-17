using Celeste.Mod.EarAid.EarAid;
using Celeste.Mod.EarAid.Module;
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

        if (!string.IsNullOrEmpty(path) && Events.PathToSettingGetter.ContainsKey(path))
        {
            instance?.setVolume((int)Events.PathToSettingGetter[path].Invoke(EarAidModule.Settings, null) / 10f);
        }

        return result;
    }

    public static void MixExistingInstances(string path, int volume)
    {
        if (Events.ModdedPaths.Contains(path) && !Audio.cachedModEvents.ContainsKey(path))
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

    public static void MixExistingInstances(string[] paths, int volume)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            MixExistingInstances(paths[i], volume);
        }
    }
}
