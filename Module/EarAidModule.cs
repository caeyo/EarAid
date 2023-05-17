using Celeste.Mod.EarAid.EarAid;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.EarAid.Module;

public class EarAidModule : EverestModule
{
    public static EarAidModule Instance;

    public override Type SettingsType => typeof(EarAidSettings);
    public static EarAidSettings Settings => Instance._Settings as EarAidSettings;

    public static bool Loaded { get; private set; }
    public IEnumerable<PropertyInfo> VolumeSettings { get; private set; }

    private readonly IEnumerable<MethodInfo> hookLoaders;
    private readonly IEnumerable<MethodInfo> hookUnloaders;

    public EarAidModule()
    {
        Instance = this;

        hookLoaders = getMethods("LoadHooks");
        hookUnloaders = getMethods("UnloadHooks");
        IEnumerable<MethodInfo> getMethods(string methodName) => Assembly.GetCallingAssembly().GetTypesSafe()
            .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(methodInfo => methodInfo.Name == methodName));
    }

    public override void Load()
    {
        if (!Loaded && Settings.Enabled)
        {
            foreach (MethodInfo hookLoader in hookLoaders)
            {
                hookLoader.Invoke(null, null);
            }
            Loaded = true;
        }
    }

    public override void Initialize()
    {
        VolumeSettings = typeof(EarAidSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.PropertyType == typeof(int));

        Events.PopulateEventPaths();
    }

    public override void Unload()
    {
        if (Loaded)
        {
            foreach (MethodInfo hookUnloader in hookUnloaders)
            {
                hookUnloader.Invoke(null, null);
            }
            Loaded = false;
        }
    }

    public override void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance snapshot)
    {
        CreateModMenuSectionHeader(menu, inGame, snapshot);
        EarAidMenu.CreateMenu(menu);
    }
}
