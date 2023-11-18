using Celeste.Mod.EarAid.EarAid;
using Celeste.Mod.EarAid.Module;
using FMOD;
using FMOD.Studio;
using System.Reflection;

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

        if (Events.PathToSettingGetter.TryGetValue(path, out MethodInfo settingGetter))
        {
            instance?.setVolume((int)settingGetter.Invoke(EarAidModule.Settings, null) / 10f);
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
