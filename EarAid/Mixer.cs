using Celeste.Mod.EarAid.Utils;
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

        if (!string.IsNullOrEmpty(path) && EventConsts.Paths.Contains(path))
        {
            instance?.setVolume(EventConsts.PathToSetting(path) / 10f);
        }

        return result;
    }

    public static void MixExistingInstances(string path, int volume)
    {
        EventDescription eventDesc = Audio.GetEventDescription(path);

        if (eventDesc?.getInstanceList(out EventInstance[] instanceArray) is RESULT.OK)
        {
            eventDesc.getInstanceCount(out int instanceCount);
            for (int i = 0; i < instanceCount; i++)
            {
                instanceArray[i].setVolume(volume / 10f);
            }
        }
    }
}
